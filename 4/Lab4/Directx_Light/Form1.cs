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
using static System.Net.Mime.MediaTypeNames;

namespace Directx_Light
{
    public partial class Form1 : Form
    {
        private Device device = null;
        private VertexBuffer vb = null;
        private float angle = 0f;
        private CustomVertex.PositionNormalTextured[] vertices;
        private IndexBuffer ib = null;
        private int[] indices;
        private int[] indices1;
        private Bitmap b;
        private Texture tex1;

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

            presentParams.EnableAutoDepthStencil = true;
            presentParams.AutoDepthStencilFormat = DepthFormat.D16;

            device = new Device(0, DeviceType.Hardware, this, CreateFlags.SoftwareVertexProcessing, presentParams);

            device.RenderState.Lighting = true;

            device.RenderState.CullMode = Cull.CounterClockwise;

            b = (Bitmap)System.Drawing.Image.FromFile("Logo.bmp");
            tex1 = new Texture(device, b, 0, Pool.Managed);
        }

        public void CameraPositioning()
        {
            device.Transform.Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4, this.Width / this.Height, 1f, 70f);
            device.Transform.View = Matrix.LookAtLH(new Vector3(15 * (float)Math.Sin(angle) + 30f, 12f, 15 * (float)Math.Cos(angle)),
                                        new Vector3(0, 0, 0),
                                        new Vector3(0, 1, 0));

            device.Lights[0].Type = LightType.Directional;
            device.Lights[0].Ambient = Color.Yellow;
            device.Lights[0].Direction = new Vector3(0f, 1f, -1f);
            device.Lights[0].Enabled = true;

            device.Lights[1].Type = LightType.Directional;
            device.Lights[1].Diffuse = Color.White;
            device.Lights[1].Direction = new Vector3(1f, -1f, 1f);
            device.Lights[1].Enabled = true;
        }

        public void VertexDeclaration()
        {
            vb = new VertexBuffer(typeof(CustomVertex.PositionNormalTextured), 32, device, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionNormalTextured.Format, Pool.Default);

            vertices = new CustomVertex.PositionNormalTextured[32];

            vertices[0] = new CustomVertex.PositionNormalTextured(2f, -4f, 0f, 0f, 3f, -2f, 0f, 0f);
            vertices[1] = new CustomVertex.PositionNormalTextured(5f, -4f, 0f, 0f, 3f, -2f, 0f, 1f);
            vertices[2] = new CustomVertex.PositionNormalTextured(0f, 0f, 5f, 0f, 3f, -2f, 1f, 0.5f);

            vertices[3] = new CustomVertex.PositionNormalTextured(5f, -4f, 0f, 10f, 5f, 14f, 0f, 0f);
            vertices[4] = new CustomVertex.PositionNormalTextured(3f, 0f, 0f, 10f, 5f, 14f, 0f, 1f);
            vertices[5] = new CustomVertex.PositionNormalTextured(0f, 0f, 5f, 10f, 5f, 14f, 1f, 0.5f);

            vertices[6] = new CustomVertex.PositionNormalTextured(3f, 0f, 0f, 0f, 1f, 0f, 0f, 0f);
            vertices[7] = new CustomVertex.PositionNormalTextured(0f, 0f, 0f, 0f, 1f, 0f, 0f, 1f);
            vertices[8] = new CustomVertex.PositionNormalTextured(0f, 0f, 5f, 0f, 1f, 0f, 1f, 0.5f);

            vertices[9] = new CustomVertex.PositionNormalTextured(0f, 0f, 0f, -2f, -1f, 0f, 0f, 0f);
            vertices[10] = new CustomVertex.PositionNormalTextured(2f, -4f, 0f, -2f, -1f, 0f, 0f, 1f);
            vertices[11] = new CustomVertex.PositionNormalTextured(0f, 0f, 5f, -2f, -1f, 0f, 1f, 0.5f);

            vertices[12] = new CustomVertex.PositionNormalTextured(0f, 0f, 0f, 0f, 0f, -1f, 0f, 0f);
            vertices[13] = new CustomVertex.PositionNormalTextured(3f, 0f, 0f, 0f, 0f, -1f, 0f, 1f);
            vertices[14] = new CustomVertex.PositionNormalTextured(2f, -4f, 0f, 0f, 0f, -1f, 1f, 0f);
            vertices[15] = new CustomVertex.PositionNormalTextured(5f, -4f, 0f, 0f, 0f, -1f, 1f, 1f);


            indices = new int[]
            {
                //bok
                0, 1, 2,
                3, 4, 5,
                6, 7, 8,
                9, 10, 11,
                //dno
                12, 13, 15,
                12, 15, 14,

            };

            for (int i = 16; i < 32; i++)
                vertices[i] = new CustomVertex.PositionNormalTextured(new Vector3(vertices[i - 16].Position.X + 1,
                    vertices[i - 16].Position.Y + 1, vertices[i - 16].Position.Z), vertices[i - 16].Normal, vertices[i - 16].Tu, vertices[i - 16].Tv);

            indices1 = new int[indices.Length * 2];
            for (int i = 0; i < indices.Length; i++) indices1[i] = indices[i];
            for (int i = indices.Length; i < indices.Length * 2; i++) indices1[i] = indices1[i - indices.Length] + 16;

            ib = new IndexBuffer(typeof(int), indices1.Length, device, Usage.WriteOnly, Pool.Default);

            ib.SetData(indices1, 0, LockFlags.None);

            vb.SetData(vertices, 0, LockFlags.None);
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.White, 1.0f, 0);

            device.BeginScene();
            device.VertexFormat = CustomVertex.PositionNormalTextured.Format;

            device.SetStreamSource(0, vb, 0);

            Material M = new Material();
            M.Diffuse = Color.Red;
            M.Emissive = Color.Purple;
            M.Ambient = Color.Aquamarine;
            M.Specular = Color.WhiteSmoke;
            M.SpecularSharpness = 0.5f;
            device.Material = M;

            device.Indices = ib;
            device.SetTexture(0, tex1);

            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 16, 0, 6);   //0, 0, 24, 0, 12

            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 16, 16, 18, 6); //0, 24, 24, 36, 12

            //x^2 / 100 + y^2 / 64 = 1 - ellipse. x = 10cost, y = 8sint
            //z = y - x - 10 = 8sint - 10cost - 10
            device.Transform.World = Matrix.RotationX(angle) * Matrix.RotationY(2 * angle) * Matrix.RotationZ(3 * angle);
            device.Lights[0].Direction = new Vector3(10 * (float)Math.Cos(angle), 8 * (float)Math.Sin(angle), 8 * (float)Math.Sin(angle) - 10 * (float)Math.Cos(angle) - 10);
            device.Lights[0].Enabled = true;

            device.EndScene();

            device.Present();
                
            this.Invalidate();
            angle += 0.01f;
        }
    }
}

