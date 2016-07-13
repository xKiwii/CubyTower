using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Tutorial.Core
{
    class Tower
    {
        public float towerHeight;
        public float offset;

        public float midPoint = 0;
        public float upperWidth = 200;

        public int countBlocks;
        private float blockHeight = 25;


        public Tower(int lowerScreenHeight)
        {
            offset = lowerScreenHeight+ 80;
            countBlocks = 0;
        }

        public void AddBlock()
        {
            countBlocks += 1;
            CalculateTowerHeight();
        }

        private void CalculateTowerHeight()
        {
          towerHeight = countBlocks * blockHeight + offset;
        }

        public float GetHeight()
        {
            return towerHeight;
        }

        public float GetBlockHeight()
        {
            return blockHeight;
        }

        public int GetCountBlocks()
        {
            return countBlocks;
        }

        public void ChangeFutureBlocks(float midPoint, float newWidth)
        {
            this.midPoint = midPoint;
            this.upperWidth = newWidth;
        }

    }
}
