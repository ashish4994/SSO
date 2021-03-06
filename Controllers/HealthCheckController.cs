using System;
using System.Net;

using Microsoft.AspNetCore.Mvc;

namespace CreditOne.Microservices.Sso.API.Controllers
{
    /// <summary>
    /// Implements health check controller class
    /// </summary>
    /// <remarks>
    /// <list type="table">
    /// <listheader>
    ///     <term>Date</term>
    ///     <term>Who</term>
    ///     <term>BR/WO</term>
    ///     <description>Description</description>
    /// </listheader>
    /// <item>
    ///     <term>7/14/2020</term>
    ///     <term>Levi Tamiozzo</term>
    ///     <term>MK-1001</term>
    ///     <description>Initial implementation</description>
    /// </item>
    /// </list>
    /// </remarks>
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/health")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        /// <summary>
        /// Checks if the microservice is running
        /// </summary>
        /// <remarks>
        /// Sample Request:   
        /// 
        ///     GET /api/v1/health
        ///     
        /// </remarks>        
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public ActionResult<string> Check()
        {
            return Ok($"SSO microservice is up and running as is at {DateTime.Now.ToString()}. Server Name: {Environment.MachineName}.");
        }   
    }
}
