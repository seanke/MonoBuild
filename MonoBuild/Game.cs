using System;
using System.IO;
using Microsoft.Xna.Framework.Input;
using MonoBuild.Map;
using MonoBuild.Player;

namespace MonoBuild;

public class Game : Microsoft.Xna.Framework.Game
{
    private MapRenderer _mapRenderer;
    private Camera _camera;
    private DebugInformation _debugInformation;

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
        GraphicsDevice.RasterizerState = new RasterizerState { CullMode = CullMode.None };

        base.Initialize();
    }

    protected override void LoadContent()
    {
        MapState.LoadMapFromFile(new FileInfo("E1L1.MAP"));
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

        _mapRenderer.Draw(_camera.View, _camera.Projection);
        _debugInformation.Draw();

        base.Draw(gameTime);
    }
}
