using Ifp.Validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ifp.Validation
{
    /// <summary>
    /// Takes one or more validation rules for <typeparamref name="T"/> and validates <see cref="IEnumerable{T}"/> against that rules.
    /// The validator can be configured to either apply all rules to an object first and than go to the next object or to perform the first rule to
    /// all objects and proceed with the next rule by setting the <see cref="OutterLoopOverRulesInnerLoopOverObjects"/> property.
    /// </summary>
    /// <typeparam name="T">The type of the object to validate.</typeparam>
    public class CollectionValidator<T>
    {
        /// <summary>
        /// Constructs a <see cref="CollectionValidator{T}"/> and takes an enumeration of <see cref="IValidationRule{T}"/>.
        /// </summary>
        /// <param name="validationRules">The </param>
        public CollectionValidator(IEnumerable<IValidationRule<T>> validationRules) : this(validationRules.ToArray())
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="validationRules"></param>
        public CollectionValidator(params IValidationRule<T>[] validationRules)
        {
            ValidationRules = validationRules;
        }

        /// <summary>
        /// TODO
        /// </summary>
        protected IValidationRule<T>[] ValidationRules { get; }
        /// <summary>
        /// TODO
        /// </summary>
        public bool OutterLoopOverRulesInnerLoopOverObjects { get; set; }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="collectionToValidate"></param>
        /// <returns></returns>
        public IEnumerable<ValidationOutcome> ValidateCollection(IEnumerable<T> collectionToValidate) =>
            OutterLoopOverRulesInnerLoopOverObjects ?
            ValidateCollectionRulesFirst(collectionToValidate) :
            ValidateCollectionObjectsFirst(collectionToValidate);

        private IEnumerable<ValidationOutcome> ValidateCollectionRulesFirst(IEnumerable<T> collectionToValidate)
        {
            foreach (var rule in ValidationRules)
            {
                foreach (var objectToValidate in collectionToValidate)
                {
                    var result = rule.ValidateObject(objectToValidate);
                    yield return result;
                    if (rule.CausesValidationProcessToStop && result.Severity.IsAnError) yield break;
                }
            }
        }

        private IEnumerable<ValidationOutcome> ValidateCollectionObjectsFirst(IEnumerable<T> collectionToValidate)
        {
            foreach (var objectToValidate in collectionToValidate)
            {
                foreach (var rule in ValidationRules)
                {
                    var result = rule.ValidateObject(objectToValidate);
                    yield return result;
                    if (rule.CausesValidationProcessToStop && result.Severity.IsAnError) break;
                }
            }
        }
    }
}
