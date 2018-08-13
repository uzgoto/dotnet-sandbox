using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Uzgoto.Dotnet.Sandbox.Winapi;

namespace Uzgoto.Dotnet.Sandbox.ConsolePopup
{
    public class Program
    {
        static void Main(string[] args)
        {
            var flag = true;
            var sw = new Stopwatch();
            while (true)
            {
                sw.Start();
                if (flag)
                {

                    Console.WriteLine($"Elapsed (1-1-1): {sw.ElapsedMilliseconds}");
                    var dialogs = Dialog.EnumerateChilds();
                    foreach(var dialog in dialogs)
                    {
                        dialog.Close();
                    }
                    Console.WriteLine($"Elapsed (1-1-2): {sw.ElapsedMilliseconds}");
                    
                    Task.Factory.StartNew(() =>
                    {
                        Console.WriteLine($"Elapsed (1-2-1): {sw.ElapsedMilliseconds}");
                        MessageBox.Show(
                            $"{sw.ElapsedMilliseconds}",
                            $"{Process.GetCurrentProcess().ProcessName}",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning,
                            MessageBoxDefaultButton.Button1,
                            MessageBoxOptions.ServiceNotification);
                        Console.WriteLine($"Elapsed (1-2-2): {sw.ElapsedMilliseconds}");
                    }).ConfigureAwait(true);

                    Console.WriteLine($"Elapsed (1-3-1): {sw.ElapsedMilliseconds}");
                    Task.Delay(5000).Wait();
                    flag = false;
                    Console.WriteLine($"Elapsed (1-3-2): {sw.ElapsedMilliseconds}");
                }
                else
                {
                    Console.WriteLine($"Elapsed (2-1-1): {sw.ElapsedMilliseconds}");
                    var dialogs = Dialog.EnumerateChilds();
                    foreach (var dialog in dialogs)
                    {
                        dialog.Close();
                    }
                    Console.WriteLine($"Elapsed (2-1-2): {sw.ElapsedMilliseconds}");
                    
                    Task.Factory.StartNew(() =>
                    {
                        Console.WriteLine($"Elapsed (2-2-1): {sw.ElapsedMilliseconds}");
                        MessageBox.Show(
                            $"{sw.ElapsedMilliseconds}",
                            $"{Process.GetCurrentProcess().ProcessName}",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information,
                            MessageBoxDefaultButton.Button1,
                            MessageBoxOptions.ServiceNotification);
                        Console.WriteLine($"Elapsed (2-2-2): {sw.ElapsedMilliseconds}");
                    }).ConfigureAwait(true);

                    Console.WriteLine($"Elapsed (2-3-1): {sw.ElapsedMilliseconds}");
                    Task.Delay(5000).Wait();
                    flag = true;
                    Console.WriteLine($"Elapsed (2-3-2): {sw.ElapsedMilliseconds}");
                }
            }
        }
    }
}
