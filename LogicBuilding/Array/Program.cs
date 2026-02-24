namespace Array
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] id = new int[5];

            id[0] = 2;
            id[1] = 3;
            id[2] = 4;
            id[3] = 5;

            Console.WriteLine(id[1]);
            //-----------------------------------------------------------------------------------------
            int[] num = {8,5,3,9,6 };
            foreach(int i in num)
            {
                Console.WriteLine(i);
            }
        }
    }
}
