using CreditOne.Microservices.BuildingBlocks.RequestValidationFilter.Filter;

namespace CreditOne.Microservices.Sso.Models.Request
{
    /// <summary>
    /// Header Request Model for Ping Federate SSO Microservice.
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
    ///  <term>12/10/2020</term>
    ///  <term>Levi Tamiozzo</term>
    ///  <term>MK-1001</term>
    ///  <description>Initial implementation</description>
    /// </item>
    /// </list>
    /// </remarks> 
    public class HeaderRequest
    {
        [ActionParameter("BaseURL", Required = true)]
        public string BaseURL { get; set; }

        [ActionParameter("UserName", Required = true)]
        public string UserName { get; set; }

        [ActionParameter("Password", Required = true)]
        public string Password { get; set; }

        [ActionParameter("PartnerSpId", Required = true)]
        public string PartnerSpId { get; set; }

        [ActionParameter("IdpAdapterId", Required = true)]
        public string IdpAdapterId { get; set; }
    }
}

