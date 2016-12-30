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
            if (objectToValidate is Dog)
                return "Can bite".ToFailure(FailureSeverity.Error);
            return ValidationOutcome.Success;
        }
    }
}
