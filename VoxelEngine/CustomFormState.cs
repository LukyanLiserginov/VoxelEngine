using ImGuiNet;
using System;
using System.Collections.Generic;
using System.Text;
using Urho3DNet;

namespace VoxelEngine
{
    internal partial class CustomFormState : ApplicationState
    {
        protected readonly Scene _scene;
        private readonly UrhoPluginApplication _app;
        private readonly Node _cameraNode;
        private readonly Camera _camera;
        private readonly Viewport _viewport;

        private InputMap _inputMap;
        private readonly PhysicsRaycastResult _raycastResult;

        private bool _LKMPressed;
        private bool _usePressed;
        private bool _constructMode = true;
        private Node _comstructNode = null;
        private int _counter = 0;
        private int _blockType = 1;

        public CustomFormState(UrhoPluginApplication app) : base(app.Context)
        {
            MouseMode = MouseMode.MmFree;
            IsMouseVisible = true;

            _app = app;
            _scene = Context.CreateObject<Scene>();
            _scene.Load("Scenes/Scene.scene");
            _cameraNode = _scene.FindChild("CameraNode", true);
            _camera = _cameraNode.GetComponent<Camera>();
            _viewport = Context.CreateObject<Viewport>();
            _viewport.Scene = _scene;
            _viewport.Camera = _camera;
            SetViewport(0, _viewport);
            _inputMap = Context.ResourceCache.GetResource<InputMap>("Input/MoveAndOrbit.inputmap");

            _cameraNode.CreateComponent<FreeFlyController>();
            _raycastResult = new PhysicsRaycastResult();

            _comstructNode = _scene.CreateChild();
            _comstructNode.CreateComponent<StaticModel>().SetModel(Context.ResourceCache.GetResource<Model>("Models/Box.mdl"));
        }

        public override void Update(float timeStep)
        {
            //ui block
            ImGui.Begin("Main");
            ImGui.InputInt("BlockType", ref _blockType);
            ImGui.End();


            if (_inputMap.Evaluate("MouseRight") > 0.5f)
            {
                MouseMode = MouseMode.MmRelative;
                IsMouseVisible = false;
            }
            else
            {
                MouseMode = MouseMode.MmFree;
                IsMouseVisible = true;

                var world = _scene.GetComponent<PhysicsWorld>();
                world.RaycastSingle(_raycastResult, _camera.ScreenRayFromMouse, 400);
                if (_raycastResult != null)
                {
                    _comstructNode.Position = MyTools.Round(_raycastResult.Position);
                }
            }

            var LKMPresed = _inputMap.Evaluate("MouseLeft") > 0.5f;
            if (LKMPresed != _LKMPressed)
            {
                if (LKMPresed && _constructMode)
                {
                    var world = _scene.GetComponent<PhysicsWorld>();
                    world.RaycastSingle(_raycastResult, _camera.ScreenRayFromMouse, 400);
                    if (_raycastResult != null)
                    {
                        SetVoxel(_raycastResult.Position);
                    }

                }
                _LKMPressed = LKMPresed; //comment this line for fast draw
            }
            var usePressed = _inputMap.Evaluate("Jump") > 0.5f;
            if (usePressed != _usePressed)
            {
                if (usePressed)
                {
                    //MyTools.SaveBuild(_coreBlock.Blocks);
                    //System.Console.WriteLine("file saved");
                }
                _usePressed = usePressed;
            }
            
        }

        public void SetVoxel(Vector3 pos)
        {
            var voxelNode = _scene.CreateChild();
            voxelNode.CreateComponent<Voxel>();
            voxelNode.Position = pos;
        }

        public override void Activate(StringVariantMap bundle)
        {
            SubscribeToEvent(E.KeyUp, HandleKeyUp);
            _scene.IsUpdateEnabled = true;

            base.Activate(bundle);
        }

        public override void Deactivate()
        {
            _scene.IsUpdateEnabled = false;
            base.Deactivate();
        }

        protected override void Dispose(bool disposing)
        {
            _scene?.Dispose();

            base.Dispose(disposing);
        }


        private void HandleKeyUp(VariantMap args)
        {
            var key = (Key)args[E.KeyUp.Key].Int;
            switch (key)
            {
                case Key.KeyEscape:
                case Key.KeyBackspace:
                    _app.HandleBackKey();
                    return;
            }
        }
    }
}
