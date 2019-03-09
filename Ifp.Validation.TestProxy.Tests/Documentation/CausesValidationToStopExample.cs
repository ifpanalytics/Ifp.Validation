using FluentAssertions;
using System;
using System.Linq;
using System.Security.Principal;
using Xunit;

namespace Ifp.Validation.TestProxy.Tests.Documentation
{
    class ArgumentCantBeNullRule<T> : ValidationRule<T> where T : class
    {
        public override ValidationOutcome ValidateObject(T objectToValidate)
        {
            if (objectToValidate == null)
            {
                return "The argument must be specified.".ToFailure(FailureSeverity.Error);
            }

            return ValidationOutcome.Success;
        }
        public override bool CausesValidationProcessToStop => true;
    }

    class UserMustBeAuthenticated : ValidationRule<IIdentity>
    {
        public override ValidationOutcome ValidateObject(IIdentity objectToValidate)
        {
            // This might cause a NullReferenceException
            if (!objectToValidate.IsAuthenticated)
            {
                return "User must be authenticated.".ToFailure(FailureSeverity.Error);
            }

            return ValidationOutcome.Success;
        }
    }

    class IdentityValidator : RuleBasedValidator<IIdentity>
    {
        public IdentityValidator(ArgumentCantBeNullRule<IIdentity> rule1, UserMustBeAuthenticated rule2) :
            base(rule1, rule2)
        {

        }
    }

    public class CausesValidationToStopTestClass
    {
        [Fact]
        public void CausesValidationToStopExampleTest()
        {
            var identityValidator = new IdentityValidator(new ArgumentCantBeNullRule<IIdentity>(), new UserMustBeAuthenticated());
            // Because the first rule returns an error, the second rule will not be processed.
            var summary = identityValidator.Validate(null);
            summary.ValidationOutcomes.Should().HaveCount(1);
            summary.Severity.Should().Be(ValidationSeverity.Error);
            summary.ValidationOutcomes.Should().AllBeAssignableTo<ValidationOutcomeWithMessage>();
            summary.ValidationOutcomes.Cast<ValidationOutcomeWithMessage>().First().ErrorMessage.Should().Be("The argument must be specified.");
        }

        [Fact]
        public void RuleFailsExampleTest()
        {
            var dogValidator = new RuleBasedValidator<IIdentity>(new UserMustBeAuthenticated());
            Func<ValidationSummary> func = () => dogValidator.Validate(null);
            func.Should().Throw<NullReferenceException>();
        }
    }
}
