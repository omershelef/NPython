using System;
using NPython.Internals;

namespace NPython
{
    public class PyTypes
    {
        private PythonAPI _api;
        private PyUtils _pyUtils;

        /* TODO implement this types
        None,
        Type,
        Bool,
        Int,
        Long,
        Float,
        String,
        Unicode,
        Tuple,
        List,
        Dict,
        Function,
        Class,
        Instance,
        Method,
        Module,
        Traceback,
         */

        internal PyTypes(PythonAPI api)
        {
            _api = api;
            _pyUtils = new PyUtils(api);
        }

        public PyObject Str
        {
            get { return _pyUtils.NewRef(_api.PyString_Type); }
        }


        public PyObject Unicode
        {
            get { return _pyUtils.NewRef(_api.PyString_Type); }
        }

        public PyObject Int
        {
            get { return _pyUtils.NewRef(_api.PyInt_Type); }
        }

        public PyObject Long
        {
            get { return _pyUtils.NewRef(_api.PyLong_Type); }
        }

        public PyObject Bool
        {
            get { return _pyUtils.NewRef(_api.PyBool_Type); }
        }

        public PyObject Object
        {
            get { return _pyUtils.NewRef(_api.PyBaseObject_Type); }
        }


    }
}