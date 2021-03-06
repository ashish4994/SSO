namespace CreditOne.Microservices.Sso.Models.Response
{
    /// <summary>
    /// Drop Off Response Model for Ping Federate SSO Microservice.
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
    public class DropOffResponse
    {
        public string REF { get; set; }
        public string RedirectUrl { get; set; }
    }
}
