using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NPython.Exceptions;
using NPython.Internals;

namespace NPython
{
    public class PyObject : IDisposable
    {
        #region Constants

        private const string PYOBJECT_DISPOSED_EX = "PyObject has been disposed and can't be used anymore";

        #endregion

        #region Private members

        private readonly PyUtils _pyUtils;
        private bool _isDisposed;

        #endregion

        internal PyObject(PythonAPI api, IntPtr pyObject)
        {
            if (pyObject == IntPtr.Zero)
            {
                throw new ArgumentNullException("pyObject");
            }

            Api = api;
            _pyUtils = new PyUtils(api);
            PyObjectPtr = pyObject;
        }

        internal IntPtr PyObjectPtr { get; private set; }

        internal PythonAPI Api { get; private set; }


        /// <summary>
        ///     Detect if the PyObject instance is callable.
        /// </summary>
        public bool IsCallable
        {
            get
            {
                IntPtr gil = BeginSafeMethod();
                try
                {
                    int result = Api.PyCallable_Check(PyObjectPtr);
                    return result != 0;
                }
                finally
                {
                    EndSafeMethod(gil);
                }
            }
        }



        /// <summary>
        ///     Get the python type of the instance,
        ///     Equivalent to the python "type" function.
        /// </summary>
        public PyObject Type
        {
            get
            {
                IntPtr gil = BeginSafeMethod();
                try
                {
                    IntPtr result = Api.PyObject_Type(PyObjectPtr);
                    _pyUtils.ThrowExcIf(() => result == IntPtr.Zero);
                    return new PyObject(Api, result);
                }
                finally
                {
                    EndSafeMethod(gil);
                }
            }
        }


        /// <summary>        
        ///     Release PyObject unmanaged memory. this function will be call anyway by the GC
        ///     when the pyobject is not longer in use.
        /// </summary>
        public void Dispose()
        {
            IntPtr gil = Api.PyGILState_Ensure();

            if (!_isDisposed)
            {
                Api.Py_DecRef(PyObjectPtr);
                _isDisposed = true;
                GC.SuppressFinalize(this);
            }

            Api.PyGILState_Release(gil);
        }

        public PyObject Call(params PyObject[] parameters)
        {
            //TODO Assert memory management is correct! (Tuple and function call)
            IntPtr gil = BeginSafeMethod();
            IntPtr tuple = Api.PyTuple_New(parameters.Length);
            if (tuple == IntPtr.Zero)
            {
                throw new NotImplementedException();
            }

            // TODO move tuple management to the auto converter if possible.
            for (int i = 0; i < parameters.Length; i++)
            {
                IntPtr ptr = parameters[i].PyObjectPtr;
                Api.Py_IncRef(ptr);
                int r = Api.PyTuple_SetItem(tuple, i, ptr);
                if (r != 0)
                {
                    throw new NotImplementedException();
                }
            }

            IntPtr result = Api.PyObject_CallObject(PyObjectPtr, tuple);
            if (result == IntPtr.Zero)
            {
                throw new NotImplementedException();
            }

            //TODO decref tuple after call?
            EndSafeMethod(gil);
            return new PyObject(Api, result);
        }


        /// <summary>
        ///     Detect if attribute exists.
        ///     Equivalent to python hasattr function.
        /// </summary>
        /// <param name="attr">the attr to check if exists</param>
        /// <returns>is pyObject has attr</returns>
        public bool HasAttr(string attr)
        {
            IntPtr gil = BeginSafeMethod();
            try
            {
                int result = Api.PyObject_HasAttrString(PyObjectPtr, attr);
                return result != 0;
            }
            finally
            {
                EndSafeMethod(gil);
            }
        }

        /// <summary>
        ///     Get the attribute for the specified attr name
        ///     Equivalent to python getattr function.
        /// </summary>
        /// <param name="attr">The attribute to get</param>
        /// <returns>The attribute</returns>
        public PyObject GetAttr(string attr)
        {
            IntPtr gil = BeginSafeMethod();
            try
            {
                IntPtr result = Api.PyObject_GetAttrString(PyObjectPtr, attr);
                _pyUtils.ThrowExcIf(() => result == IntPtr.Zero);
                return new PyObject(Api, result);
            }
            finally
            {
                EndSafeMethod(gil);
            }
        }


