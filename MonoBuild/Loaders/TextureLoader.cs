using System.Linq;
using Engine.Art;
using Color = Microsoft.Xna.Framework.Color;

namespace MonoBuild.Loaders;

public static class TextureLoader
{
    private static Dictionary<int, Texture2D> _textures = new();

    public static Texture2D LoadTextureFromTile(GraphicsDevice graphicsDevice, Tile tile)
    {
        if (_textures.ContainsKey(tile.Id))
            return _textures[tile.Id];

        var textureDate = tile
            .PixelData.AsEnumerable()
            .Select(c => new Color(c.Red, c.Green, c.Blue))
            .ToArray();

        var texture = new Texture2D(graphicsDevice, tile.Width, tile.Height);
        texture.SetData(textureDate);

        _textures.Add(tile.Id, texture);

        return texture;
    }
}
