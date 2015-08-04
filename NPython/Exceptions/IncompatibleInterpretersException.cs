using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NPython.Exceptions
{
    [Serializable]
    public class InCompatibleInterpretersException : Exception
    {
        private const string EXCEPTION_MESSAGE = "python interpreter cannot operate on PyObject belongs to other interpreter.";

        public InCompatibleInterpretersException()
            : base(EXCEPTION_MESSAGE)
        {
        }

        public InCompatibleInterpretersException(string message) : base(message)
        {
        }

        public InCompatibleInterpretersException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InCompatibleInterpretersException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
