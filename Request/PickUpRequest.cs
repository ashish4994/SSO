using CreditOne.Microservices.BuildingBlocks.RequestValidationFilter.Filter;

namespace CreditOne.Microservices.Sso.Models.Request
{
    /// <summary>
    /// Pick Up Request Model for Ping Federate SSO Microservice.
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
    public class PickUpRequest : HeaderRequest
    {
        [ActionParameter("REF", Required = true)]
        public string REF { get; set; }
    }
}
