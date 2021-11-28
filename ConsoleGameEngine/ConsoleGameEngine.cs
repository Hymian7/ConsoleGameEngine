using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleGameEngine
{

    #region Structs
    [StructLayout(LayoutKind.Sequential)]
    public struct Coord
    {
        public short X;
        public short Y;

        public Coord(short X, short Y)
        {
            this.X = X;
            this.Y = Y;
        }
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct CharUnion
    {
        [FieldOffset(0)] public char UnicodeChar;
        [FieldOffset(0)] public byte AsciiChar;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct CharInfo
    {
        [FieldOffset(0)] public CharUnion Char;
        [FieldOffset(2)] public short Attributes;
        public CharInfo(byte asciiChar, short attribute)
        {
            Char = new CharUnion() { AsciiChar = asciiChar };
            Attributes = attribute;
        }

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SmallRect
    {
        public short Left;
        public short Top;
        public short Right;
        public short Bottom;
    }

    public struct KeyState
    {
        public bool isPressed;
        public bool isHeld;

    }

    #endregion

    public abstract class ConsoleGameEngine
    {
        #region DLLImport
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern SafeFileHandle CreateFile(
        string fileName,
        [MarshalAs(UnmanagedType.U4)] uint fileAccess,
        [MarshalAs(UnmanagedType.U4)] uint fileShare,
        IntPtr securityAttributes,
        [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
        [MarshalAs(UnmanagedType.U4)] int flags,
        IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteConsoleOutput(
          SafeFileHandle hConsoleOutput,
          CharInfo[] lpBuffer,
          Coord dwBufferSize,
          Coord dwBufferCoord,
          ref SmallRect lpWriteRegion);

        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(ConsoleKey vKey); // Keys enumeration
        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(System.Int32 vKey);

        #endregion        

        public short ScreenWidth { get; private set; }
        public short ScreenHeight { get; private set; }

        private SafeFileHandle h { get; set; }

        protected CharInfo[] Screen { get; set; }

        protected KeyState[] KeyStates { get; set; }

        public ConsoleGameEngine()
        {
            KeyStates = new KeyState[256];
        }

        public void ConstructConsole(short width, short height)
        {
            ScreenWidth = width;
            ScreenHeight = height;

#pragma warning disable CA1416 // Plattformkompatibilität überprüfen
            Console.SetBufferSize(ScreenWidth, ScreenHeight);
#pragma warning restore CA1416 // Plattformkompatibilität überprüfen
#pragma warning disable CA1416 // Plattformkompatibilität überprüfen
            Console.SetWindowSize(ScreenWidth, ScreenHeight);
#pragma warning restore CA1416 // Plattformkompatibilität überprüfen

            h = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

            if (h.IsInvalid) return;
            Console.CursorVisible = false;

             Screen = new CharInfo[ScreenHeight * ScreenWidth];
        }

        public async Task Start()
        {

            await Task.Run(() => GameThread());

        }

        private void GameThread()
        {

            if (!OnUserCreate()) return;

            var tp1 = DateTime.Now;
            var tp2 = DateTime.Now;

            bool isQuit = false;

            while (!isQuit)
            {
                // Timing
                tp2 = DateTime.Now;
                TimeSpan elapsedTime = tp2 - tp1;
                tp1 = tp2;
                float fElapsedTime = (float)elapsedTime.TotalMilliseconds;


                

                //Get input
                for (int i = 0; i < 256; i++)
                {
                    if (Convert.ToBoolean(GetAsyncKeyState(i) & 0x8000))
                    {
                        if (KeyStates[i].isPressed) KeyStates[i].isHeld = true;
                        KeyStates[i].isPressed = true;
                    }
                    else 
                    {
                        KeyStates[i].isPressed = false;
                        KeyStates[i].isHeld = false;
                    }
                }


                isQuit= !OnUserUpdate(fElapsedTime);

                //Screen[ScreenHeight * ScreenWidth - 1].Char.AsciiChar = (byte)'\0';
                Console.Title = "olcGameEngine - FPS:" + (int)(10000000 / elapsedTime.Ticks);

                SmallRect rect = new SmallRect() { Left = 0, Top = 0, Right = ScreenWidth, Bottom = ScreenHeight };
                WriteConsoleOutput(h, Screen, new Coord() { X = ScreenWidth, Y = ScreenHeight }, new Coord() { X = 0, Y = 0 }, ref rect);

            }


        }

        public virtual bool OnUserCreate() { return false; }

        public virtual bool OnUserUpdate(float fElapsedTime) { return false; }
    }
}
