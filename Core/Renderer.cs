using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Tutorial.Core
{

    class Renderer : SceneVisitor
    {
        public RenderContext RC;
        public IShaderParam AlbedoParam;
        public IShaderParam ShininessParam;
        public IShaderParam SpecFactorParam;
        public IShaderParam SpecColorParam;
        public IShaderParam AmbientColorParam;
        public float4x4 View;
        private Dictionary<MeshComponent, Mesh> _meshes = new Dictionary<MeshComponent, Mesh>();
        private CollapsingStateStack<float4x4> _model = new CollapsingStateStack<float4x4>();
        private IShaderParam TexMixParam;
        private IShaderParam TextureParam;
        private ITexture _leavesTexture;

        private Mesh LookupMesh(MeshComponent mc)
        {
            Mesh mesh;
            if (!_meshes.TryGetValue(mc, out mesh))
            {
                mesh = new Mesh
                {
                    Vertices = mc.Vertices,
                    Normals = mc.Normals,
                    UVs = mc.UVs,
                    Triangles = mc.Triangles,
                };
                _meshes[mc] = mesh;
            }
            return mesh;
        }

        public Renderer(RenderContext rc)
        {
            RC = rc;
            // Initialize the shader(s)
            var vertsh = AssetStorage.Get<string>("VertexShader.vert");
            var pixsh = AssetStorage.Get<string>("PixelShader.frag");
            var shader = RC.CreateShader(vertsh, pixsh);
            RC.SetShader(shader);
            AlbedoParam = RC.GetShaderParam(shader, "albedo");
            ShininessParam = RC.GetShaderParam(shader, "shininess");
            SpecFactorParam = RC.GetShaderParam(shader, "specfactor");
            SpecColorParam = RC.GetShaderParam(shader, "speccolor");
            AmbientColorParam = RC.GetShaderParam(shader, "ambientcolor");

            ImageData leaves = AssetStorage.Get<ImageData>("pflasterstein.jpg");
            _leavesTexture = RC.CreateTexture(leaves);
            TextureParam = RC.GetShaderParam(shader, "texture");
            TexMixParam = RC.GetShaderParam(shader, "texmix");
        }


        protected override void InitState()
        {
            _model.Clear();
            _model.Tos = float4x4.Identity;
        }
        protected override void PushState()
        {
            _model.Push();
        }
        protected override void PopState()
        {
            _model.Pop();
            RC.ModelView = View * _model.Tos;
        }
        [VisitMethod]
        void OnMesh(MeshComponent mesh)
        {
            RC.Render(LookupMesh(mesh));
        }
        [VisitMethod]
        void OnMaterial(MaterialComponent material)
        {
            if (CurrentNode.Name.Contains("block"))
            {
                RC.SetShaderParamTexture(TextureParam, _leavesTexture);
                RC.SetShaderParam(TexMixParam, 1.0f);
            }
            else
            {
                RC.SetShaderParam(TexMixParam, 0.0f);
            }
            if (material.HasDiffuse)
            {
                RC.SetShaderParam(AlbedoParam, material.Diffuse.Color);
            }
            else
            {
                RC.SetShaderParam(AlbedoParam, float3.Zero);
            }
            if (material.HasSpecular)
            {
                RC.SetShaderParam(ShininessParam, material.Specular.Shininess);
                RC.SetShaderParam(SpecFactorParam, material.Specular.Intensity);
                RC.SetShaderParam(SpecColorParam, material.Specular.Color);
            }
            else
            {
                RC.SetShaderParam(ShininessParam, 0);
                RC.SetShaderParam(SpecFactorParam, 0);
                RC.SetShaderParam(SpecColorParam, float3.Zero);
            }
            if (material.HasEmissive)
            {
                RC.SetShaderParam(AmbientColorParam, material.Emissive.Color);
            }
            else
            {
                RC.SetShaderParam(AmbientColorParam, float3.Zero);
            }
        }
        [VisitMethod]
        void OnTransform(TransformComponent xform)
        {
            _model.Tos *= xform.Matrix();
            RC.ModelView = View * _model.Tos;
        }
    }
}