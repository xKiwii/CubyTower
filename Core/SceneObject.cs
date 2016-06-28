using Fusee.Engine.Core;
using Fusee.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Tutorial.Core
{
    class SceneObject
    {
        protected SceneContainer model;
        protected List<SceneComponentContainer> components;
        protected MaterialComponent materialComponent;
        protected MeshComponent meshComponent;
        public TransformComponent transformComponent;


        public Mesh Mesh;

        public SceneContainer getModel()
        {
            return model;
        }

        protected void parseModel()
        {
            components = model.Children.First().Components;
            foreach (var component in components)
            {
                if (component.GetType() == typeof(MaterialComponent)) { materialComponent = (MaterialComponent)component; }
                if (component.GetType() == typeof(TransformComponent)) { transformComponent = (TransformComponent)component; }
                if (component.GetType() == typeof(MeshComponent)) { meshComponent = (MeshComponent)component; }
            }
        }

        public SceneObject(SceneContainer scene)
        {
            this.model = scene;
            parseModel();
        }

        public SceneObject()
        {

        }
    }
}


