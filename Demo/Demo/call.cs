using System;
using System.Collections.Generic;
using System.Text;

namespace Demo
{
    internal class call
    {
        int a, b, c;
        internal void accept()
        {
            Console.WriteLine("Enter First Number: ");
            a = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter Second Number: ");
            b = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Result is",a);
        }
        internal void sum()
        {
            c = a + b;
            Console.WriteLine("Sum of number is: ");
            Console.WriteLine(c);
        }
    }
}
