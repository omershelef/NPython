using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPython.Internals;

namespace NPython.Converters
{
    public class DoubleConverter : IConverter<double>
    {
        public double Convert(PyObject pyObject)
        {
            var pyUtils = new PyUtils(pyObject.Api);

            IntPtr gil = pyObject.BeginSafeMethod();
            try
            {
                if (pyObject.IsInstance(pyUtils.NewRef(pyObject.Api.PyFloat_Type)))
                {
                    var result = pyObject.Api.PyFloat_AsDouble(pyObject.PyObjectPtr);
                    pyUtils.ThrowExcIf(() => result == -1.0 && pyUtils.IsErrorOccured);
                    return result;
                }

                throw new ConversionException(pyObject, typeof (double));             
            }
            finally
            {
                pyObject.EndSafeMethod(gil);
            }
        }
    }
}
