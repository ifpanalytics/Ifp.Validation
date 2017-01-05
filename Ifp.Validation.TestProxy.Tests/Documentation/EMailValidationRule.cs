using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ifp.Validation.TestProxy.Tests.Documentation
{
    class EMailValidationRule : ValidationRule<RegisterNewUserModel>
    {
        // Import other services per constructor injection if needed
        public EMailValidationRule(IRegisteredUserRepository registeredUserRepository, IValidateEmailAdressesAccordingRFC5322 validateEmailAdressesAccordingRFC5322)
        {
            this.RegisteredUserRepository = registeredUserRepository;
            this.ValidateEmailAdressesAccordingRFC5322 = validateEmailAdressesAccordingRFC5322;
        }

        protected IRegisteredUserRepository RegisteredUserRepository { get; }
        protected IValidateEmailAdressesAccordingRFC5322 ValidateEmailAdressesAccordingRFC5322 { get; }

        public override ValidationOutcome ValidateObject(RegisterNewUserModel objectToValidate)
        {
            if (!ValidateEmailAdressesAccordingRFC5322.IsValidEmailAdsress(objectToValidate.EMail))
                return $"The email address {objectToValidate.EMail} is not a valid mail address.".ToFailure(FailureSeverity.Error);
            if (RegisteredUserRepository.GetUserByEMailaddress(objectToValidate.EMail) != null)
                return $"The email address {objectToValidate.EMail} is already registered. If you have forgotten your password, you can use our 'Forgotten password' service.".ToFailure(FailureSeverity.Error);
            return ValidationOutcome.Success;
        }
    }
}
