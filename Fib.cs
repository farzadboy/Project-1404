using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Fibonacci
{
    static int Fib(int n, int a = 0, int b = 1)
    {
        if (n == 0)
        {
            return a;
        }
        else
        {
            return Fib(n - 1, b, a + b);
        }
    }

    static void Main()
    {
        Console.Write("Enter number: ");
        int num = int.Parse(Console.ReadLine());
        Console.WriteLine("Fibonacci({0}) = {1}", num, Fib(num));

        Console.ReadKey();
    }
}
