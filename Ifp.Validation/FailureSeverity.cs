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
        /// Failure severity <see cref="Information"/> indicates that there is a minor rule violation.
        /// This violation might be shown to the user but should never prevent the user from proceeding. 
        /// </summary>
        Information,

        /// <summary>
        /// Failure severity <see cref="Warning"/> indicates a rule violation that the user must confirm.
        /// An user should be able to either proceed or to cancel.
        /// </summary>
        Warning,

        /// <summary>
        /// Failure severity <see cref="Error"/> indicates a rule violation that the user must confirm. An user should not be able to proceed.
        /// </summary>
        Error,
    }
}
