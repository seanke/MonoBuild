using System;
using Engine.Art;
using MonoBuild.Loaders;
using Color = Microsoft.Xna.Framework.Color;

namespace MonoBuild.ProofOfConcepts;

public class Debug(GraphicsDevice graphicsDevice) : IDisposable
{
    private Texture2D _texture;
    private SpriteBatch _spriteBatch;

    public void LoadContent(Tile tile)
    {
        // Load the texture from tile 0
        _texture = TextureLoader.LoadTextureFromTile(graphicsDevice, tile);

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
