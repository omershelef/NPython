using System;
using System.Data;
using System.Runtime.InteropServices.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using Microsoft.Win32.Fakes;
using NPython.Converters;
using NUnit.Core;
using Assert = NUnit.Framework.Assert;

namespace NPython.Tests
{
    [TestClass]
    public class StringConverterTests
    {
        [TestMethod]
        public void Convert_ReturnString_WhenAnsiString()
        {
            var py = Python.Instance();
            var pyStr = py.Eval("\"ansi string\"");
            var converter = new StringConverter();
            Assert.AreEqual("ansi string", converter.Convert(pyStr));
        }

        
        [TestMethod]
        public void Convert_ReturnString_WhenUnicodeString()
        {
            var py = Python.Instance();
            var pyStr = py.Eval("u\"unicode string\"");
            var converter = new StringConverter();
            Assert.AreEqual("unicode string", converter.Convert(pyStr));
        }


        [TestMethod]
        public void Convert_ThrowsException_WhenNotUnicodeOrString()
        {
            var py = Python.Instance();
            var pyInt = py.Eval("55");
            var converter = new StringConverter();            
            Assert.Throws<ConversionException>(() => converter.Convert(pyInt));
        }
    }
}
