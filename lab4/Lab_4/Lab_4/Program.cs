using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Lab_4
{
    class Program
    {
        static void Main()
        {
            AutoResetEvent evtObj = new AutoResetEvent(false);
            CBarber barber = new CBarber(evtObj);
            Random randomGenerator = new Random();
            for (int i = 1; i < 26; i++)
            {
                AutoResetEvent clientEvent = new AutoResetEvent(false);
                CClient client = new CClient(i, barber, clientEvent);
                Thread.Sleep(randomGenerator.Next(100, 1000));
            }
           
            //_barber.barberThread.Abort();
        }
    }
}
