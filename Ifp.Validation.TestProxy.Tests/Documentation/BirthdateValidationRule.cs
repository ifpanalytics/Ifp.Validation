using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ifp.Validation.TestProxy.Tests.Documentation
{
    public class BirthdateValidationRule : ValidationRule<RegisterNewUserModel>
    {
        public override ValidationOutcome ValidateObject(RegisterNewUserModel objectToValidate)
        {
            // Use the ToFailure extension method for strings to create a ValidationOutcome.
            if (objectToValidate.BithDate == null)
                return "You did not enter a birth date. You will not be able to use some of our services.You can add this information later.".ToFailure(FailureSeverity.Information);
            // Return ValidationOutcome.Success to indicate success.
            return ValidationOutcome.Success;
        }
    }
}
