using System;
using System.Collections.Generic;

public static class Program
{
    public static void Main()
    {
        var numbers = new List<int> { 3, 1, 4 };
        numbers.Add(1);
        numbers.Remove(3);

        int sum = 0;
        foreach (var n in numbers) sum += n;

        Console.WriteLine($"Count={numbers.Count}, Sum={sum}");
    }
}
