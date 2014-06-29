using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Log4NetlyTesting {
    class Program {
        static void Main(string[] args) {
            log4net.Config.XmlConfigurator.Configure();
            var logger = LogManager.GetLogger(typeof(Program));
            ////logger.Info("I know he can get the job, but can he do the job?");
            ////logger.Debug("I'm not arguing that with you.");
            ////logger.Warn("Be careful!");

            logger.Error("Now you've done it...", new Exception("This is my exception message.", new Exception("This is my inner exception message.")));
            try {
                var hi = 1/int.Parse("0");
            } catch (Exception ex) {
                logger.Error("Now you've done it...", ex);
            }
            ////logger.Fatal("That's it. It's over.");
        }
    }
}
