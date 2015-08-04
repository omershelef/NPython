using System;

namespace NPython.Internals
{

    //TODO move "utils" from other classes to here.
    
    internal class PyUtils
    {
        private PythonAPI _api;

        internal PyUtils(PythonAPI api)
        {
            _api = api;
        }

        /// <summary>
        /// WARNING: Call this function only after aquiring the GIL!
        /// </summary>
        internal bool IsErrorOccured
        {
            get
            {
                var ptr = _api.PyErr_Occurred();
                if (ptr == IntPtr.Zero)
                {
                    return false;
                }

                return true;
            }
        }

        internal PyObject NewRef(IntPtr pyObject)
        {
            IntPtr gil = _api.PyGILState_Ensure();
            try
            {
                _api.Py_IncRef(pyObject);
                return new PyObject(_api, pyObject);
            }
            finally
            {
                _api.PyGILState_Release(gil);
            }
        }



        /// <summary>
        /// WARNING: Call this function only after aquiring the GIL!
        /// </summary>
        /// <param name="condition"></param>        
        internal void ThrowExcIf(Func<bool> condition)
        {
            if (condition())
            {
                var gil = _api.PyGILState_Ensure();

                try
                {
                    IntPtr excType = IntPtr.Zero;
                    IntPtr excValue = IntPtr.Zero;
                    IntPtr excTraceback = IntPtr.Zero;
                    _api.PyErr_Fetch(ref excType, ref excValue, ref excTraceback);
                    _api.PyErr_Clear();


                    var pyExcType = excType != IntPtr.Zero ? new PyObject(_api, excType) : null;
                    var pyExcValue = excValue != IntPtr.Zero ? new PyObject(_api, excValue) : null;
                    var pyExcTraceback = excTraceback != IntPtr.Zero ? new PyObject(_api, excTraceback) : null;
                    throw new PyException(pyExcType, pyExcValue, pyExcTraceback);
                }
                finally
                {
                    _api.PyGILState_Release(gil);
                }
            }
        }

    }
}
