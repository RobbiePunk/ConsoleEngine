using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using static ConsoleEngine.Program;

using System.Runtime.InteropServices;

namespace ConsoleEngine
{
    public static class Controller
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetKeyboardState(byte[] lpKeyState);

        public static bool isKeyProcessed = true;
        public static ConsoleKey keyToProcess;

        public static Vector3 RotateAroundY(Vector3 vector, float angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            // Матрица поворота вокруг оси Y
            float x = vector.X * cos + vector.Z * sin;
            float z = -vector.X * sin + vector.Z * cos;

            return new Vector3(x, vector.Y, z);
        }

        public static Vector3 RotateAroundX(Vector3 vector, float angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            // Матрица поворота вокруг оси X
            float y = vector.Y * cos - vector.Z * sin;
            float z = vector.Y * sin + vector.Z * cos;

            return new Vector3(vector.X, y, z);
        }

        public static Vector3 RotateAroundZ(Vector3 vector, float angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            // Матрица поворота вокруг оси Z
            float x = vector.X * cos - vector.Y * sin;
            float y = vector.X * sin + vector.Y * cos;

            return new Vector3(x, y, vector.Z);
        }

        public static void MonitorKeysPress()
        {
            // Ожидаем нажатия клавиши
            while (true)
            {
                if(!isKeyProcessed)
                {
                    while (Console.KeyAvailable)
                        Console.ReadKey(true);
                    Thread.Sleep(100);
                    continue;
                }
                
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                //byte[] keyState = new byte[256];
                //GetKeyboardState(keyState);

                keyToProcess = keyInfo.Key;

                isKeyProcessed = false;
            }
        }

        public static void ProcessKey(ConsoleKey key)
        {
            if (ConsoleKey.D == key)
            {
                Camera.position += RotateAroundY(new Vector3(0.1f, 0, 0), Camera.angle.Y);
            }
            if (ConsoleKey.A == key)
            {
                Camera.position -= RotateAroundY(new Vector3(0.1f, 0, 0), Camera.angle.Y);
            }
            if (ConsoleKey.W == key)
            {
                Camera.position += RotateAroundY(new Vector3(0, 0, 0.1f), Camera.angle.Y);
            }
            if (ConsoleKey.S == key)
            {
                Camera.position -= RotateAroundY(new Vector3(0, 0, 0.1f), Camera.angle.Y);
            }
            if (ConsoleKey.RightArrow == key)
            {
                Camera.angle.Y += 0.1f;
            }
            if (ConsoleKey.LeftArrow == key)
            {
                Camera.angle.Y -= 0.1f;
            }
            if (ConsoleKey.UpArrow == key)
            {
                Camera.angle.X -= 0.1f;
            }
            if (ConsoleKey.DownArrow == key)
            {
                Camera.angle.X += 0.1f;
            }
        }

    }
}
