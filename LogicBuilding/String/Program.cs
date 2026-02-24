namespace String
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string str = "Hello";
            str = str + " World";
            Console.WriteLine(str);
            Console.WriteLine(str.ToLower());
            Console.WriteLine(str.ToUpper());
            Console.WriteLine(str.Length);
            Console.WriteLine(str.Substring(0,3));
            Console.WriteLine(str.Split());
            Console.WriteLine(str.Contains("Hello"));
        }
    }
}
