using System.Collections.Generic;

namespace Ifp.Validation.TestProxy.Tests.SupportClasses
{
    public class Animal
    {
    }

    public class Dog : Animal
    {

    }

    public class Zoo
    {
        public Zoo(params Animal[] animals)
        {
            Animals = animals;
        }

        public IEnumerable<Animal> Animals { get; }
    }
}
