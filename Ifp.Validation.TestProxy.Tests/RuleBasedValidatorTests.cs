using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ifp.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ifp.Validation.TestProxy.Tests.SupportClasses;

namespace Ifp.Validation.Tests
{
    [TestClass()]
    public class RuleBasedValidatorTests
    {
        [TestMethod()]
        public void RuleBasedValidatorTest()
        {
            var validator=new RuleBasedValidator<Dog>(new AnimalShouldNotBiteRule());

        }
    }
}