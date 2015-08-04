using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPython.Internals;

namespace NPython.Converters
{
    public class LongConverter : IConverter<long>
    {
        public long Convert(PyObject pyObject)
        {
            var pyUtils = new PyUtils(pyObject.Api);

            var gil = pyObject.BeginSafeMethod();
            try
            {                
                if (pyObject.IsInstance(pyUtils.NewRef(pyObject.Api.PyInt_Type)))
                {
                    var result = pyObject.Api.PyInt_AsLong(pyObject.PyObjectPtr);
                    pyUtils.ThrowExcIf(() => result == -1 && pyUtils.IsErrorOccured);
                    return result;
                }
                else if (pyObject.IsInstance(pyUtils.NewRef(pyObject.Api.PyLong_Type)))
                {
                    var result = pyObject.Api.PyLong_AsLongLong(pyObject.PyObjectPtr);
                    pyUtils.ThrowExcIf(() => result == -1 && pyUtils.IsErrorOccured);
                    return result;
                }
                
                throw new ConversionException(pyObject, typeof (long));                
            }
            finally
            {
                pyObject.EndSafeMethod(gil);
            }
        }
    }
}
