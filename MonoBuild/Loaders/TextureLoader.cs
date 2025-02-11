using System.Linq;
using Engine.Art;
using Color = Microsoft.Xna.Framework.Color;

namespace MonoBuild.Loaders;

public static class TextureLoader
{
    public static Texture2D CreateTextureFromTile(GraphicsDevice graphicsDevice, Tile tile)
    {
        var textureDate = tile
            .PixelData.AsEnumerable()
            .Select(c => new Color(c.Red, c.Green, c.Blue))
            .ToArray();

        var texture = new Texture2D(graphicsDevice, tile.Width, tile.Height);
        texture.SetData(textureDate);
        return texture;
    }
}
