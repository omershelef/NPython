using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPython;

namespace EvalAndExec.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var py = Python.Instance();

            // print to the console.
            py.Exec("print(\"Hello world!\")");

            // Declare variable
            py.Exec("myvar = 5");

            // evaluate condition and auto convert to bool.
            bool result = py.Eval<bool>("myvar > 2");

            // multi line code
            string code = @"
myvar = 1
myvar2 = 5
print(""result: "" + str(myvar + myvar2))
";
            py.Exec(code);
        }
    }
}
