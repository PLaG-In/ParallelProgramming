using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_4
{
    class CBarbershop
    {
        private Queue<CClient> ClientQueue = new Queue<CClient>();
        public static int clientsCount = 0;
        public bool isWork = false;

        public void GoToQueue(CClient client_)
        {
            isWork = true;
            ClientQueue.Enqueue(client_);
        }

        public CClient GetFirst()
        {
            return ClientQueue.Peek();
        }

        public void LeaveQueue()
        {
            ClientQueue.Dequeue();
            if (QueueStatus() == 0)
            {
                isWork = false;
            }
        }

        public int QueueStatus()
        {
            return (ClientQueue.Count());
        }
    }
}