        /// <summary>
        ///     Set attribute of PyObject instance.
        ///     Equivalent to python setattr function.
        /// </summary>
        /// <param name="attr">attribute name.</param>
        /// <param name="value">the attribute value.</param>
        public void SetAttr(string attr, PyObject value)
        {
            IntPtr gil = BeginSafeMethod();
            try
            {
                int result = Api.PyObject_SetAttrString(PyObjectPtr, attr, value.PyObjectPtr);
                _pyUtils.ThrowExcIf(() => result == -1);
            }
            finally
            {
                EndSafeMethod(gil);
            }
        }


        //TODO unittest
        public void DelAttr(string attr)
        {
            //TODO implement del. PyObject_DelAttr not working.
            throw new NotImplementedException();
        }

        /// <summary>
        ///     returns the length of the PyObject instance.
        ///     Equivalent to the result of python "len" function.
        /// </summary>
        /// <returns>The length of the PyObject.</returns>
        public int Len()
        {
            IntPtr gil = BeginSafeMethod();
            try
            {
                int result = Api.PyObject_Length(PyObjectPtr);
                _pyUtils.ThrowExcIf(() => result == -1);
                return result;
            }
            finally
            {
                EndSafeMethod(gil);
            }
        }


        //TODO implement indexers?

        //TODO unittest
        public IEnumerable<string> Dir()
        {
            IntPtr gil = BeginSafeMethod();

            IntPtr result = Api.PyObject_Dir(PyObjectPtr);

            if (result == IntPtr.Zero)
            {
                //TODO implement
                throw new NotImplementedException();
            }

            EndSafeMethod(gil);

            //TODO implement. decref result
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Check if the instance is instance of the specified pyObject.
        ///     Equivalent to the python "isinstance" function.
        /// </summary>
        /// <param name="pyObject">isinstance of this param.</param>
        /// <returns>isinstance result</returns>
        public bool IsInstance(PyObject pyObject)
        {
            if (pyObject.Api != Api)
            {
                throw new InCompatibleInterpretersException();
            }

            IntPtr gil = BeginSafeMethod();
            try
            {
                int result = Api.PyObject_IsInstance(PyObjectPtr, pyObject.PyObjectPtr);
                _pyUtils.ThrowExcIf(() => result == -1);
                return result != 0;
            }
            finally
            {
                EndSafeMethod(gil);
            }
        }


        /// <summary>
        ///     Checks for equality.
        ///     Equivalent to "==" python operator.
        /// </summary>
        /// <param name="obj">obj to checks equality with.</param>
        /// <returns>is equal.</returns>
        public override bool Equals(object obj)
        {
            // TODO try auto convert if obj is not PyObject?
            if (!(obj is PyObject))
            {
                return false;
            }

            var pyObject = (PyObject) obj;
            if (pyObject.Api != Api)
            {
                throw new InCompatibleInterpretersException();
            }

            IntPtr gil = BeginSafeMethod();
            try
            {
                int result = Api.PyObject_Compare(PyObjectPtr, pyObject.PyObjectPtr);
                _pyUtils.ThrowExcIf(() => _pyUtils.IsErrorOccured);
                return result == 0;
            }
            finally
            {
                EndSafeMethod(gil);
            }
        }

        /// <summary>
        ///     Return instance as string.
        ///     Equivalent to the python "__str__" function.
        /// </summary>
        /// <returns>instance as string.</returns>
        public override string ToString()
        {
            IntPtr gil = BeginSafeMethod();
            try
            {
                IntPtr pystring = Api.PyObject_Str(PyObjectPtr);
                _pyUtils.ThrowExcIf(() => pystring == IntPtr.Zero);
                try
                {
                    IntPtr strPtr = Api.PyString_AsString(pystring);
                    _pyUtils.ThrowExcIf(() => strPtr == IntPtr.Zero);
                    string str = Marshal.PtrToStringAnsi(strPtr);
                    return str;
                }
                finally
                {
                    Api.Py_DecRef(pystring);
                }
            }
            finally
            {
                EndSafeMethod(gil);
            }
        }


        /// <summary>
        ///     Release PyObject unmanaged memory.
        /// </summary>
        ~PyObject()
        {
            Dispose();
        }

        internal IntPtr BeginSafeMethod()
        {
            IntPtr gil = Api.PyGILState_Ensure();
            if (_isDisposed)
            {
                throw new ObjectDisposedException(PYOBJECT_DISPOSED_EX);
            }
            return gil;
        }

        internal void EndSafeMethod(IntPtr gil)
        {
            Api.PyGILState_Release(gil);
        }
    }
}