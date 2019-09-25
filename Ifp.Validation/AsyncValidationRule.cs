using System.Threading.Tasks;

namespace Ifp.Validation
{

    /// <summary>
    /// The base class for implementing the validation logic. <see cref="ValidateObjectAsync(T)"/> must be implemented with the validation logic.
    /// </summary>
    /// <typeparam name="T">The type of the object to validate.</typeparam>
    public abstract class AsyncValidationRule<T> : IAsyncValidationRule<T>
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
        /// <para>Indicate success by returning <see cref="ValidationOutcome.Success"/>.</para>
        /// <code language="cs">
        /// public override ValidationOutcome ValidateObject(Animal objectToValidate)
        /// {
        ///     return ValidationOutcome.Success;
        /// }
        /// </code>
        /// 
        /// <para>Return a <see cref="ValidationOutcome"/> by using the <see cref="ValidationOutcome.Failure(FailureSeverity, string)"/> method.</para>
        /// <code language="cs">
        /// public override ValidationOutcome ValidateObject(Animal objectToValidate)
        /// {
        ///     return ValidationOutcome.Failure(FailureSeverity.Error, "This is an error message.");
        /// }
        /// </code>
        /// 
        /// <para>Return a <see cref="ValidationOutcome"/> using the <see cref="string"/> extension method 
        /// <see cref="ValidationSummaryBuilderExtensions.ToFailure(string, FailureSeverity)"/>.</para> 
        /// <code language="cs">
        /// public override ValidationOutcome ValidateObject(Animal objectToValidate)
        /// {
        ///     return "This is an error message.".ToFailure(FailureSeverity.Error);
        /// }
        /// </code> 
        /// </example>
        public abstract Task<ValidationOutcome> ValidateObjectAsync(T objectToValidate);

        /// <summary>
        /// Returns always false. Override this property and return true if
        /// the <see cref="AsyncRuleBasedValidator{T}"/> should not proceed validation tests in case this validation rule returns a <see cref="ValidationOutcome"/> with <see cref="ValidationSeverity.IsAnError"/>. 
        /// This is useful to prevent further processing of rules in cases where all the following rules will also fail. A typical use case is a check for <c>objectToValidate == null</c>.
        /// </summary>
        public virtual bool CausesValidationProcessToStop => false;
    }

    /// <summary>
    /// The <see cref="AsyncValidationFunction{T}"/> delegate defines a function that can validate another object.
    /// The signature is the same as in <see cref="IAsyncValidationRule{T}.ValidateObjectAsync(T)"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to validate.</typeparam>
    /// <param name="objectToValidate">The object to validate.</param>
    /// <returns>A <see cref="ValidationOutcome"/> that represents the result of the validation.</returns>
    public delegate Task<ValidationOutcome> AsyncValidationFunction<T>(T objectToValidate);

    /// <summary>
    /// A class that takes a <see cref="AsyncValidationFunction{T}"/> delegate to perform the validation. This allows to define a validation rule without the need to implement a class that derives from <see cref="AsyncValidationRule{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to validate.</typeparam>
    public class AsyncValidationRuleDelegate<T> : IAsyncValidationRule<T>
    {

        /// <summary>
        /// Constructs a new <see cref="AsyncValidationRuleDelegate{T}"/> object and takes a <see cref="AsyncValidationFunction{T}"/> delegate that does the validation.
        /// </summary>
        /// <param name="validationFunction">The <see cref="ValidationFunction{T}"/> delegate, that implements the validation logic.</param>
        /// <remarks>
        /// The <see cref="IAsyncValidationRule{T}.CausesValidationProcessToStop"/> property is <c>false</c>.
        /// </remarks>
        public AsyncValidationRuleDelegate(AsyncValidationFunction<T> validationFunction)
            : this(validationFunction, false)
        {
        }

        /// <summary>
        /// Constructs a new <see cref="AsyncValidationRuleDelegate{T}"/> object and takes a <see cref="AsyncValidationFunction{T}"/> delegate that does the validation.
        /// </summary>
        /// <param name="validationFunction">The <see cref="ValidationFunction{T}"/> delegate, that implements the validation logic.</param>
        /// <param name="causesValidationProcessToStop">Pass <c>true</c> to prevent the <see cref="AsyncRuleBasedValidator{T}"/> to proceed with more <see cref="IAsyncValidationRule{T}"/>.</param>
        public AsyncValidationRuleDelegate(AsyncValidationFunction<T> validationFunction, bool causesValidationProcessToStop)
        {
            ValidationFunction = validationFunction;
            CausesValidationProcessToStop = causesValidationProcessToStop;
        }

        /// <summary>
        /// The <see cref="AsyncValidationFunction{T}"/> passes to the constructor.
        /// </summary>
        protected AsyncValidationFunction<T> ValidationFunction { get; }

        /// <summary>
        /// Implements <see cref="IAsyncValidationRule{T}.ValidateObjectAsync(T)"/> by delegating to the validation function passed in the constructor.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <returns>The <see cref="ValidationOutcome"/>.</returns>
        public async Task<ValidationOutcome> ValidateObjectAsync(T objectToValidate) 
            => await ValidationFunction(objectToValidate);

        /// <summary>
        /// Use the constructor <see cref="AsyncValidationRuleDelegate(AsyncValidationFunction{T}, bool)"/> to configure this property.
        /// Return true if the <see cref="AsyncRuleBasedValidator{T}"/> should not proceed validation tests in case this validation rule returns a <see cref="ValidationOutcome"/> with <see cref="ValidationSeverity.IsAnError"/>. 
        /// This is useful to prevent further processing of rules in cases where all the following rules will also fail. A typical use case is a check for <c>objectToValidate == null</c>.
        /// </summary>
        public bool CausesValidationProcessToStop { get; }
    }
}
