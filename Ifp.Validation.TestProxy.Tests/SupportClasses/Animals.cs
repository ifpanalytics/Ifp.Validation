using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
