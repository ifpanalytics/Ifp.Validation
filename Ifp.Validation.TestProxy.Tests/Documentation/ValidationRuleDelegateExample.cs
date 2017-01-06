using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ifp.Validation.TestProxy.Tests.Documentation
{
    // The validator expects a RegisterNewUserModel.
    class ValidationRuleDelegateExample : RuleBasedValidator<RegisterNewUserModel>
    {
        // Use the ValidationRuleDelegate to delegate the validation to the EMailAddressValidationRule
        public ValidationRuleDelegateExample(EMailAddressValidationRule emailAddressValidationRule) :
            base(new ValidationRuleDelegate<RegisterNewUserModel>(model => emailAddressValidationRule.ValidateObject(model.EMail)))
            {

            }
    }
}
