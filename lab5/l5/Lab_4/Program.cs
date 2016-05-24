using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_4
{
    class Program
    {
        static void ArgumentsError()
        {
            Console.WriteLine("Используйте:");
            Console.WriteLine("lab_4.exe barbershop <вместимость парикмахерской>");
            Console.WriteLine("lab_4.exe client");
            Console.WriteLine("lab_4.exe barber");
            Environment.Exit(1);
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                ArgumentsError();
            }
            switch (args[0])
            {
                case "barbershop":
                    if (args.Length < 2)
                    {
                        ArgumentsError();
                    }
                    var barbershop = new CBarbershop(int.Parse(args[1]));
                    barbershop.Start();
                    break;
                case "client":
                    var client = new CClient();
                    client.Start();
                    break;
                case "barber":
                    var barber = new CBarber();
                    barber.Start();
                    break;
            }
        }
    }
}
