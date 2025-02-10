namespace MonoBuild.Art;

public static class TextureHelper
{
    public static Texture2D CreateTextureFromTile(
        GraphicsDevice graphicsDevice,
        RawTile tile,
        Color[] palette
    )
    {
        int width = tile.Width;
        int height = tile.Height;

        var texture = new Texture2D(graphicsDevice, width, height);
        var colors = new Color[width * height];

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var srcIndex = x * height + y; // Transposed indexing
                var destIndex = y * width + x; // Standard row-major order

                var paletteIndex = tile.PixelData[srcIndex];
                colors[destIndex] = palette[paletteIndex];
            }
        }

        texture.SetData(colors);
        return texture;
    }
}
