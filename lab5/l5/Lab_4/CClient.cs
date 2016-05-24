using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lab_4
{
    class CClient
    {
        private const string PipeName = "lab_4_pipe";

        public CClient()
        {
        }

        private NamedPipeClientStream CreatePipeClientStream()
        {
            return new NamedPipeClientStream(
                ".",
                PipeName,
                PipeDirection.InOut,
                PipeOptions.Asynchronous
            );
        }

        public void Start()
        {
            Console.WriteLine("Клиент пришёл. Pipe: {0}.", PipeName);
            while (true)
            {
                Thread.Sleep(100);
                var random = new Random();

                try
                {
                    SendCommand(Command.MeetClient);
                    Console.WriteLine("Встал в очередь");
                    Thread.Sleep(random.Next(1000, 8000));
                    var response = SendCommand(Command.LetMeDoHaircut);
                    while (response != Response.YouCanGo)
                    {
                        Console.WriteLine("Жду своей очереди...");
                        Thread.Sleep(5000);
                        response = SendCommand(Command.LetMeDoHaircut);
                    }
                    Console.WriteLine("Начал стричься");
                    Thread.Sleep(random.Next(1000, 8000));
                    Console.WriteLine("Стрижка окончена");
                    SendCommand(Command.Goodbye);
                    Console.WriteLine("Жду, пока не отрастут волосы...");
                    Thread.Sleep(random.Next(5000, 15000));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Thread.Sleep(1000);
                }
            }
        }

        private void TryConnect(NamedPipeClientStream pipeStream)
        {
            try
            {
                pipeStream.Connect();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Не могу найти парикмахерскую");
                Thread.Sleep(1000);
                TryConnect(pipeStream);
            }
        }

        private Response SendCommand(Command command)
        {
            var pipeStream = CreatePipeClientStream();
            TryConnect(pipeStream);
            pipeStream.WriteByte((byte)command);
            pipeStream.WaitForPipeDrain();
            var response = (Response)pipeStream.ReadByte();
            pipeStream.WaitForPipeDrain();
            return response;
        }
    }
}
