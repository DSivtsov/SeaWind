using System;

    public static class ConsoleCalculator
    {
        public static double Add(double a, double b)
        {
            return a + b;
        }

        public static double Subtract(double a, double b)
        {
            return a - b;
        }

        public static void Main()
        {
            Console.Write("Enter first number: ");
            double a = double.Parse(Console.ReadLine() ?? "0");

            Console.Write("Enter operation (+ or -): ");
            string operation = Console.ReadLine() ?? "+";

            Console.Write("Enter second number: ");
            double b = double.Parse(Console.ReadLine() ?? "0");

            double result = operation == "-" ? Subtract(a, b) : Add(a, b);

            Console.WriteLine($"Result: {result}");
        }
    }
