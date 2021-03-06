using CreditOne.Microservices.BuildingBlocks.RequestValidationFilter.Filter;
using Newtonsoft.Json;

namespace CreditOne.Microservices.Sso.Models.Request
{
    /// <summary>
    /// Drop Off Body Request Model for Ping Federate SSO Microservice.
    /// </summary>
    /// <remarks>
    /// <list type="table">
    /// <listheader>
    ///  <term>Date</term>
    ///  <term>Who</term>
    ///  <term>BR/WO</term>
    ///  <description>Description</description>
    /// </listheader>
    /// <item>
    ///  <term>05/14/2020</term>
    ///  <term>Levi Tamiozzo</term>
    ///  <term>MK-1001</term>
    ///  <description>Initial implementation</description>
    /// </item>
    /// </list>
    /// </remarks> 
    public class DropOffBodyRequest
    {
        [JsonProperty("accountId")]
        [ActionParameter("AccountId", Required = true)]
        public string AccountId { get; set; }

        [JsonProperty("startUrl")]
        [ActionParameter("StartURL", Required = true)]
        public string StartURL { get; set; }

        [JsonProperty("logoutUrl")]
        [ActionParameter("LogoutURL", Required = true)]
        public string LogoutURL { get; set; }

        [JsonProperty("returnUrl")]
        [ActionParameter("ReturnURL", Required = true)]
        public string ReturnURL { get; set; }

        [JsonProperty("keepAliveSessionUrl1")]
        [ActionParameter("KeepAliveSessionURL1", Required = true)]
        public string KeepAliveSessionURL1 { get; set; }

        [JsonProperty("keepAliveSessionUrl2")]
        [ActionParameter("KeepAliveSessionURL2", Required = true)]
        public string KeepAliveSessionURL2 { get; set; }
    }
}
