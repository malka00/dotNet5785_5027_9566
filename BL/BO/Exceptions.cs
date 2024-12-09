using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
   
        /// <summary>
        /// exception foritem  Does Not Exist
        /// </summary>    
        [Serializable]
    public class BlDoesNotExistException : Exception
    {
        public BlDoesNotExistException(string? message) : base(message) { }
        public BlDoesNotExistException(string message, Exception innerException)
                    : base(message, innerException) { }
    }
    /// <summary>
    /// exception for not possible item (like Id impossible)
    /// </summary>
    [Serializable]
    public class BlWrongItemtException : Exception
    {
        public BlWrongItemtException(string? message) : base(message) { }
        public BlWrongItemtException(string message, Exception innerException)
                    : base(message, innerException) { }
    }

    [Serializable]
    public class DeleteNotPossibleException : Exception
    {
        public DeleteNotPossibleException(string? message) : base(message)
        {
        }
        
        
        public DeleteNotPossibleException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
    [Serializable]
    public class BlAlreadyExistsException : Exception
    {
        public BlAlreadyExistsException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
