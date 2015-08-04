using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NPython.Internals
{
    public class PythonAPI : IDisposable
    {
        private const string MAXUNICODE_NOT_SUPPORTED_EX =
            "cannot detect if USC2 or USC4. The maxunicode return from sys module is not supported. Found: {0}. Expected: {1} or {2}";


        public int Py_single_input = 256;
        public int Py_file_input = 257;
        public int Py_eval_input = 258;

        public bool is32Bit = IntPtr.Size == 4;


        /// <summary>
        /// Avoid classic Func<> and Action<> delegates because
        /// Marshal.GetDelegateForFunctionPointer cannot get 
        /// generic type delegates.
        /// </summary>        
        #region Delegates

        #region Action 

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void ActionIntPtr(IntPtr param);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void ActionString([MarshalAs(UnmanagedType.AnsiBStr)]string p);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void ActionRIntPtrRIntPtrRIntPtr(ref IntPtr p, ref IntPtr p2, ref IntPtr p3);

        #endregion

        #region Func

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr FuncIntPtr();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr FuncIntPtrIntPtr(IntPtr p);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr FuncIntPtrIntPtrIntPtr(IntPtr p, IntPtr p2);
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int FuncIntPtrIntPtrInt(IntPtr p, IntPtr p2);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int FuncIntPtrInt(IntPtr p);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int FuncIntPtrIntIntPtrInt(IntPtr p1, int p2, IntPtr p3);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate long FuncIntPtrLong(IntPtr p);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate double FuncIntPtrDouble(IntPtr p);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr FuncIntPtrStringIntPtr(IntPtr p, string p2);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int FuncIntPtrStringIntPtrInt(IntPtr p, string p2, IntPtr p3);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int FuncIntPtrStringInt(IntPtr p, string p2);




        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr FuncPIntPtrIntPtr(params IntPtr[] p);



        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)] 
        public delegate int FuncInt();       

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr FuncIntIntPtr(int p); 
 
        
                
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr FuncStringIntPtr(string p);


        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr FuncStringIntIntPtrIntPtrIntPtr(string p, int p2, IntPtr p3, IntPtr p4);

        #endregion

        #endregion


        #region APIFunctions

        public Action Py_Initialize;
        public FuncInt Py_IsInitialized;
        public Action Py_Finalize;
        public Action PyEval_InitThreads;      
        public FuncIntPtr PyGILState_Ensure;
        public ActionIntPtr PyGILState_Release;
        public FuncIntPtr PyThreadState_Get;

        public FuncStringIntIntPtrIntPtrIntPtr PyRun_String;

        public Action PyErr_Print;
        public FuncIntPtr PyErr_Occurred;
        public Action PyErr_Clear;
        public ActionRIntPtrRIntPtrRIntPtr PyErr_Fetch;

        public FuncStringIntPtr PyImport_AddModule;

        public FuncIntPtrIntPtr PyModule_GetDict;

        public ActionIntPtr Py_IncRef;
        public ActionIntPtr Py_DecRef;

        public FuncIntPtrStringInt PyObject_HasAttrString;
        public FuncIntPtrStringIntPtr PyObject_GetAttrString;
        public FuncIntPtrStringIntPtrInt PyObject_SetAttrString;
        public FuncIntPtrIntPtr PyObject_Str;
        public FuncIntPtrIntPtrInt PyObject_Compare;
        public FuncIntPtrInt PyCallable_Check;
        public FuncIntPtrInt PyObject_Length;
        public FuncIntPtrIntPtr PyObject_Dir;
        public FuncIntPtrIntPtr PyObject_Type;
        public FuncIntPtrIntPtrInt PyObject_IsInstance;

        public FuncIntPtrInt PyString_Check;
        public FuncStringIntPtr PyString_FromString;
        public FuncIntPtrIntPtr PyString_AsString;
        public FuncIntPtrInt PyUnicode_Check;
        public FuncIntPtrIntPtrIntPtr PyObject_CallObject;
        public FuncIntPtrIntPtr PyUnicode_AsUnicode;

        public FuncStringIntPtr PySys_GetObject;

        public FuncIntPtrInt PyInt_AsLong;
        public FuncIntPtrLong PyLong_AsLongLong;

        public FuncIntPtrDouble PyFloat_AsDouble;

        public FuncIntIntPtr PyTuple_New;
        public FuncIntPtrIntIntPtrInt PyTuple_SetItem;

        #endregion


        private UnmanagedLibrary _pythonLibrary;
        

        public PythonAPI(PythonVersion pyVer)
        {
            string dllName = pyVer.ToString().ToLower();
            _pythonLibrary = new UnmanagedLibrary(dllName);

            Py_Initialize = _pythonLibrary.GetUnmanagedFunction<Action>("Py_Initialize");
            Py_IsInitialized = _pythonLibrary.GetUnmanagedFunction<FuncInt>("Py_IsInitialized");            
            Py_Finalize = _pythonLibrary.GetUnmanagedFunction<Action>("Py_Finalize");
            PyEval_InitThreads = _pythonLibrary.GetUnmanagedFunction<Action>("PyEval_InitThreads");
            PyGILState_Ensure = _pythonLibrary.GetUnmanagedFunction<FuncIntPtr>("PyGILState_Ensure");
            PyGILState_Release = _pythonLibrary.GetUnmanagedFunction<ActionIntPtr>("PyGILState_Release");
            PyThreadState_Get = _pythonLibrary.GetUnmanagedFunction<FuncIntPtr>("PyThreadState_Get");

            PyRun_String = _pythonLibrary.GetUnmanagedFunction<FuncStringIntIntPtrIntPtrIntPtr>("PyRun_String");

            PyErr_Print = _pythonLibrary.GetUnmanagedFunction<Action>("PyErr_Print");
            PyErr_Occurred = _pythonLibrary.GetUnmanagedFunction<FuncIntPtr>("PyErr_Occurred");
            PyErr_Clear = _pythonLibrary.GetUnmanagedFunction<Action>("PyErr_Clear");
            PyErr_Fetch = _pythonLibrary.GetUnmanagedFunction<ActionRIntPtrRIntPtrRIntPtr>("PyErr_Fetch");

            PyImport_AddModule = _pythonLibrary.GetUnmanagedFunction<FuncStringIntPtr>("PyImport_AddModule");
            PyModule_GetDict = _pythonLibrary.GetUnmanagedFunction<FuncIntPtrIntPtr>("PyModule_GetDict");

            Py_IncRef = _pythonLibrary.GetUnmanagedFunction<ActionIntPtr>("Py_IncRef");
            Py_DecRef = _pythonLibrary.GetUnmanagedFunction<ActionIntPtr>("Py_DecRef");

            PyObject_HasAttrString = _pythonLibrary.GetUnmanagedFunction<FuncIntPtrStringInt>("PyObject_HasAttrString");
            PyObject_GetAttrString = _pythonLibrary.GetUnmanagedFunction<FuncIntPtrStringIntPtr>("PyObject_GetAttrString");
            PyObject_SetAttrString = _pythonLibrary.GetUnmanagedFunction<FuncIntPtrStringIntPtrInt>("PyObject_SetAttrString");
            PyObject_Str = _pythonLibrary.GetUnmanagedFunction<FuncIntPtrIntPtr>("PyObject_Str");
            PyObject_Compare = _pythonLibrary.GetUnmanagedFunction<FuncIntPtrIntPtrInt>("PyObject_Compare");
            PyCallable_Check = _pythonLibrary.GetUnmanagedFunction<FuncIntPtrInt>("PyCallable_Check");
            PyObject_CallObject = _pythonLibrary.GetUnmanagedFunction<FuncIntPtrIntPtrIntPtr>("PyObject_CallObject");
            PyObject_Length = _pythonLibrary.GetUnmanagedFunction<FuncIntPtrInt>("PyObject_Length");
            PyObject_Dir = _pythonLibrary.GetUnmanagedFunction<FuncIntPtrIntPtr>("PyObject_Dir");
            PyObject_Type = _pythonLibrary.GetUnmanagedFunction<FuncIntPtrIntPtr>("PyObject_Type");
            PyObject_IsInstance = _pythonLibrary.GetUnmanagedFunction<FuncIntPtrIntPtrInt>("PyObject_IsInstance");
            
            
            PyString_FromString = _pythonLibrary.GetUnmanagedFunction<FuncStringIntPtr>("PyString_FromString");
            PyString_AsString = _pythonLibrary.GetUnmanagedFunction<FuncIntPtrIntPtr>("PyString_AsString");
           
            PySys_GetObject = _pythonLibrary.GetUnmanagedFunction<FuncStringIntPtr>("PySys_GetObject");

            PyInt_AsLong = _pythonLibrary.GetUnmanagedFunction<FuncIntPtrInt>("PyInt_AsLong");
            PyLong_AsLongLong = _pythonLibrary.GetUnmanagedFunction<FuncIntPtrLong>("PyLong_AsLongLong");

            PyFloat_AsDouble = _pythonLibrary.GetUnmanagedFunction<FuncIntPtrDouble>("PyFloat_AsDouble");

            PyTuple_New = _pythonLibrary.GetUnmanagedFunction<FuncIntIntPtr>("PyTuple_New");
            PyTuple_SetItem = _pythonLibrary.GetUnmanagedFunction<FuncIntPtrIntIntPtrInt>("PyTuple_SetItem");

            PyType_Type = _pythonLibrary.GetGlobalVar("PyType_Type");
            PyBaseObject_Type = _pythonLibrary.GetGlobalVar("PyBaseObject_Type");
            PyString_Type = _pythonLibrary.GetGlobalVar("PyString_Type");
            PyUnicode_Type = _pythonLibrary.GetGlobalVar("PyUnicode_Type");
            PyInt_Type = _pythonLibrary.GetGlobalVar("PyInt_Type");
            PyLong_Type = _pythonLibrary.GetGlobalVar("PyLong_Type");
            PyBool_Type = _pythonLibrary.GetGlobalVar("PyBool_Type");
            PyFloat_Type = _pythonLibrary.GetGlobalVar("PyFloat_Type");
        }

        
        public void Init()
        {
            // TODO prevent double initilization.
            Py_Initialize();
            PyEval_InitThreads();

            string unicodeFuncName = IsUnicodeUCS2() ? "PyUnicodeUCS2_AsUnicode" : "PyUnicodeUCS4_AsUnicode";
            PyUnicode_AsUnicode = _pythonLibrary.GetUnmanagedFunction<FuncIntPtrIntPtr>(unicodeFuncName);            
        }

        internal IntPtr PyType_Type { get; set; }

        internal IntPtr PyBaseObject_Type { get; set; }

        internal IntPtr PyString_Type { get; private set; }

        internal IntPtr PyUnicode_Type { get; private set; }

        internal IntPtr PyInt_Type { get; private set; }

        internal IntPtr PyLong_Type { get; private set; }

        internal IntPtr PyBool_Type { get; private set; }

        internal IntPtr PyFloat_Type { get; private set; }

        

        internal bool IsUnicodeUCS2()
        {
            //TODO BAD DESIGN! interpreter is not initilized yet! maybe wrap the initlize func?            

            var gil = PyGILState_Ensure();
            //TODO release gil when errors.

            var maxunicodePtr = PySys_GetObject("maxunicode");

            if (maxunicodePtr == IntPtr.Zero)
            {
                //TODO implement
                throw new NotImplementedException();
            }

            var maxunicode = PyInt_AsLong(maxunicodePtr);

            if (maxunicode == -1)
            {
                //TODO implement.
                //TODO check PyErr_Occurred() to check if real error or just -1
                    
            }

            PyGILState_Release(gil);

            switch (maxunicode)
            {
                case 0xFFFF:
                    return true;
                case 0x10FFFF:
                    return false;
                default:
                    throw new FormatException(string.Format(MAXUNICODE_NOT_SUPPORTED_EX, maxunicode, 0xFFFF, 0x10FFFF));
            }
        }

        public void Dispose()
        {
            //TODO better design. Python object cannot be disposed in multi threaded enviroment. can lead to use of unvalid handle. 
            throw new NotImplementedException();
            _pythonLibrary.Dispose();
        }
    }
}
