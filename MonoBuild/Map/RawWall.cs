using System.IO;

namespace MonoBuild.Map;

// Define the Wall structure
public class RawWall
{
    /// <summary>
    /// The X coordinate of the wall's starting point.
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// The Y coordinate of the wall's starting point.
    /// </summary>
    public int Y { get; set; }

    /// <summary>
    /// Index of the next wall in the sector, forming a linked list of walls.
    /// </summary>
    public short Point2 { get; set; }

    /// <summary>
    /// Index of the wall on the opposite side of this wall, used for connecting sectors.
    /// </summary>
    public short NextWall { get; set; }

    /// <summary>
    /// Index of the sector on the opposite side of this wall, helping in defining sector neighbors.
    /// </summary>
    public short NextSector { get; set; }

    /// <summary>
    /// A bitfield containing flags related to the wall's rendering and behavior.
    /// </summary>
    public short CStat { get; set; }

    /// <summary>
    /// The texture index for the wall, referencing an entry in an ART file.
    /// </summary>
    public short Picnum { get; internal set; }

    /// <summary>
    /// The texture index for the over (masking) texture, used for features like doors and windows.
    /// </summary>
    public short OverPicnum { get; internal set; }

    /// <summary>
    /// The shade offset for the wall, affecting its brightness.
    /// </summary>
    public sbyte Shade { get; internal set; }

    /// <summary>
    /// The palette number for the wall texture, which can change the color palette used.
    /// </summary>
    public byte Pal { get; internal set; }

    /// <summary>
    /// Horizontal and vertical repeat factors for the wall texture, affecting its size.
    /// </summary>
    public byte XRepeat { get; internal set; }

    /// <summary>
    /// Horizontal and vertical repeat factors for the wall texture, affecting its size.
    /// </summary>
    public byte YRepeat { get; internal set; }

    /// <summary>
    /// Horizontal and vertical panning offsets for the wall texture, used for alignment.
    /// </summary>
    public byte XPanning { get; internal set; }

    /// <summary>
    /// Vertical panning offset for the wall texture, used for alignment.
    /// </summary>
    public byte YPanning { get; private set; }

    /// <summary>
    /// Game-specific tags for triggering events or actions when interacting with the wall.
    /// </summary>
    public short Lotag { get; private set; }

    /// <summary>
    /// Additional game-specific tag, similar to Lotag but for different or additional purposes.
    /// </summary>
    public short Hitag { get; private set; }

    /// <summary>
    /// An extra value for game-specific use, can hold any additional information needed by the game engine.
    /// </summary>
    public short Extra { get; private set; }

    public int Id { get; set; }

    public static RawWall ReadWall(BinaryReader reader, int id) =>
        new()
        {
            Id = id,
            X = reader.ReadInt32(),
            Y = reader.ReadInt32(),
            Point2 = reader.ReadInt16(),
            NextWall = reader.ReadInt16(),
            NextSector = reader.ReadInt16(),
            CStat = reader.ReadInt16(),
            Picnum = reader.ReadInt16(),
            OverPicnum = reader.ReadInt16(),
            Shade = reader.ReadSByte(),
            Pal = reader.ReadByte(),
            XRepeat = reader.ReadByte(),
            YRepeat = reader.ReadByte(),
            XPanning = reader.ReadByte(),
            YPanning = reader.ReadByte(),
            Lotag = reader.ReadInt16(),
            Hitag = reader.ReadInt16(),
            Extra = reader.ReadInt16()
        };
}
