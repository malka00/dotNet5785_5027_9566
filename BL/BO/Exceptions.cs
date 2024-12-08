using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
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
}
