using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
        private DepthStencilView depthTarget;
        private Buffer vertexBuffer;
        private InputLayout inputLayout;
        private Effect effect;
        private float time = 0;
        private Matrix projection;
        public Form1()
        {
            InitializeComponent();
        }

        public void Render()
        {
            //描画前の処理
            device.ImmediateContext.OutputMerger.SetTargets(depthTarget, renderTarget);//描画先の指定
            device.ImmediateContext.ClearDepthStencilView(depthTarget, DepthStencilClearFlags.Depth, 1f, 0);
            device.ImmediateContext.ClearRenderTargetView(renderTarget, new Color4(1, 0, 0, 1));//renderTargetを指定色でクリア
            device.ImmediateContext.InputAssembler.InputLayout = inputLayout;//InputLayoutの指定
            device.ImmediateContext.InputAssembler.SetVertexBuffers(0,
                new VertexBufferBinding[] { new VertexBufferBinding(vertexBuffer, 16, 0) });//頂点バッファのセット
            device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;//PrimitiveTopologyのセット。今回は三角形リスト

            effect.GetVariableBySemantic("PROJECTION").AsMatrix().SetMatrix(projection);

            //最初の描画(薄い紫色)
            effect.GetVariableBySemantic("COLOR").AsVector().Set(new Vector4(1.0f, 0.8f, 1f, 1f));//float4 col:COLORに対して指定したベクトルをセット
            effect.GetTechniqueByIndex(0).GetPassByIndex(0).Apply(device.ImmediateContext);//エフェクトの適用
            device.ImmediateContext.Draw(3, 0);//renderTargetに対して書き込み

            //次の描画(濃い紫色)
            effect.GetVariableBySemantic("COLOR").AsVector().Set(new Vector4(1f, 0f, 1f, 1f));//float4 col:COLORに対して指定したベクトルをセット
            effect.GetTechniqueByIndex(0).GetPassByIndex(0).Apply(device.ImmediateContext);//エフェクトの適用
            device.ImmediateContext.Draw(3, 3);//renderTargetに対して書き込み
            effect.GetVariableBySemantic("LIGHT").AsVector().Set(new Vector4(Math.Abs((float)Math.Sin(time)), Math.Abs((float)Math.Cos(time)),1f , 1f));

            //バックバッファとフロントバッファを入れ替える
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
            using (Texture2D depthTex = new Texture2D(device, new Texture2DDescription()
            {
                ArraySize = 1,
                BindFlags = BindFlags.DepthStencil,
                Width = Width,
                Height = Height,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                Format = Format.D32_Float,
                CpuAccessFlags = CpuAccessFlags.None,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None
            }))
            {
                depthTarget = new DepthStencilView(device, depthTex);
            }
            //四角形の頂点を表すリスト
            Vector4[] verticies = new Vector4[]
            {
                new Vector4(-0.5f,0.75f,5f,1f),//左上手前の三角形
                new Vector4(0f,0f,5f,1f),
                new Vector4(-1f,0f,5f,1f), 
 
 
                new Vector4(0f,1f,10f,1f),//中央奥の大きい三角形
                new Vector4(1f,-0.5f,10f,1f),
                new Vector4(-1f,-0.5f,10f,1f), 
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
            projection = Matrix.PerspectiveFovLH((float)Math.PI / 4f, Width/Height, 0.1f, 15);
        }
    }
}