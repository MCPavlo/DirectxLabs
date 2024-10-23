using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using Microsoft.VisualC;

namespace Directx_Light
{
    public partial class Form1 : Form
    {
        private Device device = null;
        private VertexBuffer vb = null;
        private float angle = 0f;
        private CustomVertex.PositionNormalColored[] vertices;
        private IndexBuffer ib = null;
        private int[] indices;

        public Form1()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);

            InitializeDevice();
            VertexDeclaration();
            CameraPositioning();
        }

        public void InitializeDevice()
        {
            PresentParameters presentParams = new PresentParameters();
            presentParams.Windowed = true;
            presentParams.SwapEffect = SwapEffect.Discard;

            device = new Device(0, DeviceType.Hardware, this, CreateFlags.SoftwareVertexProcessing, presentParams);
        }

        public void CameraPositioning()
        {
            //3. Камера расположена в точке (20, 20, 20), направлена в начало координат.
                        device.Transform.Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4, (float)this.Width / this.Height, 1f, 50f);
            device.Transform.View = Matrix.LookAtLH(new Vector3(20f, 20f, 20f),
                                        new Vector3(0, 0, 0),
                                        new Vector3(0, 1, 0));

            //2. На сцене присутствует направленный источник света. Его направление – параллельно вектору { 1, 1, 1}. 
            device.RenderState.Lighting = true;
            device.Lights[0].Type = LightType.Directional;
            device.Lights[0].Diffuse = Color.White;
            device.Lights[0].Direction = new Vector3(-1, -1, -1);
            device.Lights[0].Enabled = true;
        }

        public void VertexDeclaration()
        {
            vb = new VertexBuffer(typeof(CustomVertex.PositionNormalColored), 5, device, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionNormalColored.Format, Pool.Default);

            vertices = new CustomVertex.PositionNormalColored[4];

            //1. Соседние вершины фигуры должны иметь разные цвета и обладать нормалями. 

            vertices[0] = new CustomVertex.PositionNormalColored(0f, 0f, 0f, 0f, 0f, 1f, Color.Cyan.ToArgb());
            vertices[1] = new CustomVertex.PositionNormalColored(3f, 0f, 0f, 0f, 0f, 1f, Color.Red.ToArgb());
            vertices[2] = new CustomVertex.PositionNormalColored(2f, -4f, 0f, 0f, 0f, 1f, Color.Blue.ToArgb());
            vertices[3] = new CustomVertex.PositionNormalColored(5f, -4f, 0f, 0f, 0f, 1f, Color.Magenta.ToArgb());

            vb.SetData(vertices, 0, LockFlags.None);

            ib = new IndexBuffer(typeof(int), 6, device, Usage.WriteOnly, Pool.Default);
            indices = new int[]
            {
                0, 2, 1,
                2, 3, 1
            };

            ib.SetData(indices, 0, LockFlags.None);
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            device.Clear(ClearFlags.Target, Color.DarkSlateBlue, 1.0f, 0);

            device.BeginScene();
            device.VertexFormat = CustomVertex.PositionNormalColored.Format;

            device.SetStreamSource(0, vb, 0);
            device.Indices = ib;

            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);

            device.EndScene();

            device.Present();

            this.Invalidate();
        }
    }
}

