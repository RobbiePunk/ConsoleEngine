using System;
using System.Drawing;
using System.Numerics;
using System.Text;
using static ConsoleEngine.Program;

namespace ConsoleEngine
{
    public enum LIGHT_TYPES { ambient, point, directional };

    internal class Program
    {
        static Dictionary<ConsoleColor, Vector3> colors = new Dictionary<ConsoleColor, Vector3>()
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

        static char[] charSet = { ' ', '.', ',', ':', ';', 'o', '*', 'p', '0', '%', '#', '$', '@' };

        public static char GetColorChar(float color)
        {
            return charSet[(int)Math.Floor(color * (charSet.Length - 1))];
        }

        public class Light
        {
            public LIGHT_TYPES type { get; set; }
            public float intensity { get; set; }

            public Vector3 position;
            public Vector3 direction;

            public Light(float intensity)
            {
                type = LIGHT_TYPES.ambient;
                this.intensity = intensity;
            }

            public Light(float intensity, Vector3 position)
            {
                type = LIGHT_TYPES.point;
                this.intensity = intensity;
                this.position = position;
            }

            public Light(Vector3 direction, float intensity)
            {
                type = LIGHT_TYPES.directional;
                this.intensity = intensity;
                this.direction = direction;
            }

        }

        public class Sphere
        {
            public Vector3 center { get; set; }

            public double radius { get; set; }

            public float color { get; set; }

            public float specular { get; set; }

            public Vector3 color3 { get; set; }

            public Sphere(Vector3 center, double radius, float color, float specular)
            {
                this.center = center;
                this.radius = radius;
                this.color = color;
                this.specular = specular;
            }

            public Sphere(Vector3 center, double radius, Vector3 color, float specular)
            {
                this.center = center;
                this.radius = radius;
                this.color3 = color;
                this.specular = specular;
            }

        }

        public static List<Sphere> spheres = new List<Sphere>();
        public static List<Light> lights = new List<Light>();

        public static class Canvas
        {
            public static int width { get; set; }
            public static int height { get; set; }

            public static char[] canvas;
            public static Vector3[] canvas3;

            static Canvas()
            {
                width = 150;
                height = 50;

                canvas = new char[width * height];
                canvas3 = new Vector3[width * height];
            }

            public static void PutPixel3(int x, int y, Vector3 color)
            {
                x = x + width / 2;
                y = y + height / 2;

                if (x < width && y < height)
                    canvas3[width * y + x] = color;
            }

            public static void PutPixel(int x, int y, char color)
            {
                x = x + width / 2;
                y = y + height / 2;

                if (x < width && y < height)
                    canvas[width * y + x] = color;
            }

            public static Vector3 GetPixel3(int x, int y)
            {
                return canvas3[width * y + x];
            }

            public static string GetImage()
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        sb.Append(canvas[width * i + j].ToString());
                    }
                }

