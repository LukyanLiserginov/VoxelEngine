using ImGuiNet;
using System;
using System.Collections.Generic;
using System.Text;
using Urho3DNet;

namespace StarCubes
{
    internal partial class MyMenuState : ApplicationState
    {
        protected readonly Scene _scene;
        private readonly UrhoPluginApplication _app;
        private readonly Node _cameraNode;
        private readonly Camera _camera;
        private readonly Viewport _viewport;
        private bool _menu = true;
        private Vector2 _screenSize;

        public MyMenuState(UrhoPluginApplication app) : base(app.Context)
        {
            MouseMode = MouseMode.MmFree;
            IsMouseVisible = true;
            _screenSize = new Vector2(Graphics.Size.X, Graphics.Size.Y);
            _app = app;
            _scene = Context.CreateObject<Scene>();
            _scene.Load("Scenes/MyMenu.scene");
            _cameraNode = _scene.FindChild("CameraNode", true);
            _camera = _cameraNode.GetComponent<Camera>();
            _viewport = Context.CreateObject<Viewport>();
            _viewport.Scene = _scene;
            _viewport.Camera = _camera;
            SetViewport(0, _viewport);
        }

        public override void Update(float timeStep)
        {
            ImGui.SetNextWindowPos(new Vector2((_screenSize.X - 200) / 2, _screenSize.Y / 2 - 200));
            ImGui.Begin("Menu", ref _menu, ImGuiWindowFlags.ImGuiWindowFlagsNoBackground | ImGuiWindowFlags.ImGuiWindowFlagsNoDecoration | ImGuiWindowFlags.ImGuiWindowFlagsNoDocking | ImGuiWindowFlags.ImGuiWindowFlagsNoResize);
            if (ImGui.Button("Start Game", new Vector2(200, 60))) _app.ToNewGame();
            if (ImGui.Button("Exit", new Vector2(200, 60))) _app.Quit();
            ImGui.End();
        }

        public override void Activate(StringVariantMap bundle)
        {
            SubscribeToEvent(E.KeyUp, HandleKeyUp);
            base.Activate(bundle);
        }

        /// <summary>
        /// Game state deactivation handler.
        /// </summary>
        public override void Deactivate()
        {
            _menu = false;
            UnsubscribeFromEvent(E.KeyUp);
            base.Deactivate();
        }

        /// <summary>
        /// Key up handler to navigate back in the game state hierarchy.
        /// </summary>
        /// <param name="args"></param>
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
