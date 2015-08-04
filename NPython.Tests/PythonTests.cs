using System;
using System.Data;
using System.Runtime.InteropServices.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using Microsoft.Win32.Fakes;
using NUnit.Core;
using Assert = NUnit.Framework.Assert;
using NPython.Internals;
using System.Diagnostics;
using System.Threading;

namespace NPython.Tests
{
    [TestClass]
    public class PythonTests
    {
        [TestMethod]
        public void GetDefaultPython_ThrowsException_WhenNoRegistryKeys()
        {
            using(ShimsContext.Create()) {                
                ShimRegistry.GetValueStringStringObject = (s, s1, arg3) => { return null; };
                var python = new PrivateType(typeof (Python));
                Assert.Throws<VersionNotFoundException>(() => python.InvokeStatic("GetDefaultPython"));
            }
        }


        [TestMethod]
        public void GetDefaultPython_ThrowsException_WhenNoPythonPathInTheRegistryKey()
        {
            using (ShimsContext.Create())
            {
                ShimRegistry.GetValueStringStringObject = (s, s1, arg3) => { return @"C:\SomePath"; };
                var python = new PrivateType(typeof(Python));
                Assert.Throws<VersionNotFoundException>(() => python.InvokeStatic("GetDefaultPython"));
            }
        }


        [TestMethod]
        public void GetDefaultPython_ThrowsException_WhenPyVerNotInPyVersionEnum()
        {
            using (ShimsContext.Create())
            {
                ShimRegistry.GetValueStringStringObject = (s, s1, arg3) => { return @"C:\Python13"; };
                var python = new PrivateType(typeof(Python));
                Assert.Throws<VersionNotFoundException>(() => python.InvokeStatic("GetDefaultPython"));
            }
        }


        [TestMethod]
        public void GetDefaultPython_ReturnsValidPythonVersion_WhenPythonPathFromRegistryValid()
        {
            using (ShimsContext.Create())
            {
                ShimRegistry.GetValueStringStringObject = (s, s1, arg3) => { return @"C:\Python27\Python.exe"; };
                var python = new PrivateType(typeof(Python));
                var result = (PythonVersion)python.InvokeStatic("GetDefaultPython");
                Assert.AreEqual(PythonVersion.Python27, result);                
            }
        }


        [TestMethod]
        public void Eval_SanityCheck()
        {
            var result = Python.Instance().Eval("\"mytest\" + \" test\"");
            Assert.AreEqual("mytest test", result.ToString());                
        }


        [TestMethod]
        public void Exec_SaveState_WhenAllocatingGlobalVars()
        {
            var py = Python.Instance();
            py.Exec("a = \"mytest\"");
            py.Exec("b = \" test\"");
            Assert.AreEqual("mytest test", py.Eval("a + b").ToString());
        }


        [TestMethod]
        // Python26 and python27 are required
        public void Exec_GlobalsAreDifferent_WhenUsingDifferentPythonVersions()
        {
            var py26 = Python.Instance(PythonVersion.Python26);
            var py27 = Python.Instance(PythonVersion.Python27);
            py26.Exec("a = \"mytest\"");
            py26.Exec("b = \" test\"");
            py27.Exec("a = \"test\"");
            py27.Exec("b = \" mytest\"");

            Assert.AreEqual("mytest test", py26.Eval("a + b").ToString());
            Assert.AreEqual("test mytest", py27.Eval("a + b").ToString());
        }



        [TestMethod]
        public void Eval_ThrowsException_WhenRaisingException()
        {
            var py = Python.Instance();
            Assert.Throws<PyException>(() => Python.Instance().Eval("raise Exception()"));
        }


        [TestMethod]
        public void Exec_ThrowsException_WhenRaisingException()
        {
            var py = Python.Instance();
            Assert.Throws<PyException>(() => Python.Instance().Exec("raise Exception()"));
        }


        [TestMethod]
        public void Exec_StillWorks_WhenPreviousExecFailed()
        {
            var py = Python.Instance();
            try
            {
                py.Exec("raise Exception()");
            }
            catch (PyException ex) { }

            py.Exec("myVar = 5");
            Assert.AreEqual("5", py.Eval("myVar").ToString());
        }


        [TestMethod]
        public void Test_GIL_Lock()
        {
            var py = new PrivateObject(Python.Instance());
            var api = (PythonAPI)py.GetProperty("Api");
            var gil = api.PyGILState_Ensure();            

            var sw = new Stopwatch();
            sw.Start();
            var t = new Thread(() => {
                Thread.Sleep(2000);
                api.PyGILState_Release(gil);    
            });
            t.Start();
            var anotherGil = api.PyGILState_Ensure();
            Assert.GreaterOrEqual(sw.ElapsedMilliseconds, 2000);
            api.PyGILState_Release(anotherGil);
        }

    }
}
