using System;
using System.Threading.Tasks;
using log4net;
using log4net.Core;

namespace Log4NetlyTesting {
    class Program {
        static void Main(string[] args) {
            log4net.Config.XmlConfigurator.Configure();
            var logger = LogManager.GetLogger(typeof(Program));
            ////logger.Info("I know he can get the job, but can he do the job?");
            ////logger.Debug("I'm not arguing that with you.");
            ////logger.Warn("Be careful!");

            logger.Error("Have you used a computer before?", new FieldAccessException("You can't access this field.", new AggregateException("You can't aggregate this!")));
            try {
                var hi = 1/int.Parse("0");
            } catch (Exception ex) {
                logger.Error("I'm afraid I can't do that.", ex);
            }

            var loggingEvent = new LoggingEvent(typeof(LogManager), logger.Logger.Repository, logger.Logger.Name, Level.Fatal, "Fatal properties, here.", new IndexOutOfRangeException());
            loggingEvent.Properties["Foo"] = "Bar";
            loggingEvent.Properties["Han"] = "Solo";
            loggingEvent.Properties["Two Words"] = "Three words here";
            logger.Logger.Log(loggingEvent);

            var forEachTask = Task.Factory.StartNew(() => {
                Parallel.For(0, 10, i => {
                    logger.ErrorFormat("Error Loop #{0}.", i);
                });

                Console.WriteLine("Loop launched.");
            });

            Task.WaitAll(forEachTask);
            Console.WriteLine("Wait complete.");

            Console.ReadKey();
        }
    }
}
