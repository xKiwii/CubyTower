using System.Collections.Generic;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;
using System.Diagnostics;

namespace Fusee.Tutorial.Core
{

    [FuseeApplication(Name = "CubyTower", Description = "Best game ever.")]
    public class Main : RenderCanvas
    {
        private Renderer _renderer;
        private float4x4 _projection;
        private Camera _camera;
        private GUI _gui;

        private Tower tower;
        private TowerBlock firstTowerBlock;
        private TowerBlock firstMovingBlock;
        private SceneContainer steinModel;
        private SceneContainer himmelModel;
        private SceneContainer environmentModel;

        private SceneObject environment;
        private SceneObject hintergrund;



        private List<SceneObject> renderList = new List<SceneObject>();
        private List<EveryFrame> everyFrame = new List<EveryFrame>();
        private static float _angleHorz = M.PiOver6 * 2.0f, _angleVert = -M.PiOver6 * 0.5f, _angleVelHorz, _angleVelVert, _angleRoll;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private float4x4 _sceneScale;
        private bool _keys;

        public float score = 0;
        private float speed = 0;

        // Init is called on startup. 
        public override void Init()
        {
            // Instantiate our self-written renderer
            _renderer = new Renderer(RC);
            _sceneScale = float4x4.CreateScale(0.1f);
            _camera = new Camera();
            _gui = new GUI();

            Instances.Main = this;
            Instances.Renderer = _renderer;
            Instances.Camera = _camera;
            Instances.GUI = _gui;


            // Instantiate the enviroment


            environmentModel = AssetStorage.Get<SceneContainer>("plane2.fus");
            environment = new SceneObject(environmentModel);
            environment.transformComponent.Translation.y = -400;
            environment.transformComponent.Translation.z = 100;
            environment.transformComponent.Scale.x = 6;
            environment.transformComponent.Scale.z = 1.5f;

            himmelModel = AssetStorage.Get<SceneContainer>("himmel.fus");
            hintergrund = new SceneObject(himmelModel);
            hintergrund.transformComponent.Translation.z = 100;
            hintergrund.transformComponent.Scale.x = 1;
            hintergrund.transformComponent.Scale.z = 1;


            //Instantiate Tower ans his first TowerBlock

            tower = new Tower(-(Height / 2));
            Instances.Tower = tower;

            steinModel = AssetStorage.Get<SceneContainer>("stein.fus");
            tower.AddBlock();

            var copy1 = AssetStorage.DeepCopy(steinModel);
            firstTowerBlock = new TowerBlock(copy1, 0, -(Height/2)+tower.GetBlockHeight()+50);

            
            // Instantiate the first moving TowerBlock
            var copy2 = AssetStorage.DeepCopy(steinModel);
            firstMovingBlock = new TowerBlock(copy2,  Width, Height, 10.0f);



            // Add everything to render
            renderList.Add(hintergrund);
            renderList.Add(environment);
            
            renderList.Add(firstTowerBlock);
            renderList.Add(firstMovingBlock);



            everyFrame.Add(firstMovingBlock);
            everyFrame.Add(firstTowerBlock);

            _gui.Init();



            // Set the clear color for the backbuffer
            RC.ClearColor = new float4(0.18f, 0.72f, 0.98f, 1);

           // RC.ClearColor = new float4(1, 1, 1, 1);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            RC.Viewport(0, 0, Width, Height);

            // Mouse and keyboard movement
            if (Keyboard.LeftRightAxis != 0 || Keyboard.UpDownAxis != 0)
            {
                _keys = true;
            }

            var curDamp = (float)System.Math.Exp(-Damping * DeltaTime);

            // UpDown / LeftRight rotation
            if (Mouse.LeftButton)
            {
                _keys = false;
                _angleVelHorz = -RotationSpeed * Mouse.XVel * 0.000002f;
                _angleVelVert = -RotationSpeed * Mouse.YVel * 0.000002f;
            }
            else if (Touch.GetTouchActive(TouchPoints.Touchpoint_0) && !Touch.TwoPoint)
            {
                _keys = false;
                float2 touchVel;
                touchVel = Touch.GetVelocity(TouchPoints.Touchpoint_0);
                _angleVelHorz = -RotationSpeed * touchVel.x * 0.000002f;
                _angleVelVert = -RotationSpeed * touchVel.y * 0.000002f;
            }
            else
            {
                if (_keys)
                {
                    _angleVelHorz = -RotationSpeed * Keyboard.LeftRightAxis * 0.002f;
                    _angleVelVert = -RotationSpeed * Keyboard.UpDownAxis * 0.002f;
                }
                else
                {
                    _angleVelHorz *= curDamp;
                    _angleVelVert *= curDamp;
                }
            }

            _angleHorz += _angleVelHorz;
            // Wrap-around to keep _angleHorz between -PI and + PI
            _angleHorz = M.MinAngle(_angleHorz);

            _angleVert += _angleVelVert;
            // Limit pitch to the range between [-PI/2, + PI/2]
            _angleVert = M.Clamp(_angleVert, -M.PiOver2, M.PiOver2);

            // Wrap-around to keep _angleRoll between -PI and + PI
            _angleRoll = M.MinAngle(_angleRoll);



            // Create the camera matrix and set it as the current ModelView transformation
            // var mtxRot = float4x4.CreateRotationZ(_angleRoll) * float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            var mtxRot = float4x4.CreateRotationZ(0) * float4x4.CreateRotationX(0) * float4x4.CreateRotationY(0);
            //var mtxCam = float4x4.LookAt(0, 20, -80, 0, 0, 0, 0, 1, 0);
            var mtxCam = float4x4.LookAt(0, 20, -80, 0, 0, 0, 0, 1, 0);
            var mtxOffset = _camera.mtxOffset;
            _renderer.View = mtxOffset * mtxCam * mtxRot * _sceneScale;
            //var mtxOffset = float4x4.CreateTranslation(0, 0, 0);
            //RC.Projection = mtxOffset * _projection;
            RC.Projection = _projection;


            doFrame();
            renderObjects();
            _gui._guiHandler.RenderGUI();

            // Swap buffers: Show the contents of the backbuffer (containing the currently rerndered farame) on the front buffer.
            Present();

        }

