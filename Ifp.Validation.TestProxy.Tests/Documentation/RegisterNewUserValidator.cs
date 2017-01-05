using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ifp.Validation.TestProxy.Tests.Documentation
{
    public class RegisterNewUserValidator : RuleBasedValidator<RegisterNewUserModel>
    {
        public RegisterNewUserValidator(PasswordValidationRule passwordValidationRule, BirthdateValidationRule birthdateValidationRule)
            : base(passwordValidationRule, birthdateValidationRule)
        {

        }
    }
}
