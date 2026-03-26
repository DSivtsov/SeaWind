using System;

    public static class ConditionalStatementsExample
    {
        public static string GetGradeCategory(int score)
        {
            if (score >= 90)
            {
                return "Excellent";
            }

            if (score >= 70)
            {
                return "Good";
            }

            if (score >= 50)
            {
                return "Pass";
            }

            return "Fail";
        }

        public static void Main()
        {
            Console.WriteLine(GetGradeCategory(95));
            Console.WriteLine(GetGradeCategory(72));
            Console.WriteLine(GetGradeCategory(41));
        }
    }
