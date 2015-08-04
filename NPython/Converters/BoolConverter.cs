using System;
using NPython.Internals;

namespace NPython.Converters
{
    public class BoolConverter : IConverter<bool>
    {
        public bool Convert(PyObject pyObject)
        {
            var pyUtils = new PyUtils(pyObject.Api);

            var gil = pyObject.BeginSafeMethod();
            try
            {
                if (pyObject.IsInstance(pyUtils.NewRef(pyObject.Api.PyInt_Type)))
                {
                    int intVal = pyObject.Api.PyInt_AsLong(pyObject.PyObjectPtr);
                    pyUtils.ThrowExcIf(() => intVal == -1 && pyUtils.IsErrorOccured);
                    return intVal != 0;
                }

                throw new ConversionException(pyObject, typeof (bool));
            }
            finally
            {
                pyObject.EndSafeMethod(gil);
            }
        }
    }
}