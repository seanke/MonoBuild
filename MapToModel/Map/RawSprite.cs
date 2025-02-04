namespace MapToFbx.Map;

/// <summary>
/// Defines the structure for a sprite, including its position, appearance, and behavior.
/// </summary>
public class RawSprite
{
    /// <summary>
    /// The X coordinate of the sprite, determining its position in the game world.
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// The Y coordinate of the sprite.
    /// </summary>
    public int Y { get; set; }

    /// <summary>
    /// The Z coordinate of the sprite (height).
    /// </summary>
    public int Z { get; set; }

    /// <summary>
    /// A bitfield containing various flags related to the sprite's rendering and behavior, such as visibility and blocking.
    /// </summary>
    public short CStat { get; set; }

    /// <summary>
    /// The texture index for the sprite, referencing an entry in an ART file.
    /// </summary>
    public short Picnum { get; private set; }

    /// <summary>
    /// The shade offset for the sprite, affecting its brightness.
    /// </summary>
    public sbyte Shade { get; private set; }

    /// <summary>
    /// The palette number for the sprite texture, which can change the color palette used.
    /// </summary>
    public byte Pal { get; private set; }

    /// <summary>
    /// The clipping distance for the sprite, affecting collision detection and interaction radius.
    /// </summary>
    public byte Clipdist { get; private set; }

    /// <summary>
    /// A padding byte, not used for any game logic but necessary for structure alignment.
    /// </summary>
    public byte Filler { get; private set; }

    /// <summary>
    /// Horizontal and vertical repeat factors for the sprite texture, affecting its size.
    /// </summary>
    public byte XRepeat { get; private set; }

    /// <summary>
    /// Horizontal and vertical repeat factors for the sprite texture, affecting its size.
    /// </summary>
    public byte YRepeat { get; private set; }

    /// <summary>
    /// Horizontal and vertical offset for the sprite texture, used for animation and alignment.
    /// </summary>
    public sbyte XOffset { get; private set; }

    /// <summary>
    /// Horizontal and vertical offset for the sprite texture, used for animation and alignment.
    /// </summary>
    public sbyte YOffset { get; private set; }

    /// <summary>
    /// The sector number where the sprite is located, serving as an index to the sectors list.
    /// </summary>
    public short Sectnum { get; private set; }

    /// <summary>
    /// The status number of the sprite, defining its role or behavior, such as active, inactive, or enemy.
    /// </summary>
    public short Statnum { get; private set; }

    /// <summary>
    /// The angle the sprite is facing, determining its orientation.
    /// </summary>
    public short Ang { get; private set; }

    /// <summary>
    /// An identifier for the sprite's owner or source, which can be used for various game logic purposes.
    /// </summary>
    public short Owner { get; private set; }

    /// <summary>
    /// The velocity of the sprite in each direction, affecting its movement.
    /// </summary>
    public short Xvel { get; private set; }

    /// <summary>
    /// The velocity of the sprite in each direction, affecting its movement.
    /// </summary>
    public short Yvel { get; private set; }

    /// <summary>
    /// The velocity of the sprite in each direction, affecting its movement.
    /// </summary>
    public short Zvel { get; private set; }

    /// <summary>
    /// Game-specific tags for triggering events or actions when interacting with the sprite.
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

    /// <summary>
    /// Reads and constructs a sprite from a binary reader stream, typically for map loading.
    /// </summary>
    /// <param name="reader">The binary reader to read the sprite data from.</param>
    /// <returns>A new instance of a Sprite populated with data from the binary reader.</returns>
    public static RawSprite ReadSprite(BinaryReader reader) =>
        new()
        {
            X = reader.ReadInt32(),
            Y = reader.ReadInt32(),
            Z = reader.ReadInt32(),
            CStat = reader.ReadInt16(),
            Picnum = reader.ReadInt16(),
            Shade = reader.ReadSByte(),
            Pal = reader.ReadByte(),
            Clipdist = reader.ReadByte(),
            Filler = reader.ReadByte(),
            XRepeat = reader.ReadByte(),
            YRepeat = reader.ReadByte(),
            XOffset = reader.ReadSByte(),
            YOffset = reader.ReadSByte(),
            Sectnum = reader.ReadInt16(),
            Statnum = reader.ReadInt16(),
            Ang = reader.ReadInt16(),
            Owner = reader.ReadInt16(),
            Xvel = reader.ReadInt16(),
            Yvel = reader.ReadInt16(),
            Zvel = reader.ReadInt16(),
            Lotag = reader.ReadInt16(),
            Hitag = reader.ReadInt16(),
            Extra = reader.ReadInt16()
        };
}
