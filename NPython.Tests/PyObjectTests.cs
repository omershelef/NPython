using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32.Fakes;
using NPython.Exceptions;
using Assert = NUnit.Framework.Assert;

namespace NPython.Tests
{
    [TestClass]
    public class PyObjectTests
    {
        [TestMethod]
        public void IsCallable_ReturnTrue_WhenCallable()
        {
            var py = Python.Instance();
            var func = py.Eval("abs");
            Assert.IsTrue(func.IsCallable);
        }

        [TestMethod]
        public void IsCallable_ReturnFalse_WhenNotCallable()
        {
            var py = Python.Instance();
            var pyInt = py.Eval("55");
            Assert.AreEqual(false, pyInt.IsCallable);            
        }

        [TestMethod]
        public void Equals_ReturnTrue_WhenEquals()
        {
            var py = Python.Instance();
            var pyInt = py.Eval("55");
            var pyInt2 = py.Eval("55");
            Assert.AreEqual(pyInt, pyInt2);
        }

        [TestMethod]
        public void Equals_ReturnFalse_WhenNotEquals()
        {
            var py = Python.Instance();
            var pyInt = py.Eval("55");
            var pyInt2 = py.Eval("66");
            Assert.AreNotEqual(pyInt, pyInt2);
        }


        [TestMethod]
        public void Type_ReturnIntType_WhenInt()
        {
            var py = Python.Instance();
            var pyIntType = py.Eval("55").Type;            
            Assert.AreEqual(py.Types.Int, pyIntType);
        }





        [TestMethod]
        public void HasAttr_ReturnTrue_WhenAttributeExist()
        {
            var py = Python.Instance();
            var pyFunc = py.Eval("abs");
            Assert.IsTrue(pyFunc.HasAttr("__call__"));

        }

        [TestMethod]
        public void HasAttr_ReturnFalse_WhenAttributeNotExist()
        {
            var py = Python.Instance();
            var num = py.Eval("5");
            Assert.IsFalse(num.HasAttr("fakeAttr"));            
        }


        [TestMethod]
        public void GetAttr_SanityCheck()
        {
            var py = Python.Instance();
            string code = @"                           
class A():
    def myfunc():
         pass
";

            py.Exec(code);
            var pyClass = py.Eval("A");
            var myFunc = py.Eval("A.myfunc");            
            Assert.AreEqual(myFunc, pyClass.GetAttr("myfunc"));
        }

        [TestMethod]
        public void GetAttr_ThrowsException_WhenAttributeNotExist()
        {
            var py = Python.Instance();
            var num = py.Eval("5");
            Assert.Throws<PyException>(() => num.GetAttr("fakeAttr"));
        }

        [TestMethod]
        public void SetAttr_SanityCheck()
        {
            var py = Python.Instance();
            string code = @"                           
class A():
    myattr = 55
";
            py.Exec(code);
            var pyClass = py.Eval("A");
            pyClass.SetAttr("myattr", py.Eval("88"));
            Assert.AreEqual(py.Eval("88"), py.Eval("A.myattr"));
        }


        [TestMethod]
        public void IsInstance_ReturnTrue_WhenSameType()
        {
            var py = Python.Instance();
            var str = py.Eval("\"mytest\"");
            Assert.IsTrue(str.IsInstance(py.Types.Str));
        }

        [TestMethod]
        public void IsInstance_ReturnTrue_WhenSubType()
        {
            var py = Python.Instance();
            var str = py.Eval("\"mytest\"");
            Assert.IsTrue(str.IsInstance(py.Eval("basestring")));
        }


        [TestMethod]
        public void IsInstance_ReturnFalse_WhenDifferentType()
        {
            var py = Python.Instance();
            var num = py.Eval("5");
            Assert.IsFalse(num.IsInstance(py.Types.Str));
        }
       

        [TestMethod]
        public void IsInstance_ReturnTrue_WhenSubclass()
        {
            var py = Python.Instance();
            string code = @"                           
class A():
    pass

class B(A):
    pass";

            py.Exec(code);
            var i = py.Eval("B()");
            
            Assert.IsTrue(i.IsInstance(py.Eval("A")));
        }


        [TestMethod]
        public void IsInstance_ThrowsException_WhenUsingDifferentVersions()
        {
            var py26 = Python.Instance(PythonVersion.Python26);
            var py27 = Python.Instance(PythonVersion.Python27);
            var num = py26.Eval("5");
            var num2 = py27.Eval("5");
            Assert.Throws<InCompatibleInterpretersException>(() => num.IsInstance(num2));
        }


        [TestMethod]
        public void Call_SanityCheck()
        {
            var py = Python.Instance();
            var floatNum = py.Eval("-5");
            var absFunc = py.Eval("abs");
            var result = absFunc.Call(floatNum);
            Assert.AreEqual("5", result.ToString());
        }


        [TestMethod]
        public void Len_SanityCheck()
        {
            var py = Python.Instance();
            var pyStr = py.Eval("\"mystr\"");            
            Assert.AreEqual(5, pyStr.Len());            
        }


        [TestMethod]
        public void Len_ThrowsException_WhenNotSupported()
        {
            var py = Python.Instance();
            var pyInt = py.Eval("55");
            Assert.Throws<PyException>(() => pyInt.Len());
        }
    }
}
