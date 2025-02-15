using System;
using System.IO;
using System.Linq;
using Engine;
using Engine.Group;
using Engine.Map;
using Microsoft.Xna.Framework.Input;
using MonoBuild.Player;
using MonoBuild.ProofOfConcepts;

namespace MonoBuild;

public class Game : Microsoft.Xna.Framework.Game
{
    private Camera _camera;
    private DebugInformation _debugInformation;
    private Skybox _skybox;

    private MapMesh _mapMesh;

    private RaycastPicker _raycastPicker;

    private Debug _debug;

    private GroupFile _groupFile = new(new FileInfo("DUKE3D.GRP"));

    private MapFile _map;

    public Game()
    {
        var graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = false; // Hide mouse for better FPS controls

        IsFixedTimeStep = true; // Locks game loop to a fixed interval
        TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 60.0); // 60 FPS
        graphics.SynchronizeWithVerticalRetrace = true; // Enables VSync
    }

    protected override void Initialize()
    {
        //_mapRenderer = new MapRenderer(GraphicsDevice);
        _camera = new Camera(GraphicsDevice, new Vector3(0, 0, 10));
        _debugInformation = new DebugInformation(GraphicsDevice, _camera);
        _skybox = new Skybox(GraphicsDevice);
        _mapMesh = new MapMesh(GraphicsDevice);
        _raycastPicker = new RaycastPicker(GraphicsDevice);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _groupFile = new GroupFile(new FileInfo("DUKE3D.GRP"));
        _map = new MapFile(new FileInfo("E1L1.MAP"), _groupFile);
        _mapMesh.LoadContent(_map);

        _debug = new Debug(GraphicsDevice);
        _debug.LoadContent(_groupFile.Tiles[1]);

        _debugInformation.LoadContent(Content);
    }

    protected override void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();

        // Exit if ESC is pressed
        if (keyboard.IsKeyDown(Keys.Escape))
            Exit();

        if (keyboard.IsKeyDown(Keys.Space))
        {
            _map = new MapFile(new FileInfo("E1L1.MAP"), _groupFile);
            _mapMesh.LoadContent(_map);
        }

        if (keyboard.IsKeyDown(Keys.Up))
            Utils.Test += 0.5f;

        if (keyboard.IsKeyDown(Keys.Down))
            Utils.Test -= 0.5f;

        _camera.Update(gameTime);

        _raycastPicker.Update(_camera.View, _camera.Projection);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        GraphicsDevice.RasterizerState = new RasterizerState { CullMode = CullMode.None };
        GraphicsDevice.DepthStencilState = DepthStencilState.Default;

        //GraphicsDevice.RasterizerState = new RasterizerState { FillMode = FillMode.WireFrame };

        _skybox.Draw(_camera.View, _camera.Projection);
        _mapMesh.Draw(_camera.View, _camera.Projection);
        _debugInformation.Draw();
        _debug.Draw();

        base.Draw(gameTime);
    }

    protected override void Dispose(bool disposing)
    {
        _mapMesh.Dispose();
        _debugInformation.Dispose();
        _skybox.Dispose();
        base.Dispose(disposing);
    }
}
