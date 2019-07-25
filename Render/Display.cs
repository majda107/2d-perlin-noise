using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace PerlinNoise.Render
{
    class Display
    {
        public GameWindow Window { get; private set; }
        public int Width { get => this.Window.Width; }
        public int Height { get => this.Window.Height; }
        public Display(int width, int height)
        {
            this.Window = new GameWindow(width, height, OpenTK.Graphics.GraphicsMode.Default, "Perlin noise test");
            this.Init();
        }

        private void Init()
        {
            this.Window.Resize += Resized;
            this.Window.Load += Loaded;
        }

        private void Loaded(object sender, EventArgs e)
        {
            GL.Enable(EnableCap.DepthTest);
        }

        private void Resized(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, this.Width, this.Height);
        }

        public void Run()
        {
            this.Window.Run(20.0f);
        }

        public void Update()
        {
            this.Window.SwapBuffers();
        }
    }
}
