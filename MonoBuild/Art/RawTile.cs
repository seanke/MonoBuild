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
    /// The X offset for tile alignment.
    /// </summary>
    public short XOffset { get; set; }

    /// <summary>
    /// The Y offset for tile alignment.
    /// </summary>
    public short YOffset { get; set; }

    /// <summary>
    /// The raw pixel data for the tile.
    /// </summary>
    public byte[] PixelData { get; set; }

    public int Picanm { get; set; }

    public RawTile()
    {
        PixelData = [];
    }

    /// <summary>
    /// Reads a tile entry from the given binary reader.
    /// </summary>
    /// <param name="reader">The binary reader containing tile data.</param>
    /// <param name="tileIndex">The tile index.</param>
    /// <returns>A new RawTile instance populated with the data.</returns>
    public static RawTile ReadTile(BinaryReader reader, int tileIndex)
    {
        var tile = new RawTile { TileIndex = tileIndex };

        tile.Width = reader.ReadInt16();
        tile.Height = reader.ReadInt16();
        tile.XOffset = reader.ReadInt16();
        tile.YOffset = reader.ReadInt16();

        int pixelDataSize = tile.Width * tile.Height;
        tile.PixelData = reader.ReadBytes(pixelDataSize);

        return tile;
    }
}
