using System;
using System.Collections.Generic;
using System.Text;

namespace Demo
{
    internal class st
    {
        static int a = 10, b= 20;
        internal static void show()
        {
            int temp = a;
            a = b;
            b = temp;

            Console.WriteLine(a);
            Console.WriteLine(b);
        }
    }
}
