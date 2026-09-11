using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

public class Debug
{
    static public void Log<T>(T Output)
    {
        Console.WriteLine(Output?.ToString());
    }

    static public void Test()
    {
        Debug.Log("Test passed");
    }

    static public void MeasureFunctionSpeed(Action Function, string FunctionName = "")
    {
        Stopwatch stopwatch = new Stopwatch();

        stopwatch.Start();

        Function();

        Debug.Log("Function: " + FunctionName + " done in: " + stopwatch.Elapsed);
    }
}