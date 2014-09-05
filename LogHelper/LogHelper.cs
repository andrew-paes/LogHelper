using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository;

namespace System
{
    public static class LogHelper
    {
        private static string file = String.Empty;

        private static bool initialized = false;

        public static event Func<string, string> PathEvaluating;

        private static string OnPathEvaluating()
        {
            if (!String.IsNullOrEmpty(file)) return file;

            string path = null;

            if (ConfigurationManager.AppSettings.AllKeys.Contains(Constants.Configuration.Logging.PATH_PROPERTY_NAME))
                path = ConfigurationManager.AppSettings[Constants.Configuration.Logging.PATH_PROPERTY_NAME];


            Func<string, string> handler = LogHelper.PathEvaluating;
            if (handler != null)
                path = handler(path);

            if (!Directory.Exists(path))
                path = AppDomain.CurrentDomain.BaseDirectory;

            return path;
        }

        public static string LogFile
        {
            get
            {
                Initialize();

                return file;
            }
        }

        private class Unknown { }

        private static Unknown unknown = new Unknown();

        private static Dictionary<Type, ILog> _loggerDictionary;

        private static Dictionary<Type, ILog> LoggerDictionary
        {
            get
            {
                return _loggerDictionary ?? (_loggerDictionary = new Dictionary<Type, ILog>());
            }
        }

        private static ILog logger = LogManager.GetLogger(unknown.GetType());

        // TODO: make sure this still applies
        private static Dictionary<Type, ILog> separatedLoggers = new Dictionary<Type, ILog>()
        {
            {typeof(Unknown), logger}
        };

        public static void Log<T>(Exception ex)
        {
            LogHelper.Log<T>(LogLevel.Error, null, ex);
        }

        public static void Log<T>(LogLevel level, string message)
        {
            LogHelper.Log<T>(level, message, null);
        }

        public static void Log<T>(LogLevel level, string format, params object[] args)
        {
            LogHelper.Log(typeof(T), level, format, args);
        }

        public static void Log(Exception ex)
        {
            LogHelper.Log(unknown, LogLevel.Error, null, ex);
        }

        public static void Log(LogLevel level, string message)
        {
            LogHelper.Log(unknown, level, message, null);
        }

        public static void Log(LogLevel level, string format, params object[] args)
        {
            LogHelper.Log(unknown, level, format, args);
        }

        public static void Log(object obj, LogLevel level, string format, params object[] args)
        {
            Initialize();

            log4net.Core.Level logLevel = GetLevel(level);

            if (logLevel == null || !logger.Logger.IsEnabledFor(logLevel))
                return;

            Exception ex = args != null ? args.OfType<Exception>().FirstOrDefault() : null;

            string message = (format != null) ? (args != null ? String.Format(format, args) : format) : format;

            Type objType = (obj is Type) ? (obj as Type) : obj.GetType();

            string type = (obj == null || obj is Unknown) ? "static|" :
                objType.FullName + "|";

            // determine which logger to use
            ILog iLog = (separatedLoggers.ContainsKey(objType)) ? separatedLoggers[objType] : logger;

            iLog.Logger.Log(objType, logLevel, type + message, ex);

            Console.WriteLine(type + message);
        }

        public static void SetLevel(LogLevel level)
        {
            log4net.Core.Level newLevel = GetLevel(level);

            SetRootLevel(newLevel);
        }

        public static T GetProperty<T>(String propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return default(T);

            object value = log4net.GlobalContext.Properties[propertyName];

            if (value != null && typeof(T).IsAssignableFrom(value.GetType()))
                return (T)value;

            return default(T);
        }

        public static void SetProperty(String propertyName, Object value)
        {
            if (string.IsNullOrEmpty(propertyName) || value == null)
                return;

            if (string.Equals(propertyName, Constants.Configuration.USER_NAME_PROPERTY))
                value = "." + value.ToString().Replace(" ", "-").ToLowerInvariant();

            log4net.GlobalContext.Properties[propertyName] = value;
            ILoggerRepository repository = LogManager.GetRepository();
            log4net.Repository.Hierarchy.Hierarchy hierarchy = repository as log4net.Repository.Hierarchy.Hierarchy;
            hierarchy.RaiseConfigurationChanged(EventArgs.Empty);
        }

