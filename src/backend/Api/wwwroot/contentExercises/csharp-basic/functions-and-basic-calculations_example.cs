using System;

    public static class BasicMathExamples
    {
        public static int Add(int a, int b)
        {
            return a + b;
        }

        public static int Multiply(int a, int b)
        {
            return a * b;
        }

        public static double CalculateAverage(int a, int b, int c)
        {
            return (a + b + c) / 3.0;
        }

        public static void Main()
        {
            Console.WriteLine(Add(2, 3));
            Console.WriteLine(Multiply(4, 5));
            Console.WriteLine(CalculateAverage(3, 6, 9));
        }
    }
