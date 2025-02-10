using System.IO;

namespace MonoBuild.Art;

/// <summary>
/// Represents a single tile (texture) within an ART file.
/// </summary>
public class RawTile
{
    /// <summary>
    /// The tile index.
    /// </summary>
    public int TileIndex { get; set; }

    /// <summary>
    /// The width of the tile.
    /// </summary>
    public short Width { get; set; }

    /// <summary>
    /// The height of the tile.
    /// </summary>
    public short Height { get; set; }

    /// <summary>
    /// The raw pixel data for the tile.
    /// </summary>
    public byte[] PixelData { get; set; } = [];

    public int Picanm { get; set; }

    /// <summary>
    /// Animation speed extracted from Picanm.
    /// </summary>
    public int AnimationSpeed => (Picanm >> 24) & 0xFF;

    /// <summary>
    /// Y-center offset extracted from Picanm.
    /// </summary>
    public sbyte YCenterOffset => (sbyte)((Picanm >> 16) & 0xFF);

    /// <summary>
    /// X-center offset extracted from Picanm.
    /// </summary>
    public sbyte XCenterOffset => (sbyte)((Picanm >> 8) & 0xFF);

    /// <summary>
    /// Animation number extracted from Picanm.
    /// </summary>
    public int AnimationNumber => Picanm & 0xFF;

    /// <summary>
    /// Animation type extracted from Picanm (last 2 bits).
    /// </summary>
    public int AnimationType => Picanm & 0x03;
}
