using Microsoft.Extensions.Logging;
using Spectre.Console;
using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;

namespace LiveLikeBossSpawnChances.Server.Internal;

// Copied from https://github.com/sp-tarkov/server-csharp/blob/main/Testing/UnitTests/Mock/MockLogger.cs

[Injectable]
public class MockLogger<T> : ISptLogger<T>
{
    public void LogWithColor(string data, Color? textColor = null, Color? backgroundColor = null, Exception? ex = null)
    {
        Console.WriteLine(data);
    }

    public void Success(string data, Exception? ex = null)
    {
        Console.WriteLine(data);
    }

    public void Error(string data, Exception? ex = null)
    {
        Console.WriteLine(data);
    }

    public void Warning(string data, Exception? ex = null)
    {
        Console.WriteLine(data);
    }

    public void Info(string data, Exception? ex = null)
    {
        Console.WriteLine(data);
    }

    public void Debug(string data, Exception? ex = null)
    {
        Console.WriteLine(data);
    }

    public void Critical(string data, Exception? ex = null)
    {
        Console.WriteLine(data);
    }

    public void Log(
        LogLevel level,
        string data,
        Color? textColor = null,
        Color? backgroundColor = null,
        Exception? ex = null
    )
    {
        throw new NotImplementedException();
    }

    public void WriteToLogFile(string body, LogLevel level = LogLevel.Information)
    {
        throw new NotImplementedException();
    }

    public bool IsLogEnabled(LogLevel level)
    {
        return true;
    }

    public void DumpAndStop()
    {
        throw new NotImplementedException();
    }

    public void LogWithColor(string data, Exception? ex = null, Color? textColor = null, Color? backgroundColor = null)
    {
        Console.WriteLine(data);
    }

    public void WriteToLogFile(object body)
    {
        Console.WriteLine(body);
    }
}