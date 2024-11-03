// See https://aka.ms/new-console-template for more information
using System.Threading.Channels;

Console.WriteLine("Hello, World!");

namespace Stage0
{
    partial class program {
        static void main(string[]arge)
        {
            Wellcome9566();
            Wellcome5027();

            Console.ReadKey();

        }
         static partial void Wellcome5027();
        private static void Wellcome9566()
        {
            Console.WriteLine("Enter your name: ");
            string name = Console.ReadLine();
            Console.WriteLine("{0} Welcome to my first consol application", name);
        }
    }

}
