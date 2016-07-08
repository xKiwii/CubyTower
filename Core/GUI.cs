using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math.Core;
using Fusee.Engine.Core.GUI;
using Fusee.Engine.Core;
using Fusee.Engine.Common;
using Fusee.Base.Core;
using System.Diagnostics;
using Fusee.Base.Common;

namespace Fusee.Tutorial.Core
{
    class GUI
    {
       // private RenderContext RC;

        public GUIHandler _guiHandler;

        public GUIButton _guiFuseeLink;
       // private GUIImage _guiFuseeLogo;
        public FontMap _guiLatoBlack;
        private GUIText _guiSubText;
        private GUIText _guiPointsText;
        private GUIText _guiPoints;
        private GUIImage _guiBackground;

        public float score = 0;
        

        public void Init()
        // public GUI()
        {
            //guihandler
            _guiHandler = new GUIHandler();
            _guiHandler.AttachToContext(Instances.Renderer.RC);


            var fontLato = AssetStorage.Get<Font>("Montserrat-Regular.ttf");
            fontLato.UseKerning = true;
            _guiLatoBlack = new FontMap(fontLato, 18);

            _guiFuseeLink = new GUIButton("Reset", _guiLatoBlack, 20, 6, 157, 87);
            _guiFuseeLink.ButtonColor = new float4(0, 0, 0, 0);
            _guiFuseeLink.BorderColor = new float4(0, 0.6f, 0.2f, 1);
            _guiFuseeLink.BorderWidth = 0;
            _guiFuseeLink.OnGUIButtonDown += _guiFuseeLink_OnGUIButtonDown;
            _guiFuseeLink.OnGUIButtonEnter += _guiFuseeLink_OnGUIButtonEnter;
            _guiFuseeLink.OnGUIButtonLeave += _guiFuseeLink_OnGUIButtonLeave;
            _guiHandler.Add(_guiFuseeLink);

            //_guiBackground = new GUIImage(AssetStorage.Get<ImageData>("himmel.jpg"), -30, -300, -5, 1500, 1000);
            //_guiHandler.Add(_guiBackground);

            _guiPoints = new GUIText("", _guiLatoBlack, 1150, 57);
            _guiHandler.Add(_guiPoints);

        }

        public void _guiFuseeLink_OnGUIButtonLeave(GUIButton sender, GUIButtonEventArgs mea)
        {
            _guiFuseeLink.ButtonColor = new float4(0, 0, 0, 0);
            _guiFuseeLink.BorderWidth = 0;
        }

        public void _guiFuseeLink_OnGUIButtonEnter(GUIButton sender, GUIButtonEventArgs mea)
        {
            _guiFuseeLink.ButtonColor = new float4(0, 0.6f, 0.2f, 0.4f);
            _guiFuseeLink.BorderWidth = 1;
        }

        void _guiFuseeLink_OnGUIButtonDown(GUIButton sender, GUIButtonEventArgs mea)
        {
            Debug.WriteLine("Button wurde geklickt");
            Instances.Main.RestartGame();
            //OpenLink("http://fusee3d.org");
        }

      /*  private void OpenLink(string v)
        {
            throw new NotImplementedException();
        }*/

        internal void RenderGUI()
        {
            throw new NotImplementedException();
        }

        public void AddPointsToScore()
        {
            _guiPoints.Text = "SCORE" + " "+ Instances.Main.score;
            _guiHandler.Refresh();
            
        }

    }
}
    