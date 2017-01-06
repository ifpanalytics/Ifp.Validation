using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ifp.Validation.TestProxy.Tests.SupportClasses
{
    public class AnimalMustBeMaleRule : ValidationRule<Animal>
    {
        public override ValidationOutcome ValidateObject(Animal objectToValidate) => "This is an error message.".ToFailure(FailureSeverity.Error);
    }

    class DogMustBeOlderThan2Years : ValidationRule<Dog>
    {
        public override ValidationOutcome ValidateObject(Dog objectToValidate) => ValidationOutcome.Success;
    }

    class DogMustBeMale : ValidationRule<Dog>
    {
        public override ValidationOutcome ValidateObject(Dog objectToValidate) => ValidationOutcome.Success;
    }

    class DogValidator : RuleBasedValidator<Dog>
    {
        public DogValidator(DogMustBeOlderThan2Years rule1, DogMustBeMale rule2) :
            base(rule1, rule2)
        {

        }
    }
}
