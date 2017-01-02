using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ifp.Validation.TestProxy.Tests.SupportClasses
{
    public class AnimalShouldNotBiteRule : ValidationRule<Animal>
    {
        public override ValidationOutcome ValidateObject(Animal objectToValidate)
        {
            return ValidationOutcome.Failure(FailureSeverity.Error, "Error message");
            return "This is an error message.".ToFailure(FailureSeverity.Error);
        }
    }
}
