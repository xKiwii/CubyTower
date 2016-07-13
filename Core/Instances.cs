using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Tutorial.Core
{
    class Instances
    {
        public static Main Main { get; set; }
        public static Renderer Renderer { get; set; }
        public static Tower Tower { get; set; }
        public static Camera Camera { get; set; }
        public static GUI GUI { get; set; }
        public static TowerBlock TowerBlock { get; internal set; }
    }
}
