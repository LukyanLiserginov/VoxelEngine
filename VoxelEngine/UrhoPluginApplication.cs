using VoxelEngine.Utils;
using Urho3DNet;

namespace VoxelEngine
{
    /// <summary>
    ///     This class represents an Urho3D plugin application.
    /// </summary>
    [LoadablePlugin]
    public partial class UrhoPluginApplication : PluginApplication
    {
        private SharedPtr<CustomFormState> _constructState;
        private SharedPtr<BuildState> _buildState;
        private SharedPtr<MyMenuState> _myMenuState;
        private StateStack _stateStack;



        public UrhoPluginApplication(Context context) : base(context)
        {
        }


        /// <summary>
        ///     Gets a value indicating whether the game is running.
        /// </summary>
        public bool IsGameRunning => _constructState;
        private SharedPtr<DebugHud> _debugHud;

        /// <summary>
        /// Current game state or null if no game is running.
        /// </summary>

        protected override void Load()
        {
            Context.RegisterFactories(GetType().Assembly);
        }

        protected override void Unload()
        {
            Context.RemoveFactories(GetType().Assembly);
        }

        protected override void Suspend(Archive output)
        {
            base.Suspend(output);
        }

        protected override void Resume(Archive input, bool differentVersion)
        {
            base.Resume(input, differentVersion);
        }

        public override bool IsMain()
        {
            return true;
        }

        protected override void Start(bool isMain)
        {

            _stateStack = new StateStack(Context.GetSubsystem<StateManager>());

            _myMenuState = new MyMenuState(this);

            // Setup state manager.
            var stateManager = Context.GetSubsystem<StateManager>();
            stateManager.FadeInDuration = 0.1f;
            stateManager.FadeOutDuration = 0.1f;

            // Setup end enqueue splash screen.
            using (SharedPtr<SplashScreen> splash = new SplashScreen(Context))
            {
                splash.Ptr.Duration = 1.0f;
                splash.Ptr.BackgroundImage = Context.ResourceCache.GetResource<Texture2D>("Images/Background.png");
                splash.Ptr.ForegroundImage = Context.ResourceCache.GetResource<Texture2D>("Images/Splash.png");
                stateManager.EnqueueState(splash);
            }

            // Crate end enqueue main menu screen.
            _stateStack.Push(_myMenuState);

            // Show debug hud
            _debugHud = Context.Engine.CreateDebugHud();
            _debugHud.Ptr.Mode = DebugHudMode.DebughudShowAll;

            base.Start(isMain);
        }

        protected override void Stop()
        {
            _myMenuState?.Dispose();
            _constructState?.Dispose();
            _buildState?.Dispose();

            base.Stop();
        }


        public void ToConstruct()
        {
            _constructState?.Dispose();
            _constructState = new CustomFormState(this);
            _stateStack.Push(_constructState);
        }
        public void ToBuild(IVoxelShape shape) 
        {
            _buildState?.Dispose();
            _buildState = new BuildState(this, shape);
            _stateStack.Push(_buildState);

        }

        public void ContinueGame()
        {
            if (_constructState) _stateStack.Push(_constructState);
            if (_buildState) _stateStack.Push(_buildState);
        }

        public void Quit()
        {
            Context.Engine.Exit();
        }

        public void HandleBackKey()
        {
            if (_stateStack.State == _myMenuState.Ptr)
            {
                if (IsGameRunning)
                    ContinueGame();
                else
                    Quit();
            }
            else
            {
                if (_stateStack.Count > 1)
                    _stateStack.Pop();
            }
        }


    }
}