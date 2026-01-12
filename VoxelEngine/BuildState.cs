using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Urho3DNet;

namespace VoxelEngine
{
    internal partial class BuildState : ApplicationState
    {
        protected readonly Scene _scene;
        private readonly UrhoPluginApplication _app;
        private readonly Node _cameraNode;
        private readonly Camera _camera;
        private readonly Viewport _viewport;
        private InputMap _inputMap;

        private Queue<Vector3> voxelQueue = new Queue<Vector3>();

        public BuildState(UrhoPluginApplication app, IVoxelShape shape) : base(app.Context)
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

            int size = 32;
            //IVoxelShape shape = VoxelShapeFactory.Create<Sphere>();
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        if (shape.IsInside(x, y, z, size))
                        {
                            voxelQueue.Enqueue(new Vector3(x, y, z));
                        }
                    }
                }
            }
        }

        public override void Update(float timeStep)
        {
            if (voxelQueue.Count > 0) 
            {
                SetupVoxel(voxelQueue.Dequeue());
            }
        }

        public void SetupVoxel (Vector3 pos)
        {
            var voxelNode = _scene.CreateChild();
            voxelNode.Position = pos;
            voxelNode.CreateComponent<Voxel>();
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
