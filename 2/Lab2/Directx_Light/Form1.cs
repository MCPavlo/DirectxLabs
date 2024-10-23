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
            device.Transform.Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4, (float)this.Width / this.Height, 1f, 50f);
            device.Transform.View = Matrix.LookAtLH(new Vector3(10f, -10f, 15f),
                                        new Vector3(0, 0, 0),
                                        new Vector3(0, 1, 0));

            device.RenderState.Lighting = true;
            device.Lights[0].Type = LightType.Directional;
            device.Lights[0].Diffuse = Color.White;
            device.Lights[0].Direction = new Vector3(10, 0, -20);
            device.Lights[0].Enabled = true;
        }

        public void VertexDeclaration()
        {
            vb = new VertexBuffer(typeof(CustomVertex.PositionNormalColored), 5, device, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionNormalColored.Format, Pool.Default);

            vertices = new CustomVertex.PositionNormalColored[5];

            vertices[0] = new CustomVertex.PositionNormalColored(0f, 0f, 0f, -1f, 1f, -1f, Color.Cyan.ToArgb());
            vertices[1] = new CustomVertex.PositionNormalColored(3f, 0f, 0f, 1f, 1f, -1f , Color.Red.ToArgb());
            vertices[2] = new CustomVertex.PositionNormalColored(2f, -4f, 0f, -1f, -1f, -1f, Color.Blue.ToArgb());
            vertices[3] = new CustomVertex.PositionNormalColored(5f, -4f, 0f, 1f, -1f, -1f, Color.Magenta.ToArgb());
            vertices[4] = new CustomVertex.PositionNormalColored(0f, 0f, 5f, 0f, 0f, 1f, Color.Green.ToArgb());

            vb.SetData(vertices, 0, LockFlags.None);

            ib = new IndexBuffer(typeof(int), 18, device, Usage.WriteOnly, Pool.Default);
            indices = new int[]
            {
                //bok
                0, 2, 4,
                2, 3, 4,
                3, 1, 4,
                1, 0, 4,
                //dno
                2, 0, 1,
                2, 1, 3

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

            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 5, 0, 6);

            //x^2 / 100 + y^2 / 64 = 1 - ellipse. x = 10cost, y = 8sint
            //z = y - x - 10 = 8sint - 10cost - 10
            device.Lights[0].Direction = new Vector3(10 * (float)Math.Cos(angle), 8 * (float)Math.Sin(angle), 8 * (float)Math.Sin(angle) - 10 * (float)Math.Cos(angle) - 10);
            device.Lights[0].Enabled = true;

            device.EndScene();

            device.Present();

            this.Invalidate();
            angle += 0.1f;
        }
    }
}

