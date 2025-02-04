namespace MapToFbx.Map;

/// <summary>
/// Defines the structure for a sector, including its geometry, appearance, and special attributes.
/// </summary>
public class RawSector
{
    /// <summary>
    /// Index to the first wall in the sector, used to identify where the sector's wall definitions start.
    /// </summary>
    public short WallPtr { get; set; }

    /// <summary>
    /// The number of walls in this sector, determining how many walls are associated with this sector.
    /// </summary>
    public short WallNum { get; set; }

    /// <summary>
    /// Z-coordinate (height) of the sector's ceiling at its first point.
    /// </summary>
    public int CeilingZ { get; set; }

    /// <summary>
    /// Z-coordinate (height) of the sector's floor at its first point.
    /// </summary>
    public int FloorZ { get; set; }

    /// <summary>
    /// Bitfield containing various flags related to the sector's ceiling, such as parallaxing and slope.
    /// </summary>
    public short CeilingStat { get; set; }

    /// <summary>
    /// Bitfield containing various flags related to the sector's floor, such as parallaxing and slope.
    /// </summary>
    public short FloorStat { get; set; }

    /// <summary>
    /// Texture index for the ceiling, referencing an entry in an ART file.
    /// </summary>
    public short CeilingPicnum { get; private set; }

    /// <summary>
    /// Shade offset for the ceiling, affecting its brightness.
    /// </summary>
    public sbyte CeilingShade { get; private set; }

    /// <summary>
    /// Slope of the ceiling, determining how steep the slope is if the ceiling is sloped.
    /// </summary>
    public short CeilingHeinum { get; private set; }

    /// <summary>
    /// Palette number for the ceiling texture, which can change the color palette used.
    /// </summary>
    public byte CeilingPal { get; private set; }

    /// <summary>
    /// Horizontal panning offset for the ceiling texture, useful for texture alignment.
    /// </summary>
    public byte CeilingXpanning { get; private set; }

    /// <summary>
    /// Vertical panning offset for the ceiling texture, useful for texture alignment.
    /// </summary>
    public byte CeilingYpanning { get; private set; }

    /// <summary>
    /// Texture index for the floor, referencing an entry in an ART file.
    /// </summary>
    public short FloorPicnum { get; private set; }

    /// <summary>
    /// Slope of the floor, determining how steep the slope is if the floor is sloped.
    /// </summary>
    public short FloorHeinum { get; private set; }

    /// <summary>
    /// Shade offset for the floor, affecting its brightness.
    /// </summary>
    public sbyte FloorShade { get; private set; }

    /// <summary>
    /// Palette number for the floor texture, which can change the color palette used.
    /// </summary>
    public byte FloorPal { get; private set; }

    /// <summary>
    /// Horizontal panning offset for the floor texture, useful for texture alignment.
    /// </summary>
    public byte FloorXpanning { get; private set; }

    /// <summary>
    /// Vertical panning offset for the floor texture, useful for texture alignment.
    /// </summary>
    public byte FloorYpanning { get; private set; }

    /// <summary>
    /// Affects how quickly sectors fade to darkness with distance. Lower values result in quicker darkening.
    /// </summary>
    public byte Visibility { get; private set; }

    /// <summary>
    /// Padding byte, not used for any game logic but necessary for structure alignment.
    /// </summary>
    public byte Filler { get; private set; }

    /// <summary>
    /// Game-specific use, often for triggering events or actions within the sector.
    /// </summary>
    public short Lotag { get; private set; }

    /// <summary>
    /// Additional tag for game-specific use, similar to Lotag but for different or additional purposes.
    /// </summary>
    public short Hitag { get; private set; }

    /// <summary>
    /// An extra value for game-specific use, can hold any additional information needed by the game engine.
    /// </summary>
    public short Extra { get; private set; }

    /// <summary>
    /// Reads and constructs a sector from a binary reader stream, typically used for map loading.
    /// </summary>
    /// <param name="reader">The binary reader to read the sector data from.</param>
    /// <returns>A new instance of a Sector populated with data from the binary reader.</returns>
    public static RawSector ReadSector(BinaryReader reader) =>
        new()
        {
            WallPtr = reader.ReadInt16(),
            WallNum = reader.ReadInt16(),
            CeilingZ = reader.ReadInt32(),
            FloorZ = reader.ReadInt32(),
            CeilingStat = reader.ReadInt16(),
            FloorStat = reader.ReadInt16(),
            CeilingPicnum = reader.ReadInt16(),
            CeilingHeinum = reader.ReadInt16(),
            CeilingShade = reader.ReadSByte(),
            CeilingPal = reader.ReadByte(),
            CeilingXpanning = reader.ReadByte(),
            CeilingYpanning = reader.ReadByte(),
            FloorPicnum = reader.ReadInt16(),
            FloorHeinum = reader.ReadInt16(),
            FloorShade = reader.ReadSByte(),
            FloorPal = reader.ReadByte(),
            FloorXpanning = reader.ReadByte(),
            FloorYpanning = reader.ReadByte(),
            Visibility = reader.ReadByte(),
            Filler = reader.ReadByte(),
            Lotag = reader.ReadInt16(),
            Hitag = reader.ReadInt16(),
            Extra = reader.ReadInt16()
        };
}
