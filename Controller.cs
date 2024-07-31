using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

class Controller
{
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool GenerateConsoleCtrlEvent(uint dwCtrlEvent, uint dwProcessGroupId);

    private const uint CTRL_BREAK_EVENT = 1;
    private static Process targetProcess;

    static void Main(string[] args)
    {
        string targetProcessName = "Process"; // Receiver process adını buraya yazın
        uint targetProcessId = GetProcessIdByName(targetProcessName);

        if (targetProcessId == 0)
        {
            Console.WriteLine($"Process {targetProcessName} not found.");
            return;
        }

        Console.WriteLine($"Target process ID: {targetProcessId}");
        targetProcess = Process.GetProcessById((int)targetProcessId);

        Console.CancelKeyPress += new ConsoleCancelEventHandler(OnCancelKeyPress);

        Console.WriteLine("Press CTRL+C to send Control + Break signal to the Receiver process.");
        while (true)
        {
            System.Threading.Thread.Sleep(1000);
        }
    }

    private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
    {
        e.Cancel = true; // Uygulamanın kapanmasını engelle
        SendCtrlBreakSignal();
    }

    private static void SendCtrlBreakSignal()
    {
        // Control + Break sinyalini gönder
        if (GenerateConsoleCtrlEvent(CTRL_BREAK_EVENT, (uint)targetProcess.Id))
        {
            Console.WriteLine("Control + Break signal sent successfully.");
        }
        else
        {
            Console.WriteLine("Failed to send Control + Break signal. Error: " + Marshal.GetLastWin32Error());
        }
    }

    private static uint GetProcessIdByName(string processName)
    {
        Process[] processes = Process.GetProcessesByName(processName);
        if (processes.Length == 0)
        {
            return 0; // Process bulunamadı
        }

        // İlk eşleşen işlemi döndür (birden fazla aynı isimli işlem olabilir)
        return (uint)processes[0].Id;
    }
}
