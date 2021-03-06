namespace CreditOne.Microservices.Sso.Models.Response
{
    /// <summary>
    /// PickUp Response Model for Ping Federate SSO Microservice.
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
    public class PickUpResponse
    {
        public string AccountId { get; set; }
        public string StartURL { get; set; }
        public string LogoutURL { get; set; }
        public string ReturnURL { get; set; }
        public string KeepAliveSessionURL1 { get; set; }
        public string KeepAliveSessionURL2 { get; set; }
    }
}
