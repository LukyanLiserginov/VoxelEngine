using System;
using System.Collections.Generic;
using System.Text;
using Urho3DNet;

namespace StarCubes
{
    internal partial class MyState : ApplicationState
    {
        protected readonly Scene _scene;
        private readonly UrhoPluginApplication _app;
        private readonly Node _cameraNode;
        private readonly Camera _camera;
        private readonly Viewport _viewport;
        private readonly InputMap _inputMap;

        public MyState(UrhoPluginApplication app) : base(app.Context)
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
        }

        public override void Update(float timeStep)
        {
            base.Update(timeStep);

            if (_inputMap.Evaluate("MouseRight") > 0.5f)
            {
                MouseMode = MouseMode.MmRelative;
                IsMouseVisible = false;

            }
            else
            {
                MouseMode = MouseMode.MmFree;
                IsMouseVisible = true;

            }
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
