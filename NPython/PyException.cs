using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NPython
{

    [Serializable]
    public class PyException : Exception
    {
        private string _pyTraceback;

        //TODO excValue may be null.

        public PyException(PyObject excType, PyObject excValue, PyObject excTraceback) 
            : this(excType, excValue, excTraceback, null) { }

        public PyException(PyObject excType, PyObject excValue, PyObject excTraceback, Exception inner)
            : base(excValue.ToString(), inner)
        {
            ExcType = excType;
            ExcValue = excValue;
            ExcTraceback = excTraceback;

            if (ExcTraceback != null)
            {
                //TODO unittest stacktrace creation?
                var tracebackModule = excTraceback.Api.PyImport_AddModule("traceback");
                var formatExcFunc = new PyObject(excTraceback.Api, tracebackModule).GetAttr("format_exception");
                var tbList = formatExcFunc.Call(excType, excValue, excTraceback);

                //TODO join tblist with \n
                _pyTraceback = "NotImplemented";
            }
        }

        public PyObject ExcType { get; private set; }

        public PyObject ExcValue { get; private set; }

        public PyObject ExcTraceback { get; private set; }


        public override string StackTrace
        {
            get { return _pyTraceback + base.StackTrace; }
        }

        //TODO Override ToString. the tostring call to GetStackTrace instead StackTrace.
    }
}
