using ImGuiNet;
using System;
using System.Collections.Generic;
using System.Text;
using Urho3DNet;

namespace VoxelEngine
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

        public IVoxelShape[] VoxelShapes = { new SierpinskiTetrahedron(), new Sphere(), new SpongeCorner(), new Metaballs() };
        private string[] _voxelShapes = { "SierpinskiTetrahedron", "Sphere", "SpongeCorner","Metaballs"};
        private int _selectedItemIndex = 0;

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
            ImGui.SetNextWindowSize(new Vector2(250, _screenSize.Y/2));
            ImGui.SetNextWindowPos(new Vector2((_screenSize.X - 200) / 2, _screenSize.Y / 2 - 200));
            ImGui.Begin("Menu", ref _menu, ImGuiWindowFlags.ImGuiWindowFlagsNoBackground | ImGuiWindowFlags.ImGuiWindowFlagsNoDecoration | ImGuiWindowFlags.ImGuiWindowFlagsNoDocking | ImGuiWindowFlags.ImGuiWindowFlagsNoResize);
            if (ImGui.BeginCombo("Select form", _voxelShapes[_selectedItemIndex]))
            {
                for (int i = 0; i < _voxelShapes.Length; i++)
                {
                    bool isSelected = (_selectedItemIndex == i);
                    if (ImGui.Selectable(_voxelShapes[i], isSelected))
                    {
                        _selectedItemIndex = i; // обновляем индекс
                    }
                    if (isSelected)
                    {
                        ImGui.SetItemDefaultFocus();
                    }
                }
                ImGui.EndCombo();
            }
            if (ImGui.Button("Build form", new Vector2(200, 60))) _app.ToBuild(VoxelShapes[_selectedItemIndex]);
            //if (ImGui.Button("Hand draw (beta)", new Vector2(200, 60))) _app.ToConstruct();
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
