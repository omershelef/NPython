using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPython
{
    internal interface IConverter { }

    internal interface IConverter<TReturnType> : IConverter
    {
        TReturnType Convert(PyObject pyObject);
    }
}
