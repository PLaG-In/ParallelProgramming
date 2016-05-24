using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Lab_4
{
    class CClient
    {
        public Thread clientThread;
        private int idClient;
        private ManualResetEvent mre1 = new ManualResetEvent(false);
        private CBarber barberThread;

        public CClient(int id, CBarber barber, ManualResetEvent MRE)
        {
            barberThread = barber;
            mre1 = MRE;
            idClient = id;
            clientThread = new Thread(new ThreadStart(Shave));
            clientThread.IsBackground = true;
            clientThread.Start();
        }

        public int GetIdClient()
        {
            return idClient;
        }

        public void WakeUp()
        {
            mre1.Set();
        }

        void Shave()
        {
            clientThread.Name = ("Клиент " + idClient);
            Console.WriteLine(clientThread.Name + " пришёл в парикмахерскую");
            if (!barberThread.barberShop.isWork)
            {
                Console.WriteLine("Будит парикмахера");
                barberThread.WakeUp();
            }
            if (barberThread.barberShop.QueueStatus() != 0)
            {
                Console.WriteLine(clientThread.Name + " встаёт в очередь");
            }
            barberThread.barberShop.GoToQueue(this);
            mre1.WaitOne();
            Console.WriteLine(clientThread.Name + " подстригся");
        }
    }
}
