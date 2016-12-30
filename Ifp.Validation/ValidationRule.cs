using System;
using System.Collections.Generic;
using System.Linq;

namespace Ifp.Validation
{
    public interface IValidationRule<in T>
    {
        ValidationOutcome ValidateObject(T objectToValidate);
        bool CausesValidationProcessToStop { get; }
    }

    public abstract class ValidationRule<T> : IValidationRule<T>
    {

        public abstract ValidationOutcome ValidateObject(T objectToValidate);

        public virtual bool CausesValidationProcessToStop => false;
    }

    public class ValidationRuleDelegate<T> : IValidationRule<T>
    {

        public ValidationRuleDelegate(ValidationFunction<T> validationFunction)
            : this(validationFunction, false)
        {
        }
        public ValidationRuleDelegate(ValidationFunction<T> validationFunction, bool causesValidationProcessToStop)
        {
            ValidationFunction = validationFunction;
            CausesValidationProcessToStop = causesValidationProcessToStop;
        }

        protected ValidationFunction<T> ValidationFunction { get; }

        public ValidationOutcome ValidateObject(T objectToValidate) => ValidationFunction(objectToValidate);

        public bool CausesValidationProcessToStop { get; }
    }
}
