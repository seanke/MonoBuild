using Microsoft.Xna.Framework.Content;
using MonoBuild.Player;

namespace MonoBuild.Pocs;

public class DebugInformation
{
    private readonly GraphicsDevice _graphicsDevice;
    private SpriteBatch _spriteBatch;
    private SpriteFont _font;
    private Camera _camera;

    public DebugInformation(GraphicsDevice graphicsDevice, Camera camera)
    {
        _graphicsDevice = graphicsDevice;
        _camera = camera;
    }

    public void LoadContent(ContentManager content)
    {
        // Load a font from the content pipeline
        _font = content.Load<SpriteFont>("DebugFont"); // Ensure you have a "DebugFont" in your content
        _spriteBatch = new SpriteBatch(_graphicsDevice);
    }

    public void Draw()
    {
        if (_font == null || _spriteBatch == null)
            return; // Prevent crashes if LoadContent wasn't called

        _spriteBatch.Begin();

        // Display camera position
        var positionText =
            $"Position: X={_camera.Position.X:F2}, Y={_camera.Position.Y:F2}, Z={_camera.Position.Z:F2}";

        _spriteBatch.DrawString(_font, positionText, new Vector2(10, 10), Color.White);

        // Display camera rotation (yaw and pitch)
        var rotationText =
            $"Rotation: Yaw={MathHelper.ToDegrees(_camera.Yaw):F2}, Pitch={MathHelper.ToDegrees(_camera.Pitch):F2}";
        _spriteBatch.DrawString(_font, rotationText, new Vector2(10, 30), Color.White);

        _spriteBatch.End();
    }
}
