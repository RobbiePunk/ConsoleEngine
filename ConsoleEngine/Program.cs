using System;
using System.Drawing;
using System.Numerics;
using System.Text;
using System.Timers;

using static ConsoleEngine.Program;
using static ConsoleEngine.Graphics;
using static ConsoleEngine.Controller;

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleEngine
{
    public enum LIGHT_TYPES { ambient, point, directional };

    internal class Program
    {
        public static bool logInfo = true;
        public static int infoSize = 3;

        private static bool isRunning = true;

        public static Stopwatch prepTime = new Stopwatch();
        public static string prepTimeStr;
        public static Stopwatch drawTime = new Stopwatch();
        public static string drawTimeStr;

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

            public Sphere(Vector3 center, double radius, float color, float specular)
            {
                this.center = center;
                this.radius = radius;
                this.color = color;
                this.specular = specular;
            }
        }

        public class Cube
        {
            public Vector3 center { get; set; }

            public double size { get; set; }

            public float color { get; set; }

            public float specular { get; set; }

            public Vector3[] points = new Vector3[8];

            public Cube(Vector3 center, double size, float color, float specular)
            {
                this.center = center;
                this.size = size;
                this.color = color;
                this.specular = specular;
            }

            public Cube(Vector3[] points, float color, float specular)
            {
                this.color = color;
                this.specular = specular;

                points.CopyTo(this.points, 0);
            }

        }

        public class Polygon
        {
            public Vector3[] points;

            public Polygon(Vector3[] points)
            {
                this.points = points;
            }
        }


        public class Torus
        {
            public Vector3 center { get; set; }

            public double bigRadius { get; set; }

            public double smallRadius { get; set; }

            public float color { get; set; }

            public float specular { get; set; }

            public Torus(Vector3 center, double R, double r, float color, float specular)
            {
                this.center = center;
                this.bigRadius = R;
                this.smallRadius = r;
                this.color = color;
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
            public static string image;

            static Canvas()
            {
                width = 150;
                height = 50;

                canvas = new char[width * height];
                image = new string(canvas);
            }

            public static void PutPixel(int x, int y, char color)
            {
                x = x + width / 2;
                y = y + height / 2;

                if (x < width && y < height)
                    canvas[width * y + x] = color;
            }

            public static string GetImage()
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        sb.Append(canvas[i * width + j].ToString());
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

        static Vector3 CanvasToViewPort(int x, int y)
        {
            Vector3 point = new Vector3(x * (float)Viewport.width / (float)Canvas.width,
                -y * 0.7f * (float)Viewport.height / (float)Canvas.height,
                (float)Viewport.distance);

            point = RotateAroundX(point, Camera.angle.X);
            point = RotateAroundY(point, Camera.angle.Y);
            //point = RotateAroundZ(point, Camera.angle.Z);

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

        static Vector2 IntersectRayPolygon(Vector3 O, Vector3 D, Polygon polygon)
        {
            Vector3[] points = polygon.points;
            Vector3 N = (points[2] - points[1]) * (points[0] - points[1]);

            Vector3 Q = points[0];

            float t = Vector3.Dot(N, (Q - O)) / Vector3.Dot(N, D);

            return new Vector2((float)t, (float)t);
        }

        static Vector2 IntersectRayTorus(Vector3 O, Vector3 D, Torus torus)
        {
            return new Vector2(0, 0);
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

            Console.OutputEncoding = Encoding.UTF8;

            if (!logInfo)
                infoSize = 0;

            Console.SetWindowSize(Canvas.width, Canvas.height + infoSize);
            Console.SetBufferSize(Canvas.width, Canvas.height + infoSize);

            spheres.Add(new Sphere(new Vector3(0, 0, 5), 1, 1, 500));
            spheres.Add(new Sphere(new Vector3(2, -1, 8), 1, 1, 500));
            spheres.Add(new Sphere(new Vector3(-2, -1, 5), 1, 1, 10));

            spheres.Add(new Sphere(new Vector3(0, -10001, 0), 10000, 0.4f, 1000));

            lights.Add(new Light(0.2f));
            lights.Add(new Light(0.2f, new Vector3(2, 1, 0)));
            lights.Add(new Light(new Vector3(1, 4, 4), 0.1f));


            Thread keyPressThread = new Thread(MonitorKeysPress);
            keyPressThread.Start();

            while (true)
            {
                prepTime.Reset();
                prepTime.Start();

                PrepareFrame();

                if (!isKeyProcessed)
                {
                    ProcessKey(keyToProcess);
                    isKeyProcessed = true;
                }

                prepTime.Stop();

                prepTimeStr = string.Format("{0:00}:{1:00}", prepTime.Elapsed.Seconds, prepTime.Elapsed.Milliseconds);

                drawTime.Reset();
                drawTime.Start();

                DrawFrame();

                drawTime.Stop();
                drawTimeStr = string.Format("{0:00}:{1:00}", drawTime.Elapsed.Seconds, drawTime.Elapsed.Milliseconds);
            }
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

            if (logInfo)
            {
                Console.WriteLine("Prepare Time: " + string.Format("{0:00}:{1:00}", prepTime.Elapsed.Seconds, prepTime.Elapsed.Milliseconds));
                Console.WriteLine("Draw Time: " + string.Format("{0:00}:{1:00}", drawTime.Elapsed.Seconds, drawTime.Elapsed.Milliseconds));
                Console.Write($"Position: {Camera.position.X}, {Camera.position.Y}, {Camera.position.Z}");
            }

        }

    }
}
