using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogHelperDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            LogHelper.Log(LogLevel.Trace, "static methods can log");

            LogHelper.Log(LogLevel.Trace, "a specific 'named' logger isn't necessary");

            LogHelper.Log<NeedsToLog>(LogLevel.Trace, "specify a named logger from a static method");

            try
            {
                throw new Exception("threw an exception...because I can.");
            }
            catch(Exception ex)
            {
                // log an exception with very little fuss.
                LogHelper.Log(ex);

                // the database did it! Let the logging system note that down
                LogHelper.Log<System.Data.Common.DbConnection>(ex);
            }

            var someClass = new NeedsToLog();

            someClass.MethodWithLogging();


            Console.WriteLine("press any key to continue...");

            Console.ReadKey();
        }
    }

    public class NeedsToLog
    {
        public void MethodWithLogging()
        {
            this.Log(LogLevel.Trace, "instance methods can log");

            this.Log(LogLevel.Trace, "a named logger is automatically created for you");
        }
    }
}