        public static void SeparateOut(Type type)
        {
            if (type == null || separatedLoggers.ContainsKey(type)) return;

            ILoggerRepository repository = LogManager.GetRepository();
            if (repository == null) return;

            IEnumerable<AppenderSkeleton> appenders = repository.GetAppenders().OfType<AppenderSkeleton>();
            if (appenders == null) return;

            ILog iLog = LogManager.GetLogger(type);

            separatedLoggers.Add(type, iLog);

            log4net.Filter.LoggerMatchFilter filter = new log4net.Filter.LoggerMatchFilter(),
                 defaultLoggerFilter = new log4net.Filter.LoggerMatchFilter();
            filter.LoggerToMatch = iLog.Logger.Name;
            defaultLoggerFilter.AcceptOnMatch = filter.AcceptOnMatch = true;

            defaultLoggerFilter.LoggerToMatch = logger.Logger.Name;

            foreach (AppenderSkeleton appender in appenders)
            {
                List<log4net.Filter.IFilter> filters = new List<log4net.Filter.IFilter>();

                log4net.Filter.IFilter current = appender.FilterHead;

                if (current != null)
                {
                    filters.Add(current);
                    while ((current = current.Next) != null)
                        filters.Add(current);
                }

                appender.ClearFilters();
                appender.AddFilter(defaultLoggerFilter);

                foreach (log4net.Filter.IFilter f in filters)
                    appender.AddFilter(f);
            }

            RollingFileAppender separateAppender = new RollingFileAppender();
            separateAppender.File = Path.Combine(
                GlobalContext.Properties[Constants.Configuration.Logging.PATH_PROPERTY_NAME].ToNullSafeString(),
                type.FullName.Replace("+", "_") + ".log");
            separateAppender.AppendToFile = true;
            separateAppender.SecurityContext = log4net.Core.SecurityContextProvider.DefaultProvider.CreateSecurityContext(separateAppender);
            separateAppender.RollingStyle = RollingFileAppender.RollingMode.Size;
            separateAppender.DatePattern = ".yyyyMMdd";
            separateAppender.CountDirection = 1;
            separateAppender.MaxSizeRollBackups = 7;
            separateAppender.MaximumFileSize = "4MB";
            separateAppender.AddFilter(filter);
            log4net.Layout.PatternLayout layout = new log4net.Layout.PatternLayout();
            layout.Header = "Date|Host|Environment|Thread|Level|Message\r\n";
            layout.ConversionPattern = "%date|%property{log4net:HostName}|%2thread|%-5level|%message%newline";
            separateAppender.Layout = layout;
            log4net.Repository.Hierarchy.Hierarchy hierarchy = repository as log4net.Repository.Hierarchy.Hierarchy;
            hierarchy.Root.AddAppender(separateAppender);
            hierarchy.RaiseConfigurationChanged(EventArgs.Empty);
        }

        public static void RemoveProperty(String propertyName, Object value)
        {
            log4net.ThreadContext.Properties.Remove(propertyName);
        }

        public static string SetLogPath(String path)
        {
            if (String.IsNullOrEmpty(path) || !Directory.Exists(path))
                return null;

            LogHelper.SetProperty(Constants.Configuration.Logging.PATH_PROPERTY_NAME, path);

            RollingFileAppender appender = LogHelper.GetAppender<RollingFileAppender>();
            if (appender != null)
                LogHelper.file = appender.File;

            return LogHelper.LogFile;
        }

        private static log4net.Core.Level GetLevel(LogLevel logLevel)
        {

            log4net.Core.Level level = null;
            Type levelType = typeof(log4net.Core.Level);


            IList<System.Reflection.FieldInfo> fields = levelType.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).ToList();

            foreach (System.Reflection.FieldInfo field in fields)
            {
                if (levelType.IsAssignableFrom(field.FieldType))
                {
                    log4net.Core.Level temp = field.GetValue(null) as log4net.Core.Level;
                    if (null == temp || !((Int32)logLevel).Equals(temp.Value)) continue;

                    level = temp;
                    break;
                }
            }
            return level;
        }

        /// <summary>
        /// Sets the root of the log4net hierarchy to the level specified
        /// </summary>
        /// <param name="level">the level to which to set the root</param>
        private static void SetRootLevel(log4net.Core.Level level)
        {
            if (level == null) return;

            log4net.Repository.Hierarchy.Hierarchy hierarchy = log4net.LogManager.GetRepository() as log4net.Repository.Hierarchy.Hierarchy;
            if (null != hierarchy)
            {
                lock (hierarchy) { hierarchy.Root.Level = level; }
            }
        }