                return sb.ToString();

            }

        }

        public static class Viewport
        {
            public static int width { get; set; }
            public static int height { get; set; }

            public static double distance { get; set; }

            static Viewport()
            {
                width = 1;
                height = 1;

                distance = 1;
            }
        }

        public static class Camera
        {
            public static Vector3 position;
            public static Vector3 angle;

            static Camera()
            {
                position = new Vector3(0, 1.5f, 0);
                angle = new Vector3(0, 0, 0);
            }

        }

        static Vector3 RotateAroundY(Vector3 vector, float angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            // Матрица поворота вокруг оси Y
            float x = vector.X * cos + vector.Z * sin;
            float z = -vector.X * sin + vector.Z * cos;

            return new Vector3(x, vector.Y, z);
        }

        static Vector3 RotateAroundX(Vector3 vector, float angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            // Матрица поворота вокруг оси X
            float y = vector.Y * cos - vector.Z * sin;
            float z = vector.Y * sin + vector.Z * cos;

            return new Vector3(vector.X, y, z);
        }

        static Vector3 RotateAroundZ(Vector3 vector, float angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            // Матрица поворота вокруг оси Z
            float x = vector.X * cos - vector.Y * sin;
            float y = vector.X * sin + vector.Y * cos;

            return new Vector3(x, y, vector.Z);
        }

        static Vector3 CanvasToViewPort(int x, int y)
        {
            Vector3 point = new Vector3(x * (float)Viewport.width / (float)Canvas.width,
                -y * 0.7f * (float)Viewport.height / (float)Canvas.height,
                (float)Viewport.distance);

            point = RotateAroundX(point, Camera.angle.X);
            point = RotateAroundY(point, Camera.angle.Y);
            //point = RotateAroundZ(point, Camera.angle.Z);

            //point += Camera.position;

            return point;
        }

        static Vector2 IntersectRaySphere(Vector3 O, Vector3 D, Sphere sphere)
        {
            Vector3 C = sphere.center;
            double r = sphere.radius;
            Vector3 oc = O - C;

            double k1 = Vector3.Dot(D, D);
            double k2 = 2 * Vector3.Dot(oc, D);
            double k3 = Vector3.Dot(oc, oc) - r * r;

            double discriminant = k2 * k2 - 4 * k1 * k3;

            if (discriminant < 0)
                return new Vector2(float.MaxValue, float.MaxValue);

            double t1 = (-k2 + Math.Sqrt(discriminant)) / (2 * k1);
            double t2 = (-k2 - Math.Sqrt(discriminant)) / (2 * k1);

            return new Vector2((float)t1, (float)t2);
        }

        static Vector3 TraceRayColored(Vector3 O, Vector3 D, float t_min, float t_max)
        {
            float closest_t = t_max;
            Sphere closest_sphere = null;

            foreach (Sphere sphere in spheres)
            {
                Vector2 tvec = IntersectRaySphere(O, D, sphere);
                float t1 = tvec.X;
                float t2 = tvec.Y;

                if (t1 < closest_t && t1 > t_min && t1 < t_max)
                {
                    closest_t = t1;
                    closest_sphere = sphere;
                }

                if (t2 < closest_t && t2 > t_min && t2 < t_max)
                {
                    closest_t = t2;
                    closest_sphere = sphere;
                }
            }

            if (closest_sphere == null)
                return Vector3.Zero;

            Vector3 P = O + closest_t * D;
            Vector3 N = P - closest_sphere.center;
            N /= N.Length();

            Vector3 color = closest_sphere.color3 * ComputeLighting(P, N, -D, closest_sphere.specular);

            if (color.X < 0)
                color.X = 0f;
            if (color.Y < 0)
                color.Y = 0f;
            if (color.Z < 0)
                color.Z = 0f;

            if (color.X > 1)
                color.X = 1f;
            if (color.Y > 1)
                color.Y = 1f;
            if (color.Z > 1)
                color.Z = 1f;

            return color * 255;
        }

        static float TraceRay(Vector3 O, Vector3 D, float t_min, float t_max)
        {
            float closest_t = t_max;
            Sphere closest_sphere = null;

            foreach (Sphere sphere in spheres)
            {
                Vector2 tvec = IntersectRaySphere(O, D, sphere);
                float t1 = tvec.X;
                float t2 = tvec.Y;

                if (t1 < closest_t && t1 > t_min && t1 < t_max)
                {
                    closest_t = t1;
                    closest_sphere = sphere;
                }

                if (t2 < closest_t && t2 > t_min && t2 < t_max)
                {
                    closest_t = t2;
                    closest_sphere = sphere;
                }
            }

            if (closest_sphere == null)
                return 0;

            Vector3 P = O + closest_t * D;
            Vector3 N = P - closest_sphere.center;
            N /= N.Length();

            float color = closest_sphere.color * ComputeLighting(P, N, -D, closest_sphere.specular);

            if (color < 0)
                return 0;

            if (color > 1)
                return 1;

            return color;
        }

        static float ComputeLighting(Vector3 P, Vector3 N, Vector3 V, float s)
        {
            float i = 0f;

            foreach (Light light in lights)
            {
                if (light.type == LIGHT_TYPES.ambient)
                {
                    i += light.intensity;
                }
                else
                {
                    Vector3 L;
                    if (light.type == LIGHT_TYPES.point)
                        L = light.position - P;
                    else
                        L = light.direction;

                    //Диффузность
                    float nDotL = Vector3.Dot(N, L);
                    if (nDotL > 0)
                        i += light.intensity * nDotL / (N.Length() * N.Length());

                    //зеркальность
                    if (s != -1f)
                    {
                        Vector3 R = 2 * N * Vector3.Dot(N, L) - L;
                        float rDotV = Vector3.Dot(R, V);
                        if (rDotV > 0)
                            i += light.intensity * (float)Math.Pow(rDotV / (R.Length() * V.Length()), s);
                    }

                }
            }

            return i;
        }

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetWindowSize(Canvas.width, Canvas.height);
            Console.SetBufferSize(Canvas.width, Canvas.height);

            /*
            spheres.Add(new Sphere(new Vector3(0, 0, 5), 1, new Vector3(0, 0, 255), 500));
            spheres.Add(new Sphere(new Vector3(2, -1, 8), 1, new Vector3(0, 255, 0), 500));
            spheres.Add(new Sphere(new Vector3(-2, -1, 5), 1, new Vector3(255, 0, 0), 10));

            spheres.Add(new Sphere(new Vector3(0, -10001, 0), 10000, new Vector3(100, 100, 100), 1000));

            */

            spheres.Add(new Sphere(new Vector3(0, 0, 5), 1, 1, 500));
            spheres.Add(new Sphere(new Vector3(2, -1, 8), 1, 1, 500));
            spheres.Add(new Sphere(new Vector3(-2, -1, 5), 1, 1, 10));

            spheres.Add(new Sphere(new Vector3(0, -10001, 0), 10000, 0.4f, 1000));

            lights.Add(new Light(0.2f));
            lights.Add(new Light(0.2f, new Vector3(2, 1, 0)));
            lights.Add(new Light(new Vector3(1, 4, 4), 0.1f));

            PrepareFrame();

            DrawFrame();

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.D)
                {
                    Camera.position += RotateAroundY(new Vector3(0.1f, 0, 0), Camera.angle.Y);
                }
                if (keyInfo.Key == ConsoleKey.A)
                {
                    Camera.position -= RotateAroundY(new Vector3(0.1f, 0, 0), Camera.angle.Y);
                }
                if (keyInfo.Key == ConsoleKey.W)
                {
                    Camera.position += RotateAroundY(new Vector3(0, 0, 0.1f), Camera.angle.Y);
                }
                if (keyInfo.Key == ConsoleKey.S)
                {
                    Camera.position -= RotateAroundY(new Vector3(0, 0, 0.1f), Camera.angle.Y);
                }
                if (keyInfo.Key == ConsoleKey.RightArrow)
                {
                    Camera.angle.Y += 0.1f;
                }
                if (keyInfo.Key == ConsoleKey.LeftArrow)
                {
                    Camera.angle.Y -= 0.1f;
                }
                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    Camera.angle.X -= 0.1f;
                }
                if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    Camera.angle.X += 0.1f;
                }

                PrepareFrame();

                DrawFrame();
            }
        }

        static void PrepareFrameColored()
        {
            Vector3 O = Camera.position;

            for (int x = -Canvas.width / 2; x < Canvas.width / 2; x++)
            {
                for (int y = -Canvas.height / 2; y < Canvas.height / 2; y++)
                {
                    Vector3 D = CanvasToViewPort(x, y);
                    //char color = GetColorChar(TraceRay(O, D, 1, 100000));
                    Vector3 color = TraceRayColored(O, D, 1, 100000);
                    Canvas.PutPixel3(x, y, color);
                }
            }

            return;
        }

        static void PrepareFrame()
        {
            Vector3 O = Camera.position;

            for (int x = -Canvas.width / 2; x < Canvas.width / 2; x++)
            {
                for (int y = -Canvas.height / 2; y < Canvas.height / 2; y++)
                {
                    Vector3 D = CanvasToViewPort(x, y);
                    char color = GetColorChar(TraceRay(O, D, 1, 100000));
                    Canvas.PutPixel(x, y, color);
                }
            }

            return;
        }

        static void DrawFrame()
        {
            Console.SetCursorPosition(0, 0);
            Console.Write(Canvas.GetImage());
        }

        static void DrawFrameColored()
        {
            Console.SetCursorPosition(0, 0);

            for (int y = 0; y < Canvas.height; y++)
            {
                for (int x = 0; x < Canvas.width; x++)
                {
                    Vector3 c = Canvas.GetPixel3(x, y);
                    Console.ForegroundColor = ChooseColor(c);
                    Console.Write(ChooseSymbol(c, Console.ForegroundColor));
                    //Console.Write('@');
                }
            }

            //Console.Write(Canvas.GetImage());
        }

        static char ChooseSymbol(Vector3 c, ConsoleColor cc)
        {
            float d = (c - colors[cc]).Length() / 255;

            if (d < 0)
                d = 0;
            if (d > 1)
                d = 1;

            return GetColorChar(d);
        }

        static ConsoleColor ChooseColor(Vector3 color)
        {
            ConsoleColor result = ConsoleColor.Black;
            float minDistance = new Vector3(255, 255, 255).Length();

            foreach (var c in colors)
            {
                if ((color - c.Value).Length() < minDistance)
                {
                    result = c.Key;
                    minDistance = (color - c.Value).Length();
                }
            }

            return result;
        }

    }
}
