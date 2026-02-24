namespace LogicBuilding
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //-----------------------------------------------------------------------------------------
            int age = 18;
            if (age >= 18)
            {
                Console.WriteLine("You are eligible to vote");
            }

            //-----------------------------------------------------------------------------------------

            int number = 5;

            if (number % 2 == 0)
            {
                Console.WriteLine("Even Number");
            }
            else
            {
                Console.WriteLine("Odd Number");
            }

            //-----------------------------------------------------------------------------------------

            int marks = 75;

            if (marks >= 90)
            {
                Console.WriteLine("Grade A");
            }
            else if (marks >= 60)
            {
                Console.WriteLine("Grade B");
            }
            else
            {
                Console.WriteLine("Grade C");
            }
                
            //-----------------------------------------------------------------------------------------
            int day = 2;

            switch (day)
            {
                case 1:
                    Console.WriteLine("Monday");
                    break;
                case 2:
                    Console.WriteLine("Tuesday");
                    break;
                default:
                    Console.WriteLine("Invalid Day");
                    break;
            }

            //-----------------------------------------------------------------------------------------


            for(int i = 0; i <= 10; i++)
            {
                Console.WriteLine(i);
            }

            //-----------------------------------------------------------------------------------------

            int n = 0;
            while (n < 10)
            {
                Console.WriteLine(n);
                n++;
            }

            //-----------------------------------------------------------------------------------------
            Console.WriteLine("Hello");
            do
            {
                Console.WriteLine(n);
                n++;
            } while (n < 12);
        }
    }
}
