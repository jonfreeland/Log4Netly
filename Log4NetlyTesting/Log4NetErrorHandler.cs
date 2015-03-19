using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Core;

namespace Log4NetlyTesting {
    public class Log4NetErrorHandler : IErrorHandler {
        public void Error(string message, Exception e, ErrorCode errorCode) {
            Console.WriteLine(message);
            Console.WriteLine("\t" + e.Message);
            if (e.InnerException != null)
                Console.WriteLine("\t\t" + e.InnerException.Message);
            Console.WriteLine("\t\t" + errorCode);
        }

        public void Error(string message, Exception e) {
            Console.WriteLine(message);
            Console.WriteLine("\t" + e.Message);
            if (e.InnerException != null)
                Console.WriteLine("\t\t" + e.InnerException.Message);
        }

        public void Error(string message) {
            Console.WriteLine(message);
        }
    }
}
