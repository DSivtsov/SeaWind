using System;
using System.Threading.Tasks;

public static class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Start");
        await Task.Delay(300);
        Console.WriteLine("After delay");
    }
}
