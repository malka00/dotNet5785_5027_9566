using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BO
{

    /// <summary>
    /// Custom exception for issues related to XML file loading or creation.
    /// </summary>
    [Serializable]
    public class BlXMLFileLoadCreateException : Exception
    {
        public BlXMLFileLoadCreateException(string? message) : base(message) { }
        public BlXMLFileLoadCreateException(string message, Exception innerException)
                : base(message, innerException) { }
    }


    /// <summary>
    /// exception for item that does Not Exist
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
    public class BlWrongItemException : Exception
    {
        public BlWrongItemException(string? message) : base(message) { }
        public BlWrongItemException(string message, Exception innerException)
                    : base(message, innerException) { }
    }


    /// <summary>
    /// Exception for an object that cannot be deleted
    /// </summary>
    [Serializable]
    public class BlDeleteNotPossibleException : Exception
    {
        public BlDeleteNotPossibleException(string? message) : base(message) { }
        public BlDeleteNotPossibleException(string message, Exception innerException)
                : base(message, innerException) { }
    }


    /// <summary>
    /// Exception for an object that already exists
    /// </summary>
    [Serializable]
    public class BlAlreadyExistsException : Exception
    {
        public BlAlreadyExistsException(string? message) : base(message) { }
        public BlAlreadyExistsException(string message, Exception innerException)
                : base(message, innerException) { }
    }


    /// <summary>
    /// Exception for a value that cannot be null
    /// </summary>
    public class BlNullPropertyException : Exception
    {
        public BlNullPropertyException(string? message) : base(message) { }
    }


    /// <summary>
    /// Exception for incorrect input
    /// </summary>
    [Serializable]
    public class BlWrongInputException : Exception
    {
        public BlWrongInputException(string? message) : base(message) { }
        public BlWrongInputException(string message, Exception innerException)
                : base(message, innerException) { }
    }
}

