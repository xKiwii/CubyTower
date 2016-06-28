using Fusee.Base.Core;
using Fusee.Engine.Core;
using Fusee.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math.Core;
using System.Diagnostics;

namespace Fusee.Tutorial.Core
{
    class TowerBlock : SceneObject, EveryFrame
    {
        private KeyboardDevice kd = Main.getKeyboard();

        public float speed;
        private float fallSpeed = 5.0f;
        public bool isActive = true;
        public bool isFalling = false;
        public bool isGrounded = false;

        private float alpha = 0.0f;
        private float x;
        private float y;
        private float z;
        private int screenWidth;
        private int screenHeight;

        private float towerPosition;

        public TowerBlock(SceneContainer model, int screenWidth, int screenHeight, float speed)
        {
            this.model = model;
            parseModel();

            transformComponent.Translation.y = screenHeight/2 - 100;
            x = transformComponent.Translation.x;
            y = transformComponent.Translation.y;
            z = transformComponent.Translation.z;

            towerPosition = Instances.Tower.GetHeight();


            if (Instances.Tower.GetCountBlocks() > 3)
            {
                y += (Instances.Tower.GetBlockHeight() * (Instances.Tower.GetCountBlocks() - 3)) ;
            }
       

            transformComponent.Translation = new float3(x, y, z);
            transformComponent.Rotation = new float3(0, 0, 0);
            transformComponent.Scale = new float3(1, 0.5f, 1);

            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.speed = speed;

            
        }

        public TowerBlock(SceneContainer model, float x, float y)
        {
            this.model = model;
            parseModel();

            this.x = x;
            this.y = y;
            this.z = transformComponent.Translation.z;

            transformComponent.Scale = new float3(1, 0.5f, 1);
            transformComponent.Translation = new float3(x, y, z);

            isGrounded = true;
            isActive = false;
            isFalling = false;

            towerPosition = Instances.Tower.GetHeight();
        }



        public void RenderAFrame()
        {
            if(!isGrounded)
            {
                
                if (!isFalling && isActive && kd.IsKeyDown(Engine.Common.KeyCodes.Space))
                {
                    isFalling = true;
                    isActive = false;
                }


                if (isActive && !isFalling)
                {
                    if (x > screenWidth / 2 || x < (screenWidth / 2) * (-1))
                    {
                        speed *= -1;
                    }

                    x += speed;
                }

                if (isFalling)
                {
                    y -= fallSpeed;
                    if (y < towerPosition)
                    {
                        isFalling = false;
                        Instances.Main.CreateNewBlock();
                    }
                }

                transformComponent.Translation = new float3(x, y, z);
            }
              
        }
    }
}
