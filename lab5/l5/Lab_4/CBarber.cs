using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Lab_4
{
    class CBarber
    {
        private const string PipeName = "lab_4_pipe";


        public CBarber()
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
            Console.WriteLine("Парикмахер пришёл на работу. Pipe: {0}.", PipeName);

            var random = new Random();
            var restTime = random.Next(1000, 5000);
            Console.WriteLine("Дремаю");
            Thread.Sleep(restTime);
            while (true)
            {
                try
                {
                    var response = SendCommand(Command.BarberWork);
                    switch (response)
                    {
                        case Response.Rest:
                            restTime = random.Next(1000, 5000);
                            Console.WriteLine("Можно поспать");
                            Thread.Sleep(restTime);
                            break;
                        case Response.ClientsInside:
                            restTime = random.Next(1000, 5000);
                            Console.WriteLine("Займусь клиентом");
                            Thread.Sleep(restTime);
                            break;
                        default:
                            Console.WriteLine("Неизвестная команда");
                            Thread.Sleep(1000);
                            break;
                    }
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