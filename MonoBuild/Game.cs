using System;
using System.IO;
using System.Linq;
using Engine.Group;
using Microsoft.Xna.Framework.Input;
using MonoBuild.Player;
using MonoBuild.Pocs;
using MonoBuild.ProofOfConcepts;

namespace MonoBuild;

public class Game : Microsoft.Xna.Framework.Game
{
    private Camera _camera;
    private DebugInformation _debugInformation;
    private Skybox _skybox;

    private Debug _debug;

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

        base.Initialize();
    }

    protected override void LoadContent()
    {
        var groupFile = new GroupFile(new FileInfo("DUKE3D.GRP"));

        _debug = new Debug(GraphicsDevice);
        _debug.LoadContent(groupFile.Tiles[0]);

        //State.LoadMapFromFile(new FileInfo("E1L1.MAP"));
        //State.LoadMapFromBytes(group.Lumps.Find(x => x.FileName == "E1L1.MAP").Data);

        //_mapRenderer.LoadContent();
        _debugInformation.LoadContent(Content);
    }

    protected override void Update(GameTime gameTime)
    {
        // Exit if ESC is pressed
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _camera.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        GraphicsDevice.RasterizerState = new RasterizerState { CullMode = CullMode.None };
        GraphicsDevice.DepthStencilState = DepthStencilState.Default;

        //GraphicsDevice.RasterizerState = new RasterizerState { FillMode = FillMode.WireFrame };

        _skybox.Draw(_camera.View, _camera.Projection);
        //_mapRenderer.Draw(_camera.View, _camera.Projection);
        _debugInformation.Draw();

        _debug.Draw();

        base.Draw(gameTime);
    }

    protected override void Dispose(bool disposing)
    {
        //_mapRenderer.Dispose();
        base.Dispose(disposing);
    }
}
