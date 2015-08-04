using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NPython.Converters;
using NPython.Internals;

namespace NPython.Console
{
    class Program
    {
        static unsafe void Main(string[] args)
        {
            var a = Foo();
        }

        public static int Foo()
        {
            try
            {
                return 5;
            }
            finally
            {
                System.Console.WriteLine("Hello world!");
            }
        }
    }
}
