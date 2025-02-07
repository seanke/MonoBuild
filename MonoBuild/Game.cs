using System;
using System.IO;
using Microsoft.Xna.Framework.Input;
using MonoBuild.Art;
using MonoBuild.Map;
using MonoBuild.Player;
using MonoBuild.Render;

namespace MonoBuild;

public class Game : Microsoft.Xna.Framework.Game
{
    private MapRenderer _mapRenderer;
    private Camera _camera;
    private DebugInformation _debugInformation;
    private Skybox _skybox;

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
        _mapRenderer = new MapRenderer(GraphicsDevice);
        _camera = new Camera(GraphicsDevice, new Vector3(0, 0, 10));
        _debugInformation = new DebugInformation(GraphicsDevice, _camera);
        _skybox = new Skybox(GraphicsDevice);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        State.LoadGroupFromFile(new FileInfo("DUKE3D.GRP"));
        var group = State.LoadedRawGroup;

        if (group == null)
            throw new Exception("Failed to load group file.");

        foreach (var file in group.Lumps)
        {
            Console.WriteLine($"Loaded {file.FileName} ({file.Data.Length} bytes)");

            if (file.FileName.EndsWith(".ART"))
            {
                var art = RawArtFile.LoadFromBytes(file.Data);
                Console.WriteLine("ART file loaded with {0} tiles.", art.Tiles.Count);
            }
        }

        //State.LoadMapFromFile(new FileInfo("E1L1.MAP"));
        State.LoadMapFromBytes(group.Lumps.Find(x => x.FileName == "E1L1.MAP").Data);

        _mapRenderer.LoadContent();
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

        _skybox.Draw(_camera.View, _camera.Projection);
        _mapRenderer.Draw(_camera.View, _camera.Projection);
        _debugInformation.Draw();

        base.Draw(gameTime);
    }

    protected override void Dispose(bool disposing)
    {
        _mapRenderer.Dispose();
        base.Dispose(disposing);
    }
}
