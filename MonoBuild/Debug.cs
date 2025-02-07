using MonoBuild.Art;

namespace MonoBuild;

public class Debug(GraphicsDevice graphicsDevice)
{
    private Texture2D _texture;
    private SpriteBatch _spriteBatch;

    public void LoadContent()
    {
        // Load the texture from tile 0
        _texture = TextureHelper.CreateTextureFromTile(
            graphicsDevice,
            State.LoadedGroupArt.Tiles[0],
            State.LoadedPaletteFile.Colors
        );

        // Initialize SpriteBatch for drawing
        _spriteBatch = new SpriteBatch(graphicsDevice);
    }

    public void Draw()
    {
        if (_texture == null)
            return; // Ensure texture is loaded before drawing

        // Begin drawing
        _spriteBatch.Begin();

        // Draw texture at position (0,0)
        _spriteBatch.Draw(_texture, new Vector2(0, 0), Color.White);

        // End drawing
        _spriteBatch.End();
    }

    public void Dispose()
    {
        _texture?.Dispose();
        _spriteBatch?.Dispose();
    }
}
