using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;

class Controller
{
    static Process receiverProcess;

    static void Main(string[] args)
    {
        string pipeName = "testpipe";

        using (NamedPipeServerStream pipeServer = new NamedPipeServerStream(pipeName))
        {
            Console.WriteLine("Named Pipe Server created. Waiting for connection...");

            pipeServer.WaitForConnection();

            Console.WriteLine("Receiver process connected. Press Ctrl+C to send Control Break signal...");

            Console.CancelKeyPress += (sender, e) =>
            {
                using (StreamWriter writer = new StreamWriter(pipeServer))
                {
                    writer.AutoFlush = true;
                    writer.WriteLine("ControlBreak");
                }
                receiverProcess.WaitForExit();
                Environment.Exit(0);
            };

            // Infinite loop to keep the controller running
            while (true)
            {
                System.Threading.Thread.Sleep(100);
            }
        }
    }
}
