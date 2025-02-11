namespace Engine.Art;

/// <summary>
/// Represents a single tile (texture) within an ART file.
/// </summary>
public class Tile
{
    /// <summary>
    /// The tile index.
    /// </summary>
    internal int RawTileIndex { get; }

    /// <summary>
    /// The width of the tile.
    /// </summary>
    internal short RawWidth { get; }

    /// <summary>
    /// The height of the tile.
    /// </summary>
    internal short RawHeight { get; }

    /// <summary>
    /// The raw pixel data for the tile.
    /// </summary>
    internal byte[] RawPixelData { get; } = [];

    internal int RawPicanm { get; }

    /// <summary>
    /// Animation speed extracted from Picanm.
    /// </summary>
    internal int RawAnimationSpeed => (RawPicanm >> 24) & 0xFF;

    /// <summary>
    /// Y-center offset extracted from Picanm.
    /// </summary>
    internal sbyte RawYCenterOffset => (sbyte)((RawPicanm >> 16) & 0xFF);

    /// <summary>
    /// X-center offset extracted from Picanm.
    /// </summary>
    internal sbyte RawXCenterOffset => (sbyte)((RawPicanm >> 8) & 0xFF);

    /// <summary>
    /// Animation number extracted from Picanm.
    /// </summary>
    internal int RawAnimationNumber => RawPicanm & 0xFF;

    /// <summary>
    /// Animation type extracted from Picanm (last 2 bits).
    /// </summary>
    internal int RawAnimationType => RawPicanm & 0x03;

    internal Palette Palette { get; }

    public short Width => RawWidth;
    public short Height => RawHeight;

    public Color[] PixelData
    {
        get
        {
            var colors = new Color[Width * Height];

            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    var srcIndex = x * Height + y; // Transposed indexing
                    var destIndex = y * Width + x; // Standard row-major order
                    colors[destIndex] = new Color(RawPixelData[srcIndex], Palette);
                }
            }

            return colors;
        }
    }

    internal Tile(
        int rawIndex,
        short rawWidth,
        short rawHeight,
        int rawPicanm,
        byte[] rawTileData,
        Palette palette
    )
    {
        RawTileIndex = rawIndex;
        RawWidth = (short)rawWidth;
        RawHeight = (short)rawHeight;
        RawPicanm = rawPicanm;
        RawPixelData = rawTileData;
        Palette = palette;
    }
}
