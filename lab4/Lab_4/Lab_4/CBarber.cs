using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Lab_4
{
    class CBarber
    {
        public CBarbershop barberShop = new CBarbershop();
        private Thread barberThread = Thread.CurrentThread;
        private AutoResetEvent barberEvent;
        //private static Mutex mtx = new Mutex();
        private static Random randomGenerator = new Random();

        public CBarber(AutoResetEvent evt)
        {
            barberThread = new Thread(Work);
            Console.WriteLine("Пришёл парикмхер");
            barberEvent = evt;
            barberThread.Start();
        }

        public void WakeUp()
        {   
            barberEvent.Set();
        }

        public void Work()
        {
            while (barberThread.IsAlive)
            {
                if (!barberShop.isWork)
                {
                    barberEvent.WaitOne();
                    Thread.Sleep(1000);
                }
                else
                {
                    if (barberShop.QueueStatus() != 0)
                    {
                        //mtx.WaitOne();
                        CClient client = barberShop.GetFirst();
                        Console.WriteLine(client.clientThread.Name + " стрижётся");
                        Thread.Sleep(randomGenerator.Next(100, 1000));
                        CBarbershop.clientsCount++;
                        Console.WriteLine(client.clientThread.Name + " подстригся");
                        barberShop.LeaveQueue();
                        client.WakeUp();
                        //mtx.ReleaseMutex();
                    }
                }
            }
        }
    }
}
