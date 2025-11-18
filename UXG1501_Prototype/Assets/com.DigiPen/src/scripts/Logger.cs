using System;
using System.Reflection;
using UnityEngine;

public static class Logger
{
    public enum SEVERITY_LEVEL
    {
        INFO = 0x01,
        WARNING = 0x02,
        ERROR = 0x03
    }

    public enum LOGGER_OPTIONS
    {
        SIMPLE = 0x01,
        VERBOSE = 0x02
    }

    private static readonly string SIMPLE_FORMAT = "{0}";
    private static readonly string VERBOSE_FORMAT = "{0}.{1}: {2}";

    public static void Log(string message, SEVERITY_LEVEL level, LOGGER_OPTIONS loggerOptions = LOGGER_OPTIONS.VERBOSE, MethodBase method = null)
    {
        string output = "";

        if (method == null)
            loggerOptions = LOGGER_OPTIONS.SIMPLE;

        switch (loggerOptions)
        {
            case LOGGER_OPTIONS.SIMPLE:
                output = string.Format(SIMPLE_FORMAT, message);
                break;
            case LOGGER_OPTIONS.VERBOSE:
                output = string.Format(VERBOSE_FORMAT, method.ReflectedType.Name, method.Name, message);
                break;
        }
        switch (level)
        {
            case SEVERITY_LEVEL.INFO:
                Debug.Log(output);
                break;
            case SEVERITY_LEVEL.WARNING:
                Debug.LogWarning(output);
                break;
            case SEVERITY_LEVEL.ERROR:
                Debug.LogError(output);
                break;
        }
    }
}
