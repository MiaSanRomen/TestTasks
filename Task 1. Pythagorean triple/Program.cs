using System;

namespace TestTasks
{
    class Program
    {
        static void Main(string[] args)
        {
            int iterations = 0;                 //variable for counting iterations of solution
            Console.WriteLine("First solution:");
            for (int n = 1; n < 100; n++)
            {
                for (int m = n + 1; m < 100; m++)
                {
                    iterations++;
                    if (GetAbcEuclid(m, n))
                    {
                        Console.WriteLine($"Number of iterations: {iterations}");
                        break;
                    }
                }
            }

            Console.WriteLine("\nSecond solution:");
            for (int n = 1; n < 100; n++)
            {
                if (GetAbcDiscriminant(n))
                {
                    Console.WriteLine($"Number of iterations: {n}");
                    break;
                }
            }
        }

        public static bool GetAbcEuclid(int m, int n)           //This method is used to verify if 'm' and 'n' can generate Pythagorean triple we need
        {
            int a = m * m - n * n;                              //Euclidean formulas for generating Pythagorean triples
            int b = 2 * m * n;
            int c = m * m + n * n;
            int sum = a + b + c;
            int product = a * b * c;

            if (sum == 1000)                                   //Validating are these 'a', 'b', 'c' what we need.
            {
                Console.WriteLine($"m={m}, n={n}");
                Console.WriteLine($"a={a}, b={b}, c={c}");
                Console.WriteLine($"Summ: {sum}");
                Console.WriteLine($"Product abc: {product}");
                return true;
            }

            return false;
        }

        public static bool GetAbcDiscriminant(int n)            //This method is used to verify if 'n' can be used for creating discriminant to find 'm' we need
        {                                                       //Better solution. Details in attached file
            double discriminant = n * n + 2000;
            if (Math.Sqrt(discriminant) % 1 == 0)               //Validate if discriminant is integer
            {
                int m = ((int) Math.Sqrt(discriminant) - n) / 2;         //Getting 'm' with help of discriminant 
                return GetAbcEuclid(m, n);
            }

            return false;
        }
    }
}
