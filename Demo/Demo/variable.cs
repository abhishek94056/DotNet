using System;
using System.Collections.Generic;
using System.Text;

namespace Demo
{
    internal class variable
    {
        int a = 10;
        static int b =20;

        internal void show()
        {
            Console.WriteLine(b);
            Console.WriteLine(b);
        }
        internal static void show1()
        {
            Console.WriteLine(b);
        }

        const int clgid = 101;

        internal void show2()
        {
            Console.WriteLine(clgid);
        }

    }
}
