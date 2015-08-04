using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NPython.Converters;
using Assert = NUnit.Framework.Assert;

namespace NPython.Tests.Converters
{
    [TestClass]
    public class FloatConverterTests
    {
        [TestMethod]
        public void Convert_SanityCheck()
        {
            var py = Python.Instance();
            var pyFloat = py.Eval("123.456789");
            var converter = new DoubleConverter();
            Assert.AreEqual(123.456789, converter.Convert(pyFloat));
        }


        [TestMethod]
        public void Convert_ThrowsException_WhenNotDouble()
        {
            var py = Python.Instance();
            var pyStr = py.Eval("\"omershelef\"");
            var converter = new DoubleConverter();
            Assert.Throws<ConversionException>(() => converter.Convert(pyStr));
        }
    }
}
