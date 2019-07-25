using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;

using PerlinNoise.Noise;
using PerlinNoise.Render;

namespace PerlinNoise.Engine
{
    class Game
    {
        public Display Display { get; private set; }
        public Renderer Renderer { get; private set; }

        public PerlinNoiseGenerator PNGenerator { get; private set; }

        public float[,] PerlinTestArray { get; private set; }
        public int Mode { get; private set; }

        public int Octaves { get; private set; }
        public float Bias { get; private set; }
        public int Height { get; private set; }
        public int Resolution { get; private set; }

        public Game(int width, int height)
        {
            this.Display = new Display(width, height);
            this.Renderer = new Renderer();

            this.PNGenerator = new PerlinNoiseGenerator("123456");

            this.Mode = 2;
            this.Octaves = 8;
            this.Bias = 1.0f;
            this.Height = 20;
            this.Resolution = 256;

            this.Init();

            this.GeneratePerlinNoise();
        }

        private void Init()
        {
            this.Display.Window.RenderFrame += RenderFrame;
            this.Display.Window.KeyPress += KeyPressed;
        }

        private void KeyPressed(object sender, KeyPressEventArgs e)
        {
            switch(e.KeyChar)
            {
                case '1':
                    this.Mode = 1;
                    break;
                case '2':
                    this.Mode = 2;
                    break;
                case 'o':
                    Octaves += 1;
                    if (Octaves > 32) this.Octaves = 2;
                    Console.WriteLine($"Octaves set to: {this.Octaves}");
                    this.GeneratePerlinNoise();
                    break;
                case 'b':
                    Bias /= 2;
                    if (Bias < 0.1f) this.Bias = 2.0f;
                    Console.WriteLine($"Bias set to: {this.Bias}");
                    this.GeneratePerlinNoise();
                    break;
                case 'h':
                    this.Height += 10;
                    if (this.Height > 160) this.Height = 0;
                    Console.WriteLine($"Height set to: {this.Height}");
                    break;
                case 'r':
                    this.Resolution *= 2;
                    if (this.Resolution > 1024) this.Resolution = 32;
                    Console.WriteLine($"Resolution set to: {this.Resolution}");
                    this.GeneratePerlinNoise();
                    break;
            }
        }

        public void Run()
        {
            this.Display.Run();
        }

        private void CreatePerspectiveProjection(float angle)
        {
            GL.MatrixMode(MatrixMode.Projection);
            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(angle), (float)Display.Width / (float)Display.Height, 1.0f, 10000.0f);
            GL.LoadMatrix(ref perspective);
        }

        private void GeneratePerlinNoise()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            this.PerlinTestArray = new float[this.Display.Width, this.Display.Height];
            for (int y = 0; y < this.Display.Height; y++)
            {
                for (int x = 0; x < this.Display.Width; x++)
                {
                    this.PerlinTestArray[x, y] = this.PNGenerator.GetXZ(new VectorXZ(x, y), this.Octaves, this.Resolution, this.Bias);
                }
            }
            stopwatch.Stop();
            Console.WriteLine($"Perlin noise generation done! {stopwatch.ElapsedMilliseconds}ms elapsed...");
        }

        private void Render3D()
        {
            this.CreatePerspectiveProjection(45f);
            GL.MatrixMode(MatrixMode.Modelview);
            var lookAt = Matrix4.LookAt(new Vector3(this.Display.Width / 2, this.Display.Width / 3 + this.Height*2, -this.Display.Width / 2), new Vector3(this.Display.Width / 2, this.Display.Width / 6 + this.Height, 0), Vector3.UnitY);
            GL.LoadMatrix(ref lookAt);

            GL.Begin(PrimitiveType.Quads);

            if (this.PerlinTestArray != null)
            {
                for (int y = 1; y < this.PerlinTestArray.GetLength(1) - 1; y++)
                {
                    for (int x = 1; x < this.PerlinTestArray.GetLength(0) - 1; x++)
                    {
                        float corner1 = this.PerlinTestArray[x, y];
                        float corner2 = this.PerlinTestArray[x, y - 1];
                        float corner3 = this.PerlinTestArray[x - 1, y - 1];
                        float corner4 = this.PerlinTestArray[x - 1, y];

                        GL.Color3(corner1, corner1, corner1);
                        GL.Vertex3(x, corner1 * this.Height, y);

                        GL.Color3(corner2, corner2, corner2);
                        GL.Vertex3(x, corner2 * this.Height, y - 1);

                        GL.Color3(corner3, corner3, corner3);
                        GL.Vertex3(x - 1, corner3 * this.Height, y - 1);

                        GL.Color3(corner4, corner4, corner4);
                        GL.Vertex3(x - 1, corner4 * this.Height, y);
                    }
                }
            }

            GL.End();
        }

        private void Render2D()
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.MatrixMode(MatrixMode.Projection);
            GL.Ortho(0, this.Display.Width, this.Display.Height, 0, -1f, 1.0f);

            GL.Begin(PrimitiveType.Quads);
            for (int y = 1; y < this.Display.Height - 1; y++)
            {
                for (int x = 1; x < this.Display.Width - 1; x++)
                {
                    float noiseVal = this.PerlinTestArray[x, y];
                    GL.Color3(noiseVal, noiseVal, noiseVal);

                    GL.Vertex2(x, y);
                    GL.Vertex2(x, y - 1);
                    GL.Vertex2(x - 1, y - 1);
                    GL.Vertex2(x - 1, y);
                }
            }

            GL.End();
        }
        private void RenderFrame(object sender, OpenTK.FrameEventArgs e)
        {
            this.Renderer.Prepare();

            switch(this.Mode)
            {
                case 1:
                    this.Render3D();
                    break;
                case 2:
                    this.Render2D();
                    break;
            }

            this.Display.Update();
        }
    }
}
