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
//using Fusee.Engine.Core.GUI;


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

        private Dictionary<string, ITexture> _textures = new Dictionary<string, ITexture>();

        private IShaderParam TexMixParam;
        private IShaderParam TextureParam;
        private ShaderProgram _shader;

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

        private ImageData LookUpImage(string imageFile)
        {
            ITexture texture;
            ImageData image = AssetStorage.Get<ImageData>(imageFile);
            if (!_textures.TryGetValue(imageFile, out texture))
            {
                texture = RC.CreateTexture(image);
                _textures[imageFile] = texture;
            }

            return image;
        }

        public Renderer(RenderContext rc)
        {
            RC = rc;
            // Initialize the shader(s)
            var vertsh = AssetStorage.Get<string>("VertexShader.vert");
            var pixsh = AssetStorage.Get<string>("PixelShader.frag");
            _shader = RC.CreateShader(vertsh, pixsh);
            RC.SetShader(_shader);

            AlbedoParam = RC.GetShaderParam(_shader, "albedo");
            ShininessParam = RC.GetShaderParam(_shader, "shininess");
            SpecFactorParam = RC.GetShaderParam(_shader, "specfactor");
            SpecColorParam = RC.GetShaderParam(_shader, "speccolor");
            AmbientColorParam = RC.GetShaderParam(_shader, "ambientcolor");

            LookUpImage("Leaves.jpg");
            LookUpImage("pflasterstein.jpg");
            LookUpImage("himmel.jpg");

            TextureParam = RC.GetShaderParam(_shader, "texture");
            TexMixParam = RC.GetShaderParam(_shader, "texmix");
        }


        protected override void InitState()
        {
            RC.SetShader(_shader);

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
            if (material.HasDiffuse)
            {
                RC.SetShaderParam(AlbedoParam, material.Diffuse.Color);
            }
            else
            {
                RC.SetShaderParam(AlbedoParam, float3.Zero);
            }

            if (CurrentNode.Name.Contains("block"))
            {
                RC.SetShaderParamTexture(TextureParam, _textures["pflasterstein.jpg"]);
                RC.SetShaderParam(TexMixParam, 1.0f);
            }


            else if (CurrentNode.Name.Contains("Ebene"))
            {
                RC.SetShaderParamTexture(TextureParam, _textures["Leaves.jpg"]);
                RC.SetShaderParam(TexMixParam, 1.0f);
            }
            else if (CurrentNode.Name.Contains("himmel"))
            {
                RC.SetShaderParamTexture(TextureParam, _textures["himmel.jpg"]);
                RC.SetShaderParam(TexMixParam, 1.0f);
            }
            else
            {
                RC.SetShaderParam(TexMixParam, 0.0f);
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