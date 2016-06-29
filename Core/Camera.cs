﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math.Core;

namespace Fusee.Tutorial.Core
{
    class Camera
    {
        public float4x4 mtxOffset = float4x4.CreateTranslation(0, 0, 0);
        private float yOffset = 0;
        private float speed = -0.093f;

        public void CheckCameraPosition(int blocksCount)
        {
            if(blocksCount > 3)
            {
                yOffset += speed;
                mtxOffset = float4x4.CreateTranslation(0, yOffset, 0);
            }
        }



    }
}
