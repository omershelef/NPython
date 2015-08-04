using System;

namespace NPython
{
    /// <summary>
    /// when Conversion is not possible from specified pyObject to expected .Net Type.
    /// </summary>
    [Serializable]
    public class ConversionException : Exception
    {
        private const string PY_CONVERSATION_EX = "Cannot convert Python type {0} to {1}";


        /// <summary>
        /// when Conversion is not possible from specified pyObject to expected .Net Type.
        /// </summary>
        /// <param name="found">The object trying to convert</param>
        /// <param name="expected">The type trying to convert to</param>
        internal ConversionException(PyObject found, Type expected) :
            this(string.Format(PY_CONVERSATION_EX, found.Type, expected.Name))
        { }


        /// <summary>
        /// when Conversion is not possible from specified pyObject to expected .Net Type.
        /// </summary>
        /// <param name="found">The object trying to convert</param>
        /// <param name="expected">The type trying to convert to</param>
        /// <param name="inner">The inner exception</param>
        internal ConversionException(PyObject found, Type expected , Exception inner)
            : this(string.Format(PY_CONVERSATION_EX, found.Type, expected.Name), inner)
        { }


        private ConversionException(string message)
            : base(message)
        { }


        private ConversionException(string message, Exception inner)
            : base(message, inner)
        { }
    }


}