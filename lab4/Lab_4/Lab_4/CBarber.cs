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
        private ManualResetEvent mre;
        private static Mutex mtx = new Mutex();
        private static Random randomGenerator = new Random();

        public CBarber(ManualResetEvent evt)
        {
            barberThread = new Thread(Work);
            Console.WriteLine("Пришёл парикмхер");
            mre = evt;
            barberThread.Start();
        }

        public void WakeUp()
        {   
            mre.Set();
        }

        public void Work()
        {
            while (barberThread.IsAlive)
            {
                if (!barberShop.isWork)
                {
                    mre.WaitOne();
                }
                else
                {
                    if (barberShop.QueueStatus() != 0)
                    {
                        mtx.WaitOne();
                        CClient client = barberShop.GetFirst();
                        Console.WriteLine(client.clientThread.Name + " стрижётся");
                        Thread.Sleep(randomGenerator.Next(100, 1000));
                        client.WakeUp();
                        barberShop.LeaveQueue();
                        CBarbershop.clientsCount++;
                        mtx.ReleaseMutex();
                    }
                }
            }
        }
    }
}
