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
        private AutoResetEvent clientEvent = new AutoResetEvent(false);
        private CBarber barberThread;

        public CClient(int id, CBarber barber, AutoResetEvent evt)
        {
            barberThread = barber;
            clientEvent = evt;
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
            clientEvent.Set();
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
            clientEvent.WaitOne();
        }
    }
}
