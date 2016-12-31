using System;
using System.Collections.Generic;
using System.Linq;

namespace Ifp.Validation
{
    /// <summary>
    /// Classes that validate other objects should implement this interface.
    /// Classes that implement this interface can be combined to rules sets by the <see cref="RuleBasedValidator{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to validate.</typeparam>
    /// <remarks>
    /// You should not implement this interface directly. Use <see cref="ValidationRule{T}"/> as base class instead.
    /// </remarks>
    /// <seealso cref="ValidationRule{T}"/>
    /// <seealso cref="ValidationRuleDelegate{T}"/>
    public interface IValidationRule<in T>
    {
        /// <summary>
        /// Checks the <paramref name="objectToValidate"/> and returns a <see cref="ValidationOutcome"/>. 
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <returns>A <see cref="ValidationOutcome"/> that represents the result of the validation.</returns>
        ValidationOutcome ValidateObject(T objectToValidate);

        /// <summary>
        /// Returns true if the <see cref="RuleBasedValidator{T}"/> should not proceed validation tests in case the validation returns a <see cref="ValidationOutcome"/> with <see cref="ValidationSeverity.IsAnError"/>. 
        /// This is useful to prevent further processing of rules in cases where all the following rules will also fail. A typical use case is a check for <c>objectToValidate == null</c>.
        /// </summary>
        bool CausesValidationProcessToStop { get; }
    }

    /// <summary>
    /// The base class for implementing the validation logic. <see cref="ValidateObject(T)"/> must be implemented with the validation logic.
    /// </summary>
    /// <typeparam name="T">The type of the object to validate.</typeparam>
    public abstract class ValidationRule<T> : IValidationRule<T>
    {
        /// <summary>
        /// Checks the <paramref name="objectToValidate"/> and returns a <see cref="ValidationOutcome"/>. 
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <returns>A <see cref="ValidationOutcome"/> that represents the result of the validation.</returns>
        /// <remarks>
        /// You can use the <see cref="ValidationSummaryBuilderExtensions.ToFailure(string, FailureSeverity)"/> extension method to construct a <see cref="ValidationOutcome"/>. 
        /// </remarks>
        /// <example>
        /// <code language="cs">
        /// Return a <see cref="ValidationOutcome"/> using the extension method <see cref="ValidationSummaryBuilderExtensions.ToFailure(string, FailureSeverity)"/>.
        /// public override ValidationOutcome ValidateObject(Animal objectToValidate)
        /// {
        ///     return "This is an error message.".ToFailure(FailureSeverity.Error);
        /// }
        /// </code>
        /// </example>
        public abstract ValidationOutcome ValidateObject(T objectToValidate);

        /// <summary>
        /// Returns always false. Override this property and return true if
        /// the <see cref="RuleBasedValidator{T}"/> should not proceed validation tests in case this validation rule returns a <see cref="ValidationOutcome"/> with <see cref="ValidationSeverity.IsAnError"/>. 
        /// This is useful to prevent further processing of rules in cases where all the following rules will also fail. A typical use case is a check for <c>objectToValidate == null</c>.
        /// </summary>
        public virtual bool CausesValidationProcessToStop => false;
    }

    /// <summary>
    /// The <see cref="ValidationFunction{T}"/> delegate defines a function that can validate another object.
    /// The signature is the same as in <see cref="IValidationRule{T}.ValidateObject(T)"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to validate.</typeparam>
    /// <param name="objectToValidate">The object to validate.</param>
    /// <returns>A <see cref="ValidationOutcome"/> that represents the result of the validation.</returns>
    public delegate ValidationOutcome ValidationFunction<T>(T objectToValidate);

    /// <summary>
    /// A class that takes a <see cref="ValidationFunction{T}"/> delegate to perform the validation. This allows to define a validation rule without the need to implement a class that derives from <see cref="ValidationRule{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to validate.</typeparam>
    public class ValidationRuleDelegate<T> : IValidationRule<T>
    {

        /// <summary>
        /// Constructs a new <see cref="ValidationRuleDelegate{T}"/> object and takes a <see cref="ValidationFunction{T}"/> delegate that does the validation.
        /// </summary>
        /// <param name="validationFunction">The <see cref="ValidationFunction{T}"/> delegate, that implements the validation logic.</param>
        /// <remarks>
        /// The <see cref="IValidationRule{T}.CausesValidationProcessToStop"/> property is <c>false</c>.
        /// </remarks>
        public ValidationRuleDelegate(ValidationFunction<T> validationFunction)
            : this(validationFunction, false)
        {
        }

        /// <summary>
        /// Constructs a new <see cref="ValidationRuleDelegate{T}"/> object and takes a <see cref="ValidationFunction{T}"/> delegate that does the validation.
        /// </summary>
        /// <param name="validationFunction">The <see cref="ValidationFunction{T}"/> delegate, that implements the validation logic.</param>
        /// <param name="causesValidationProcessToStop">Pass <c>true</c> to prevent the <see cref="RuleBasedValidator{T}"/> to proceed with more <see cref="IValidationRule{T}"/>.</param>
        public ValidationRuleDelegate(ValidationFunction<T> validationFunction, bool causesValidationProcessToStop)
        {
            ValidationFunction = validationFunction;
            CausesValidationProcessToStop = causesValidationProcessToStop;
        }

        /// <summary>
        /// The <see cref="ValidationFunction{T}"/> passes to the constructor.
        /// </summary>
        protected ValidationFunction<T> ValidationFunction { get; }

        /// <summary>
        /// Implements <see cref="IValidationRule{T}.ValidateObject(T)"/> by delegating to the validation function passed in the constructor.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <returns>The <see cref="ValidationOutcome"/>.</returns>
        public ValidationOutcome ValidateObject(T objectToValidate) => ValidationFunction(objectToValidate);

        /// <summary>
        /// Use the constructor <see cref="ValidationRuleDelegate(ValidationFunction{T}, bool)"/> to configure this property.
        /// Return true if the <see cref="RuleBasedValidator{T}"/> should not proceed validation tests in case this validation rule returns a <see cref="ValidationOutcome"/> with <see cref="ValidationSeverity.IsAnError"/>. 
        /// This is useful to prevent further processing of rules in cases where all the following rules will also fail. A typical use case is a check for <c>objectToValidate == null</c>.
        /// </summary>
        public bool CausesValidationProcessToStop { get; }
    }
}
