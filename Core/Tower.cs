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

        private int countBlocks;
        private float blockHeight = 30;


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

        public void ChangeMidPoint(float midPoint)
        {
            this.midPoint = midPoint;
        }

    }
}