        // Is called when the window was resized
        public override void Resize()
        {
            // Set the new rendering area to the entire new windows size
            RC.Viewport(0, 400, Width, Height);

            // Create a new projection matrix generating undistorted images on the new aspect ratio.
            var aspectRatio = Width / (float)Height;

            // 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
            //_projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 20000);

            _projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 2000);
            Instances.GUI.Resize();
            _gui._guiHandler.Refresh();
            //Give Block new 
        }

        public void renderObjects()
        {
            foreach (var renderObject in renderList)
            {
                _renderer.Traverse(renderObject.getModel().Children);
            }

        }

        public void doFrame()
        {

            for (int i = 0; i < everyFrame.Count; i++)
            {
                everyFrame[i].RenderAFrame();
            }
        }

        public static KeyboardDevice getKeyboard()
        {
            return Keyboard;
        }


        public void CreateNewBlock()
        {
            tower.AddBlock();
            _camera.CheckCameraPosition(tower.GetCountBlocks());
            speed = Instances.Tower.countBlocks + 5;
            var copy3 = AssetStorage.DeepCopy(steinModel);
            var block2 = new TowerBlock(copy3, Width, Height, speed);

            renderList.Add(block2);
            everyFrame.Add(block2);

        }

        public void AddPointsToScore(float points)
        {
            score += points;
            score = (float) System.Math.Round(score);
            Instances.GUI.AddPointsToScore();
            Debug.WriteLine("Dazugewonnene Punkte: " + points + " Neue Punktzahl: "+ score);
        }

        public void GameOver()
        {
            // RestartGame();
            Instances.GUI.AddGuiButton();
        }


        public void RestartGame()
        {
            renderList.Clear();
            score = 0;
            AddPointsToScore(0);
            tower = new Tower(-(Height / 2));
            Instances.Tower = tower;

            var copy4 = AssetStorage.DeepCopy(steinModel);
            var copy5 = AssetStorage.DeepCopy(steinModel);
            firstTowerBlock = new TowerBlock(copy4, 0, -(Height / 2) + tower.GetBlockHeight() + 50);

            _camera.yOffset = 0;
            _camera.mtxOffset = float4x4.CreateTranslation(0, 0, 0);

            renderList.Add(hintergrund);
            renderList.Add(environment);

            renderList.Add(firstTowerBlock);

            everyFrame.Clear();
            everyFrame.Add(firstTowerBlock);

            CreateNewBlock();
        }

    }
}