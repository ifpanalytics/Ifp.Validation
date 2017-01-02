using System;
using System.Collections.Generic;
using System.Linq;

namespace Ifp.Validation
{
    /// <summary>
    /// An enumeration of predefined failure severities. 
    /// </summary>
    public enum FailureSeverity
    {
        /// <summary>
        /// Failure severity 'information' indicates that there is a minor rule violation. This violation might be shown to the user but should never prevent the user from proceeding. 
        /// </summary>
        Information,

        /// <summary>
        /// Failure severity 'warning' indicates a rule violation that the user must confirm. An user should be able to proceed after he confirmed that he read the warning.
        /// </summary>
        Warning,

        /// <summary>
        /// Failure severity 'error' indicates a rule violation that the user must confirm. An user should not be able to proceed.
        /// </summary>
        Error,
    }
}
