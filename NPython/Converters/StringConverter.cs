using System;
using System.Runtime.InteropServices;
using NPython.Internals;

namespace NPython.Converters
{
    public class StringConverter : IConverter<string>
    {
        public string Convert(PyObject pyObject)
        {
            var pyUtils = new PyUtils(pyObject.Api);

            var gil = pyObject.BeginSafeMethod();
            try
            {
                if (pyObject.IsInstance(pyUtils.NewRef(pyObject.Api.PyString_Type)))
                {
                    var strPtr = pyObject.Api.PyString_AsString(pyObject.PyObjectPtr);
                    pyUtils.ThrowExcIf(() => strPtr == IntPtr.Zero);                    
                    return Marshal.PtrToStringAnsi(strPtr);
                }
                else if (pyObject.IsInstance(pyUtils.NewRef(pyObject.Api.PyUnicode_Type)))
                {
                    var strPtr = pyObject.Api.PyUnicode_AsUnicode(pyObject.PyObjectPtr);
                    pyUtils.ThrowExcIf(() => strPtr == IntPtr.Zero);                    
                    return Marshal.PtrToStringUni(strPtr);
                }
                
                throw new ConversionException(pyObject, typeof (string));                
            }
            finally
            {
                pyObject.EndSafeMethod(gil);                
            }
        }
    }
}