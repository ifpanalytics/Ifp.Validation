using Ifp.Validation.TestProxy.Tests.SupportClasses;

namespace Ifp.Validation.TestProxy.Tests.Documentation
{
    // A CollectionValidator that can validate an IEnumerable<Dog>
    class DogCollectionValidator : CollectionValidator<Dog>
    {
        // Wrap an existing RuleBasedValidator 
        public DogCollectionValidator(DogValidator dogValidator) :
            base(dogValidator)
        {
        }

        // Or construct a CollectionValidator out of rules 
        public DogCollectionValidator(DogMustBeOlderThan2Years rule1, DogMustBeMale rule2) :
            base(rule1, rule2)
        {
        }
    }

    class UseTheValidator
    {
        void Use()
        {
            var validator = new DogCollectionValidator(new DogValidator(new DogMustBeOlderThan2Years(), new DogMustBeMale()));
            // Use the validator
            var dogs = new Dog[] { new Dog(), new Dog() };
            var summary = validator.ValidateCollection(dogs);
        }
    }
}
