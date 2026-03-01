using System;
using System.Collections.Generic;

public static class Program
{
    public static void Main()
    {
        var a = new HashSet<int> { 1, 2, 3, 3 };
        var b = new HashSet<int> { 3, 4, 5 };

        a.IntersectWith(b);
        Console.WriteLine(string.Join(", ", a)); // 3
    }
}
