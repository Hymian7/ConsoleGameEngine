using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleGameEngine
{
    class Program
    {
             


        [STAThread]
        static async Task Main(string[] args)
        {

            Game game = new Game();
            game.ConstructConsole(120, 30);
            await game.Start();

        }
    }
}
