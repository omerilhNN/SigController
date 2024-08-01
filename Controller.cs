using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

class Controller
{
    const uint CTRL_C_EVENT = 0;
    const uint CTRL_BREAK_EVENT = 1;
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool GenerateConsoleCtrlEvent(uint dwCtrlEvent, uint dwProcessGroupId);
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool AttachConsole(uint dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool FreeConsole();

    [DllImport("kernel32.dll")]
    static extern uint GetLastError();


    static void Main(string[] args)
    {
        string processName = "Recv"; // Aranacak process'in ismi

        Process[] processes = Process.GetProcessesByName(processName);
        if (processes.Length > 0)
        {
            // İlk eşleşen process'in PID'sini al ve yazdır
            int firstProcessId = processes[0].Id;
            Console.WriteLine($"First process with name '{processName}' has PID: {firstProcessId}");

            Console.WriteLine("Press Ctrl+Break to send the signal...");

            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true; // Uygulamayı hemen sonlandırmayı önler

                // Konsolu serbest bırak ve hedef process'in konsoluna bağlan - Bir Process aynı anda 1 tek konsola bağlı olabilir.
                FreeConsole();
                if (AttachConsole((uint)firstProcessId))
                {
                    bool result = GenerateConsoleCtrlEvent(CTRL_BREAK_EVENT, (uint)firstProcessId);

                    if (result)
                    {
                        Console.WriteLine("CTRL+BREAK signal sent successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to send CTRL+BREAK signal. Error code: {Marshal.GetLastWin32Error()}");
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to attach to console of process {firstProcessId}. Error code: {Marshal.GetLastWin32Error()}");
                }
            };

            // Sonsuz döngü, kullanıcı girişini bekler
            while (true)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }
        else
        {
            Console.WriteLine("Process not found.");
        }
    }
}
