using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Lab_4
{
    enum Command
    {
        MeetClient = 1,
        LetMeDoHaircut = 2,
        Goodbye = 3,
        BarberWork = 4
    }

    enum Response
    {
        Ok = 0,
        YouMustSit = 1,
        YouCanGo = 2,
        ClientsInside = 3,
        Rest = 4
    }

    class CBarbershop
    {
        private const string PipeName = "lab_4_pipe";
        //private const int VinnieStrength = 3;

        private int _capacity;
        private int _clientsInside;
        private bool _isShaving;
       // private int _mealPortions;
        private NamedPipeServerStream pipeServer;

        public CBarbershop(int capacity)
        {
            _isShaving = false;
            _capacity = capacity;
            _clientsInside = 0;
        }

        private static NamedPipeServerStream CreatePipeServer()
        {
            return new NamedPipeServerStream(
                PipeName,
                PipeDirection.InOut,
                NamedPipeServerStream.MaxAllowedServerInstances,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous
            );
        }

        public void Start()
        {
            Console.WriteLine("Парикмахерская вместимостью {0} создана. Pipe: {1}.", _capacity, PipeName);
            while (true)
            {
                pipeServer = CreatePipeServer();
                pipeServer.WaitForConnection();
                try
                {
                    var command = (Command)pipeServer.ReadByte();
                    switch (command)
                    {
                        case Command.MeetClient:
                            MeetClient();
                            break;
                        case Command.LetMeDoHaircut:
                            HandleLetMeGo();
                            break;
                        case Command.Goodbye:
                            Goodbye();
                            break;
                        case Command.BarberWork:
                            BarberWork();
                            break;
                        default:
                            Console.WriteLine("Неизвестная команда");
                            break;
                    }
                    pipeServer.WaitForPipeDrain();
                    if (command != Command.LetMeDoHaircut)
                    {
                        PrintInfo();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private void MeetClient()
        {
            ++_clientsInside;
            pipeServer.WriteByte((byte)Response.Ok);
        }

        private void HandleLetMeGo()
        {
            if (_isShaving)
            {
                pipeServer.WriteByte((byte)Response.YouMustSit);
            }
            else
            {
                _isShaving = true;
                pipeServer.WriteByte((byte)Response.YouCanGo);
            }
        }     
          
        private void Goodbye()
        {
            --_clientsInside;
            _isShaving = false;
            pipeServer.WriteByte((byte)Response.Ok);
        }

        private void BarberWork()
        {
            if (_clientsInside <= 0)
            {
                Console.WriteLine("Клиентов пока нет, можно вздремнуть");
                pipeServer.WriteByte((byte)Response.Rest);
            }
            else
            {
                Console.WriteLine("Клиенты есть, работаю");
                //_mealPortions -= 1;
                pipeServer.WriteByte((byte)Response.ClientsInside);
            }
        }

        private void PrintInfo()
        {
            Console.WriteLine("Клиентов в очереди: {0}", _clientsInside);
        }

    }
}