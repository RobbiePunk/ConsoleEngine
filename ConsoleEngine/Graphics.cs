using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleEngine
{
    public static class Graphics
    {
        public static Dictionary<ConsoleColor, Vector3> colors = new Dictionary<ConsoleColor, Vector3>()
        {
            { ConsoleColor.Black, new Vector3(12, 12, 12) },
            { ConsoleColor.DarkBlue, new Vector3(0,55,218) },
            { ConsoleColor.DarkGreen, new Vector3(19,161,14) },
            { ConsoleColor.DarkCyan, new Vector3(58,150,221) },
            { ConsoleColor.DarkRed, new Vector3(197,15,31) },
            { ConsoleColor.DarkMagenta, new Vector3(136,23,152) },
            { ConsoleColor.DarkYellow, new Vector3(193,156,0) },
            { ConsoleColor.Gray, new Vector3(204,204,204) },
            { ConsoleColor.DarkGray, new Vector3(118,118,118) },
            { ConsoleColor.Blue, new Vector3(59,120,255) },
            { ConsoleColor.Green, new Vector3(22,198,12) },
            { ConsoleColor.Cyan, new Vector3(97,214,214) },
            { ConsoleColor.Red, new Vector3(231,72,86) },
            { ConsoleColor.Magenta, new Vector3(180,0,158) },
            { ConsoleColor.Yellow, new Vector3(249,241,165) },
            { ConsoleColor.White, new Vector3(242,242,242) }
        };

        public static Dictionary<int, float> charsIntensity = new Dictionary<int, float>()
        {
            { 32, 0.13129327f},
            { 33, 0.36994612f},
            { 34, 0.31028292f},
            { 35, 0.7076673f},
            { 36, 0.7594665f},
            { 37, 0.7807504f},
            { 38, 0.81348825f},
            { 39, 0.22181381f},
            { 40, 0.45448327f},
            { 41, 0.4509787f},
            { 42, 0.40721425f},
            { 43, 0.4092657f},
            { 44, 0.28395587f},
            { 45, 0.21454823f},
            { 46, 0.21232583f},
            { 47, 0.39576027f},
            { 48, 0.7510898f},
            { 49, 0.5217539f},
            { 50, 0.55474824f},
            { 51, 0.52354896f},
            { 52, 0.6095393f},
            { 53, 0.557398f},
            { 54, 0.64193517f},
            { 55, 0.464997f},
            { 56, 0.7091204f},
            { 57, 0.6319343f},
            { 58, 0.2687409f},
            { 59, 0.35276514f},
            { 60, 0.39174286f},
            { 61, 0.3634498f},
            { 62, 0.3913155f},
            { 63, 0.45679116f},
            { 64, 0.99999994f},
            { 65, 0.6338148f},
            { 66, 0.7133943f},
            { 67, 0.523378f},
            { 68, 0.69518757f},
            { 69, 0.57304037f},
            { 70, 0.4974784f},
            { 71, 0.6591161f},
            { 72, 0.6470638f},
            { 73, 0.5132062f},
            { 74, 0.47995552f},
            { 75, 0.61090684f},
            { 76, 0.42918196f},
            { 77, 0.71347976f},
            { 78, 0.7351055f},
            { 79, 0.65749204f},
            { 80, 0.608428f},
            { 81, 0.7794683f},
            { 82, 0.7133088f},
            { 83, 0.56680053f},
            { 84, 0.45149156f},
            { 85, 0.6077442f},
            { 86, 0.59073424f},
            { 87, 0.7354474f},
            { 88, 0.640653f},
            { 89, 0.517651f},
            { 90, 0.5388495f},
            { 91, 0.5325241f},
            { 92, 0.39593124f},
            { 93, 0.5325241f},
            { 94, 0.3142149f},
            { 95, 0.28062224f},
            { 96, 0.20300879f},
            { 97, 0.5855201f},
            { 98, 0.64834595f},
            { 99, 0.42636117f},
            { 100, 0.65210694f},
            { 101, 0.57244205f},
            { 102, 0.5320112f},
            { 103, 0.81049657f},
            { 104, 0.60107696f},
            { 105, 0.49448666f},
            { 106, 0.57363874f},
            { 107, 0.6115907f},
            { 108, 0.49858958f},
            { 109, 0.7174117f},
            { 110, 0.5279083f},
            { 111, 0.5451748f},
            { 112, 0.65176505f},
            { 113, 0.65210694f},
            { 114, 0.43747327f},
            { 115, 0.48183602f},
            { 116, 0.4974784f},
            { 117, 0.5279083f},
            { 118, 0.45456874f},
            { 119, 0.5813317f},
            { 120, 0.51867676f},
            { 121, 0.5533806f},
            { 122, 0.4516625f},
            { 123, 0.4879049f},
            { 124, 0.42807072f},
            { 125, 0.5132062f},
            { 126, 0.35387638f}
        };

        public static char GetColorChar(float color)
        {
            //return charSet[(int)Math.Floor(color * (charSet.Length - 1))];
            return (char)charsIntensity.OrderBy(pair => Math.Abs(pair.Value - color)).FirstOrDefault().Key;
        }

    }
}
