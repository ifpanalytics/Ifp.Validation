using Ifp.Validation.TestProxy.Tests.SupportClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ifp.Validation.TestProxy.Tests.Documentation
{
    class DogCollectionValidator: CollectionValidator<Dog>
    {
        public DogCollectionValidator(DogValidator dogValidator) : 
            base(dogValidator)
        {

        }
    }
}
