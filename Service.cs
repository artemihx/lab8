using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using cafeapp1.Models;

namespace cafeapp1;

public class Service
{
    private static PostgresContext? _db;

    public static PostgresContext GetContext()
    {
        if (_db == null)
        {
            _db = new PostgresContext();
        }
        return _db;
    }
    
    public static async Task WaitForFileToAppear(string filePath, int timeoutMillis = 5000, int pollingIntervalMillis = 100)
    {
        var timeout = TimeSpan.FromMilliseconds(timeoutMillis);
        var pollingInterval = TimeSpan.FromMilliseconds(pollingIntervalMillis);
        var stopwatch = Stopwatch.StartNew();

        while (stopwatch.Elapsed < timeout)
        {
            if (File.Exists(filePath))
            {
                return;
            }
            await Task.Delay(pollingInterval);
        }

        throw new FileNotFoundException($"File {filePath} did not appear within the timeout period.");
    }
}