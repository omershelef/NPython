using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NPython.Converters;
using Assert = NUnit.Framework.Assert;

namespace NPython.Tests
{
    [TestClass]
    public class BoolConverterTests
    {
        [TestMethod]
        public void Convert_ReturnTrue_WhenTrue()
        {
            var py = Python.Instance();
            var pyBool = py.Eval("True");
            var converter = new BoolConverter();
            Assert.AreEqual(true, converter.Convert(pyBool));
        }


        [TestMethod]
        public void Convert_ReturnFalse_WhenFalse()
        {
            var py = Python.Instance();
            var pyBool = py.Eval("False");
            var converter = new BoolConverter();
            Assert.AreEqual(false, converter.Convert(pyBool));
        }


        [TestMethod]
        public void Convert_ThrowsException_WhenNotBool()
        {
            var py = Python.Instance();
            var pyStr = py.Eval("\"omershelef\"");
            var converter = new BoolConverter();
            Assert.Throws<ConversionException>(() => converter.Convert(pyStr));
        }
    }
}
