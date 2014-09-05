using System;

namespace System
{
    /// <summary>
    /// Provides logging aliases for log4net logging levels
    /// </summary>
    public enum LogLevel
    {

        Off = int.MaxValue,
        Emergency = 120000,
        Fatal = 110000,
        Alert = 100000,
        Critical = 90000,
        Severe = 80000,
        Error = 70000,
        Warn = 60000,
        Notice = 50000,
        Info = 40000,
        Debug = 30000,
        Fine = 30000,
        Trace = 20000,
        Finer = 20000,
        Verbose = 10000,
        Finest = 10000,
        Unknown = 0,
        All = int.MinValue
    }
    
}