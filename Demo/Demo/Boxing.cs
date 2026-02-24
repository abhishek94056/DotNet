using System;
using System.Collections.Generic;
using System.Text;

namespace Demo
{
    internal class Boxing

    {
        int a = 10;
        Object o;
        internal void show()
        {
            o = a;
            Console.WriteLine(o);
            
        }
        internal void show1()
        {
            a = (int)o;
            Console.WriteLine(a);
        }
    }
}
