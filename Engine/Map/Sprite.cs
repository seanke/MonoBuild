namespace Engine.Map;

/// <summary>
/// Defines the structure for a sprite, including its position, appearance, and behavior.
/// </summary>
public class Sprite
{
    /// <summary>
    /// The X coordinate of the sprite, determining its position in the game world.
    /// </summary>
    internal int RawX { get; private set; }

    /// <summary>
    /// The Y coordinate of the sprite.
    /// </summary>
    internal int RawY { get; private set; }

    /// <summary>
    /// The Z coordinate of the sprite (height).
    /// </summary>
    internal int RawZ { get; private set; }

    /// <summary>
    /// A bitfield containing various flags related to the sprite's rendering and behavior, such as visibility and blocking.
    /// </summary>
    internal short RawCStat { get; private set; }

    /// <summary>
    /// The texture index for the sprite, referencing an entry in an ART file.
    /// </summary>
    internal short RawPicnum { get; private set; }

    /// <summary>
    /// The shade offset for the sprite, affecting its brightness.
    /// </summary>
    internal sbyte RawShade { get; private set; }

    /// <summary>
    /// The palette number for the sprite texture, which can change the color palette used.
    /// </summary>
    internal byte RawPal { get; private set; }

    /// <summary>
    /// The clipping distance for the sprite, affecting collision detection and interaction radius.
    /// </summary>
    internal byte RawClipdist { get; private set; }

    /// <summary>
    /// A padding byte, not used for any game logic but necessary for structure alignment.
    /// </summary>
    internal byte RawFiller { get; private set; }

    /// <summary>
    /// Horizontal and vertical repeat factors for the sprite texture, affecting its size.
    /// </summary>
    internal byte RawXRepeat { get; private set; }

    /// <summary>
    /// Horizontal and vertical repeat factors for the sprite texture, affecting its size.
    /// </summary>
    internal byte RawYRepeat { get; private set; }

    /// <summary>
    /// Horizontal and vertical offset for the sprite texture, used for animation and alignment.
    /// </summary>
    internal sbyte RawXOffset { get; private set; }

    /// <summary>
    /// Horizontal and vertical offset for the sprite texture, used for animation and alignment.
    /// </summary>
    internal sbyte RawYOffset { get; private set; }

    /// <summary>
    /// The sector number where the sprite is located, serving as an index to the sectors list.
    /// </summary>
    internal short RawSectnum { get; private set; }

    /// <summary>
    /// The status number of the sprite, defining its role or behavior, such as active, inactive, or enemy.
    /// </summary>
    internal short RawStatnum { get; private set; }

    /// <summary>
    /// The angle the sprite is facing, determining its orientation.
    /// </summary>
    internal short RawAng { get; private set; }

    /// <summary>
    /// An identifier for the sprite's owner or source, which can be used for various game logic purposes.
    /// </summary>
    internal short RawOwner { get; private set; }

    /// <summary>
    /// The velocity of the sprite in each direction, affecting its movement.
    /// </summary>
    internal short RawXvel { get; private set; }

    /// <summary>
    /// The velocity of the sprite in each direction, affecting its movement.
    /// </summary>
    internal short RawYvel { get; private set; }

    /// <summary>
    /// The velocity of the sprite in each direction, affecting its movement.
    /// </summary>
    internal short RawZvel { get; private set; }

    /// <summary>
    /// Game-specific tags for triggering events or actions when interacting with the sprite.
    /// </summary>
    internal short RawLotag { get; private set; }

    /// <summary>
    /// Additional game-specific tag, similar to Lotag but for different or additional purposes.
    /// </summary>
    internal short RawHitag { get; private set; }

    /// <summary>
    /// An extra value for game-specific use, can hold any additional information needed by the game engine.
    /// </summary>
    internal short RawExtra { get; private set; }

    internal int Id { get; private set; }

    private readonly MapFile _map;

    /// <summary>
    /// Reads and constructs a sprite from a binary reader stream, typically for map loading.
    /// </summary>
    /// <param name="reader">The binary reader to read the sprite data from.</param>
    /// <param name="map"></param>
    /// <returns>A new instance of a Sprite populated with data from the binary reader.</returns>
    internal Sprite(BinaryReader reader, int indexInRawSpriteArray, MapFile map)
    {
        RawX = reader.ReadInt32();
        RawY = reader.ReadInt32();
        RawZ = reader.ReadInt32();
        RawCStat = reader.ReadInt16();
        RawPicnum = reader.ReadInt16();
        RawShade = reader.ReadSByte();
        RawPal = reader.ReadByte();
        RawClipdist = reader.ReadByte();
        RawFiller = reader.ReadByte();
        RawXRepeat = reader.ReadByte();
        RawYRepeat = reader.ReadByte();
        RawXOffset = reader.ReadSByte();
        RawYOffset = reader.ReadSByte();
        RawSectnum = reader.ReadInt16();
        RawStatnum = reader.ReadInt16();
        RawAng = reader.ReadInt16();
        RawOwner = reader.ReadInt16();
        RawXvel = reader.ReadInt16();
        RawYvel = reader.ReadInt16();
        RawZvel = reader.ReadInt16();
        RawLotag = reader.ReadInt16();
        RawHitag = reader.ReadInt16();
        RawExtra = reader.ReadInt16();

        Id = indexInRawSpriteArray;
        _map = map;
    }
}
