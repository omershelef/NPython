using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using NPython.Internals;

namespace NPython
{
    public class Python
    {
        #region Constants

        private const string PY_REG_CURRENT_USER =
            @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\Python.exe";

        private const string PY_REG_LOCAL_MACHINE =
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\Python.exe";

        private const string PY_VER_RE = @"python[0-9]+";

        private const string VER_NOT_FOUND_EX = "Couldn't detect py version based on the registry values.";

        #endregion

        #region Static members

        private static readonly object _singletonLock = new object();

        private static readonly Dictionary<PythonVersion, Python> _pythonInstances =
            new Dictionary<PythonVersion, Python>();

        #endregion

        private PyUtils _pyUtils;

        #region Singleton

        public static Python Instance()
        {
            return Instance(GetDefaultPython());
        }

        public static Python Instance(PythonVersion pyVer)
        {
            lock (_singletonLock)
            {
                Python instance;
                if (_pythonInstances.TryGetValue(pyVer, out instance))
                {
                    return instance;
                }

                instance = new Python(pyVer);
                _pythonInstances.Add(pyVer, instance);
                return instance;
            }
        }

        #endregion

        private Python(PythonVersion pyVer)
        {
            Version = pyVer;
            Api = new PythonAPI(pyVer);
            Api.Init();
            Types = new PyTypes(Api);
            _pyUtils = new PyUtils(Api);
        }

        internal PythonAPI Api { get; private set; }


        public PythonVersion Version { get; private set; }
        public PyTypes Types { get; private set; }



        
        public void Exec(string code)
        {
            IntPtr gil = Api.PyGILState_Ensure();
            try
            {
                IntPtr globals = Api.PyModule_GetDict(Api.PyImport_AddModule("__main__"));
                _pyUtils.ThrowExcIf(() => globals == IntPtr.Zero);

                IntPtr pyObject = Api.PyRun_String(code, Api.Py_file_input, globals, globals);
                _pyUtils.ThrowExcIf(() => pyObject == IntPtr.Zero);

                Api.Py_DecRef(pyObject);
            }
            finally
            {
                Api.PyGILState_Release(gil);
            }
        }


        public PyObject Eval(string code)
        {
            IntPtr gil = Api.PyGILState_Ensure();
            try
            {
                IntPtr globals = Api.PyModule_GetDict(Api.PyImport_AddModule("__main__"));
                _pyUtils.ThrowExcIf(() => globals == IntPtr.Zero);

                IntPtr pyObject = Api.PyRun_String(code, Api.Py_eval_input, globals, globals);
                _pyUtils.ThrowExcIf(() => pyObject == IntPtr.Zero);                                     

                return new PyObject(Api, pyObject);
            }
            finally
            {
                Api.PyGILState_Release(gil);
            }
        }

        public TReturnValue Eval<TReturnValue>(string code)
        {
            return AutoConverter.Convert<TReturnValue>(Eval(code));            
        }


        private static PythonVersion GetDefaultPython()
        {
            var pythonPath = (string) Registry.GetValue(PY_REG_CURRENT_USER, null, null);
            if (pythonPath != null)
            {
                return GetPyVer(pythonPath);
            }
            pythonPath = (string) Registry.GetValue(PY_REG_LOCAL_MACHINE, null, null);
            if (pythonPath != null)
            {
                return GetPyVer(pythonPath);
            }

            throw new VersionNotFoundException(VER_NOT_FOUND_EX);
        }


        private static PythonVersion GetPyVer(string pythonPath)
        {
            Match match = Regex.Match(pythonPath, PY_VER_RE, RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                throw new VersionNotFoundException(VER_NOT_FOUND_EX);
            }

            PythonVersion ver;
            if (!Enum.TryParse(match.Value, true, out ver))
            {
                throw new VersionNotFoundException(VER_NOT_FOUND_EX);
            }

            return ver;
        }
    }
}