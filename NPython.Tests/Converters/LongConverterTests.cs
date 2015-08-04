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
    public class LongConverterTests
    {
        [TestMethod]
        public void Convert_ReturnLong_WhenInt()
        {
            var py = Python.Instance();
            var pyInt = py.Eval("55");
            var converter = new LongConverter();
            NUnit.Framework.Assert.AreEqual(55, converter.Convert(pyInt));
        }


        [TestMethod]
        public void Convert_ReturnLong_WhenLong()
        {
            var py = Python.Instance();
            var pyLong = py.Eval("12345678910");
            var converter = new LongConverter();
            Assert.AreEqual(12345678910L, converter.Convert(pyLong));
        }


        [TestMethod]
        public void Convert_ThrowsException_WhenNotInteger()
        {
            var py = Python.Instance();
            var pyStr = py.Eval("\"omershelef\"");
            var converter = new LongConverter();
            Assert.Throws<ConversionException>(() => converter.Convert(pyStr));
        }
    }
}
