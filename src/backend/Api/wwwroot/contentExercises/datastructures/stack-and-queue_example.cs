using System;
using System.Collections.Generic;

public static class Program
{
    public static void Main()
    {
        var stack = new Stack<int>();
        stack.Push(10);
        stack.Push(20);
        Console.WriteLine(stack.Pop()); // 20

        var queue = new Queue<string>();
        queue.Enqueue("A");
        queue.Enqueue("B");
        Console.WriteLine(queue.Dequeue()); // A
    }
}
