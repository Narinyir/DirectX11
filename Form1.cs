using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using Buffer = SlimDX.Direct3D11.Buffer;
using Device = SlimDX.Direct3D11.Device;

namespace DirectX_01
{
    public partial class Form1 : Form
    {

        private SwapChain swapChain;
        private Device device;
        private RenderTargetView renderTarget;
        private Buffer vertexBuffer;
        private InputLayout inputLayout;
        private Effect effect;
        private float time = 0;

        public Form1()
        {
            InitializeComponent();
        }

        public void Render()
        {
            effect.GetVariableBySemantic("TIME").AsScalar().Set(time);
            device.ImmediateContext.OutputMerger.SetTargets(renderTarget);
            device.ImmediateContext.ClearRenderTargetView(renderTarget, new Color4(1, 0, 0, 1));
            device.ImmediateContext.InputAssembler.InputLayout = inputLayout;
            device.ImmediateContext.InputAssembler.SetVertexBuffers(0,
                new VertexBufferBinding[] { new VertexBufferBinding(vertexBuffer, 16, 0) });
            device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            effect.GetTechniqueByIndex(0).GetPassByIndex(0).Apply(device.ImmediateContext);
            device.ImmediateContext.Draw(6, 0);
            swapChain.Present(0, PresentFlags.None);
            time += 0.001f;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SwapChainDescription desc = new SwapChainDescription()
            {
                BufferCount = 2,
                Flags = SwapChainFlags.AllowModeSwitch,
                IsWindowed = true,
                ModeDescription = new ModeDescription()
                {
                    Format = Format.R8G8B8A8_UNorm,
                    Height = Height,
                    Width = Width,
                    RefreshRate = new Rational(1, 60),
                    Scaling = DisplayModeScaling.Centered,
                    ScanlineOrdering = DisplayModeScanlineOrdering.Progressive
                },
                OutputHandle = Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None,
                new FeatureLevel[1] { FeatureLevel.Level_11_0 }, desc, out device, out swapChain);
            using (Texture2D tex = Texture2D.FromSwapChain<Texture2D>(swapChain, 0))
            {
                renderTarget = new RenderTargetView(device, tex);
            }
            //四角形の頂点を表すリスト
            Vector4[] verticies = new Vector4[]
            {
                new Vector4(-1,1,0.5f,1),
                new Vector4(1,1,0.5f,1),
                new Vector4(-1,-1,0.5f,1),
                new Vector4(1,1,0.5f,1),
                new Vector4(1,-1,0.5f,1),
                new Vector4(-1,-1,0.5f,1), 
            };
            using (DataStream ds = new DataStream(verticies, true, true))
            {
                vertexBuffer = new Buffer(device, ds, new BufferDescription()
                {
                    BindFlags = BindFlags.VertexBuffer,
                    SizeInBytes = (int)ds.Length,
                });
            }
            using (ShaderBytecode compiledCode = ShaderBytecode.CompileFromFile("shader.fx", "fx_5_0", ShaderFlags.Debug, EffectFlags.None))
            {
                effect = new Effect(device, compiledCode);
            }
            inputLayout = new InputLayout(device, effect.GetTechniqueByIndex(0).GetPassByIndex(0).Description.Signature, new InputElement[]
            {
                new InputElement()
                {
                    SemanticName = "POSITION",
                    Format = Format.R32G32B32A32_Float
                }, 
            });
            device.ImmediateContext.Rasterizer.SetViewports(new Viewport[] { new Viewport(0, 0, Width, Height, 0, 1), });
            Texture2D sResource = Texture2D.FromFile(device , "tino.jpg");
            ShaderResourceView sView = new ShaderResourceView(device,sResource);
        }
    }
}