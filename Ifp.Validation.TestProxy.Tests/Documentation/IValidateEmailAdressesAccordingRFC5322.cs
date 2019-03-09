namespace Ifp.Validation.TestProxy.Tests.Documentation
{
    public interface IValidateEmailAdressesAccordingRFC5322
    {
        bool IsValidEmailAdsress(string emailAddress);
    }

    public class ValidateEMailAdressesAccordingRFC5322 : IValidateEmailAdressesAccordingRFC5322
    {
        public bool IsValidEmailAdsress(string emailAddress) => true;
    }
}
