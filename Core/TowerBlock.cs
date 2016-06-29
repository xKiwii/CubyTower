using Fusee.Base.Core;
using Fusee.Engine.Core;
using Fusee.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math.Core;
using System.Diagnostics;
using System;

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
            transformComponent.Scale = new float3(Instances.Tower.upperWidth/200, 1, 1);

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
                        CheckCollision();
                        
                    }
                }

                transformComponent.Translation = new float3(x, y, z);
                
            }
              
        }


        void CheckCollision()
        {
            bool collision = true;

            float rightBound = Instances.Tower.midPoint + Instances.Tower.upperWidth;
            float leftBound = Instances.Tower.midPoint - Instances.Tower.upperWidth;

            if(x > rightBound || x < leftBound)
            {
                collision = false;
            }

            if (collision)
            {

                float halfWidth = Instances.Tower.upperWidth / 2;
                float newWidth;
                float newMidPoint;
                float score = 100;


                if (x > Instances.Tower.midPoint)
                {

                    if(x > halfWidth)
                    {
                        newWidth = (Instances.Tower.midPoint + halfWidth) - (x - halfWidth);
                        float overplus = Instances.Tower.upperWidth - newWidth;
                        newMidPoint = (x - halfWidth) + (newWidth / 2);

                        Debug.WriteLine("Mittelpunkt " + newMidPoint + " Neue Breite: " + newWidth);
                    }
                    else
                    {
                        //Block liegt rechts Mittelpunkt -- rechts muss was abgeschnitten werden
                        float overplus = (x + halfWidth) - (Instances.Tower.midPoint + halfWidth);
                        newWidth = Instances.Tower.upperWidth - overplus;
                        newMidPoint = x - (overplus / 2);

                        Debug.WriteLine("Mittelpunkt " + newMidPoint + " Neue Breite: " + newWidth);
                    }

                    score = score - (x - Instances.Tower.midPoint);

                    Instances.Tower.ChangeFutureBlocks(newMidPoint, newWidth);
                    x = newMidPoint;
                    transformComponent.Translation = new float3(x, y, z);
                    transformComponent.Scale = new float3(newWidth/200, 1, 1);

                    
                }

                else if (x < Instances.Tower.midPoint)
                {
                    //Block liegt links vom Mittelpunkt

                    if(x < (Instances.Tower.midPoint - halfWidth))
                    {
                        newWidth = (Instances.Tower.midPoint - halfWidth) - (x + halfWidth); //evtl. Fehlerquelle
                        newWidth = System.Math.Abs(newWidth);
                        float overplus = Instances.Tower.upperWidth - newWidth;
                        newMidPoint = (x + halfWidth) - (newWidth / 2);

                        Debug.WriteLine("Mittelpunkt " + newMidPoint + " Neue Breite: " + newWidth);
                    }
                    else
                    {
                        float overplus = (x - halfWidth) - (Instances.Tower.midPoint - halfWidth);
                        newWidth = Instances.Tower.upperWidth + overplus;
                        newMidPoint = x - (overplus / 2);

                        Debug.WriteLine("Mittelpunkt " + newMidPoint + " Neue Breite: " + newWidth);
                    }

                    score = score - (Instances.Tower.midPoint - x);

                    Instances.Tower.ChangeFutureBlocks(newMidPoint, newWidth);
                    x = newMidPoint;
                    transformComponent.Translation = new float3(x, y, z);
                    transformComponent.Scale = new float3(newWidth / 200, 1, 1);
                }
                else if (x == Instances.Tower.midPoint)
                {
                    //Liegt direkt drauf- Super!
                }


                Instances.Main.AddPointsToScore(score);
            }
            

            if (collision)
            {
                Instances.Main.CreateNewBlock();
            }
            else
            {
                //GameOver Logic
            }
            
        }
    }
}