        /// <summary>
        /// Sets the <see cref="IAppender"/>'s filter level to the level specified
        /// </summary>
        /// <param name="appender">the <see cref="IAppender"/> to change</param>
        /// <param name="newLevel">the new level at which the <see cref="IAppender"/> should log</param>
        private static void SetAppenderFilterLevel(IAppender appender, log4net.Core.Level newLevel)
        {
            if (appender == null || newLevel == null) return;

            AppenderSkeleton baseAppender = appender as AppenderSkeleton;
            if (baseAppender == null) return;



            log4net.Filter.LevelRangeFilter rangeFilter = GetLevelRangeFilter(baseAppender);

            if (rangeFilter == null)
                return;

            // lock the IAppender so that its clients wait until this process completes
            lock (baseAppender)
            {
                appender.DoAppend(new log4net.Core.LoggingEvent(logger.GetType(), log4net.LogManager.GetRepository(),
                    logger.GetType().Name, log4net.Core.Level.Info, "Changed log level to " + newLevel.ToString(), null));

                rangeFilter.LevelMin = newLevel;
            }
        }

        private static T GetAppender<T>() where T : IAppender
        {
            ILoggerRepository repository = log4net.LogManager.GetRepository();
            if (repository == null)
                return default(T);

            IAppender[] appenders = repository.GetAppenders();
            if (appenders == null)
                return default(T);

            foreach (IAppender appender in appenders)
            {
                if ((typeof(T).IsAssignableFrom(appender.GetType())))
                    return (T)appender;
            }

            return default(T);
        }

        private static log4net.Filter.LevelRangeFilter GetLevelRangeFilter(AppenderSkeleton appender)
        {
            log4net.Filter.LevelRangeFilter rangeFilter = null;

            lock (appender)
            {
                log4net.Filter.IFilter filterHead = appender.FilterHead;

                //process through the filters until a level range filter is found
                while (filterHead != null)
                {
                    if (typeof(log4net.Filter.LevelRangeFilter).Equals(filterHead.GetType()))
                    {
                        rangeFilter = filterHead as log4net.Filter.LevelRangeFilter;
                        break;
                    }

                    filterHead = filterHead.Next;
                }
            }
            return rangeFilter;
        }

        private static void Initialize()
        {
            lock (unknown)
            {
                if (initialized) return;

                // one can setup the log path to be a directory of their choice and configure it from any source:
                // for instance, DB, app.config, etc
                string logPath = log4net.GlobalContext.Properties[Constants.Configuration.Logging.PATH_PROPERTY_NAME].ToNullSafeString();

                if (string.IsNullOrEmpty(logPath))
                {
                    logPath = ConfigurationManager.AppSettings.AllKeys.Contains(Constants.Configuration.Logging.PATH_PROPERTY_NAME) ?
                        ConfigurationManager.AppSettings[Constants.Configuration.Logging.PATH_PROPERTY_NAME] :
                        String.Empty;
                }

                if (String.IsNullOrEmpty(logPath))
                    logPath = LogHelper.OnPathEvaluating();

                log4net.GlobalContext.Properties[Constants.Configuration.Logging.PATH_PROPERTY_NAME] =
                    (!String.IsNullOrEmpty(logPath) && Directory.Exists(logPath)) ?
                    logPath :
                    AppDomain.CurrentDomain.BaseDirectory;

                log4net.GlobalContext.Properties[Constants.Configuration.APPLICATION_NAME_PROPERTY] =
                    ConfigurationManager.AppSettings.AllKeys.Contains(Constants.Configuration.APPLICATION_NAME_PROPERTY) ?
                    ConfigurationManager.AppSettings[Constants.Configuration.APPLICATION_NAME_PROPERTY] :
                    System.Text.RegularExpressions.Regex.Replace(AppDomain.CurrentDomain.FriendlyName, "[.][^.]+$", String.Empty);

                // the user name might not be known at type initialization
                log4net.GlobalContext.Properties[Constants.Configuration.USER_NAME_PROPERTY] =
                    "." + Environment.UserName.Replace(" ", "-").ToLowerInvariant();

                
            object logConfig = ConfigurationManager.GetSection("log4net");
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.config")))
            {
                XmlConfigurator.Configure(new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.config")));
            }
            if (logConfig != null)
            {
                //Load from App.Config file
                XmlConfigurator.Configure();
            }
            else
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(LogHelperResources.Resources.log);
                        writer.Flush();
                        stream.Position = 0;

                        XmlConfigurator.Configure(stream);
                    }
                }
            }
                RollingFileAppender appender = LogHelper.GetAppender<RollingFileAppender>();
                if (appender != null)
                    LogHelper.file = appender.File;

                initialized = true;
            }
        }
    }
}
