using CreditOne.Microservices.Sso.Models.Request;
using CreditOne.Microservices.Sso.Models.Response;

using HtmlAgilityPack;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CreditOne.Microservices.Sso.API.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/sso")]
    public class SsoController : Controller
    {
        #region Private Constants

        private const string DROP_OFF_LOCATION = "/ext/ref/dropoff";
        private const string PICK_UP_LOCATION = "/ext/ref/pickup";
        private const string IDP_START_SSO_LOCATION = "/idp/startSSO.ping";

        #endregion

        #region Private Members

        private readonly ILogger _logger;
        private readonly HttpClient httpClient;

        #endregion

        #region Constructor

        public SsoController(ILogger<SsoController> logger)
        {
            _logger = logger;

            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };

            httpClient = new HttpClient(handler);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Performs a Drop Off operation on the Ping Federate SSO server.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/v1/sso/dropoff
        /// 
        ///     {
        ///         "BaseURL": "https://lvtstpfeng01:9031",
        ///         "UserName": "FiservUser",
        ///         "Password": "Password01!",
        ///         "PartnerSpId": "FiservCat_LVTSTPF",
        ///         "IdpAdapterId": "AgentlessIdpAdaptor",
        ///         "Body": {
        ///             "AccountId": 1234567890123456,
        ///             "StartUrl": "https://cat-plp.firstdata.com/creditonerewards/",
        ///             "LogoutURL": "https://creditOneBank.com/transactional/index",
        ///             "ReturnURL": "https://creditOneBank.com/transactional/index",
        ///             "KeepAliveSessionURL1": "https://creditOneBank.com/api/v1/authentication/keep-session-alive",
        ///             "KeepAliveSessionURL2": "https://creditOneBank.com/api/command"
        ///         }
        ///     }
        /// 
        /// Sample response:
        /// 
        ///     {
        ///         "REF": "0EDD6AF1B293CD04640739FF150B999D55C63AD138C60978FD9500000065",
        ///         "RedirectUrl": "https://cat-plp.firstdata.com/creditonerewards/?jwt=f23j@#JFasdjf923jASJDfjasd..."
        ///     }
        /// 
        /// </remarks>
        /// <param name="dropOffModel">DropOff parameters</param>
        /// <returns>A Drop Off response containing a REF number.</returns>
        [HttpPost]
        [Route("dropoff")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<DropOffResponse>> DropOff([FromBody] DropOffRequest dropOffModel)
        {
            try
            {
                #if DEBUG
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                #endif

                var dropOffJson = JsonConvert.SerializeObject(dropOffModel.Body);
                var dropOffHttpContent = new StringContent(dropOffJson, Encoding.UTF8, "application/json");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes($"{dropOffModel.UserName}:{dropOffModel.Password}")));

                var dropOffUrl = $"{dropOffModel.BaseURL}{DROP_OFF_LOCATION}";
                _logger.LogInformation(dropOffHttpContent.ToString());
                var dropOffResponse = await httpClient.PostAsync(dropOffUrl, dropOffHttpContent);

                if (dropOffResponse.IsSuccessStatusCode)
                {
                    var parsedDropOffResponse = dropOffResponse.Content.ReadAsAsync<DropOffResponse>().Result;

                    var idpQueryParameters = new Dictionary<string, string>();
                    idpQueryParameters.Add("PartnerSpId", dropOffModel.PartnerSpId);
                    idpQueryParameters.Add("IdpAdapterId", dropOffModel.IdpAdapterId);
                    idpQueryParameters.Add("REF", parsedDropOffResponse.REF);

                    var idpUrl = QueryHelpers.AddQueryString($"{dropOffModel.BaseURL}{IDP_START_SSO_LOCATION}", idpQueryParameters);

                    httpClient.DefaultRequestHeaders.Clear();
                    var idpResponse = await httpClient.GetAsync(idpUrl);

                    if (idpResponse.IsSuccessStatusCode)
                    {
                        var htmlDoc = new HtmlDocument();
                        htmlDoc.LoadHtml(await idpResponse.Content.ReadAsStringAsync());

                        var SAMLUrl = htmlDoc.DocumentNode.SelectSingleNode("//form").Attributes["action"].Value;
                        var SAMLName = htmlDoc.DocumentNode.SelectSingleNode("//form/input").Attributes["name"].Value;
                        var SAMLValue = htmlDoc.DocumentNode.SelectSingleNode("//form/input").Attributes["value"].Value;

                        var SAMLcontent = new FormUrlEncodedContent(new Dictionary<string, string>
                        {
                            { SAMLName, SAMLValue }
                        });

                        var SAMLResponse = await httpClient.PostAsync(SAMLUrl, SAMLcontent);

                        if (SAMLResponse.StatusCode == HttpStatusCode.Redirect)
                        {
                            var redirectUrl = SAMLResponse.Headers.Location.OriginalString;

                            parsedDropOffResponse.RedirectUrl = redirectUrl;

                            return Ok(parsedDropOffResponse);
                        }
                        else
                        {
                            var SAMLResponseContentString = SAMLResponse.Content.ReadAsStringAsync().Result;

                            _logger.LogError(SAMLResponseContentString);

                            throw new Exception("The accountId is not a Fiserv account or there was an error when trying to perform the SAML handshake with Fiserv, please check SSO logs for more info.");
                        }
                    }
                    else
                    {
                        var idpResponseContentString = idpResponse.Content.ReadAsStringAsync().Result;

                        _logger.LogError(idpResponseContentString);

                        throw new Exception("There was an error when trying to perform the IDP call, please check SSO logs for more info.");
                    }
                }
                else
                {
                    var dropOffResponseContentString = dropOffResponse.Content.ReadAsStringAsync().Result;

                    _logger.LogError(dropOffResponseContentString);

                    throw new Exception("There was an error when trying to perform the DropOff into C1B servers, please check SSO logs for more info.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Performs a Pick Up operation on the Ping Federate SSO server.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/v1/sso/dropoff
        /// 
        ///     {
        ///         "BaseURL": "https://lvtstpfeng01:9031",
        ///         "UserName": "FiservUser",
        ///         "Password": "Password01!",
        ///         "PartnerSpId": "FiservCat_LVTSTPF",
        ///         "IdpAdapterId": "AgentlessIdpAdaptor",
        ///         "REF": "0EDD6AF1B293CD04640739FF150B999D55C63AD138C60978FD9500000065"
        ///     }
        /// 
        /// Sample response:
        /// 
        ///     {
        ///         "AccountId": 1234567890123456,
        ///         "StartUrl": "https://cat-plp.firstdata.com/creditonerewards/",
        ///         "LogoutURL": "https://creditOneBank.com/transactional/index",
        ///         "ReturnURL": "https://creditOneBank.com/transactional/index",
        ///         "KeepAliveSessionURL1": "https://cat-plp.firstdata.com/keepAlive1",
        ///         "KeepAliveSessionURL2": "https://cat-plp.firstdata.com/keepAlive2"
        ///     }
        /// 
        /// </remarks>
        /// <param name="pickUpModel">PickUp parameters</param>
        /// <returns>A Drop Off response containing a REF number.</returns>
        [HttpPost]
        [Route("pickup")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<PickUpResponse>> PickUp([FromBody] PickUpRequest pickUpModel)
        {
            #if DEBUG
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            #endif
            
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes($"{pickUpModel.UserName}:{pickUpModel.Password}")));
            
            var pickUpUrl = $"{pickUpModel.BaseURL}{PICK_UP_LOCATION}?REF={pickUpModel.REF}";
            var idpResponse = await httpClient.GetAsync(pickUpUrl);

            if (idpResponse.IsSuccessStatusCode)
            {
                var response = idpResponse.Content.ReadAsAsync<PickUpResponse>().Result;

                return Ok(response);
            }

            return NotFound();
        }        

        #endregion

        #region Private Methods



        #endregion
    }
}
