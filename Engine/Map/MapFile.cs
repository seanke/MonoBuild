using System.Text;

namespace Engine.Map;

/// <summary>
/// Represents a map file containing all the data needed to define a map, including sectors, walls, and sprites.
/// </summary>
public class MapFile
{
    /// <summary>
    /// The version number of the map file format. Different versions may have different features or limits.
    /// </summary>
    internal int RawMapVersion { get; }

    /// <summary>
    /// The X coordinate of the player's starting position in the map.
    /// </summary>
    internal int RawPosX { get; }

    /// <summary>
    /// The Y coordinate of the player's starting position in the map.
    /// </summary>
    internal int RawPosY { get; }

    /// <summary>
    /// The Z coordinate (height) of the player's starting position in the map.
    /// </summary>
    internal int RawPosZ { get; }

    /// <summary>
    /// The angle representing the direction the player is facing at the start.
    /// </summary>
    internal short RawAng { get; }

    /// <summary>
    /// The sector number where the player starts. This is an index to the Sectors list.
    /// </summary>
    internal short RawCurSectNum { get; }

    /// <summary>
    /// A list of Sector objects, each representing a sector in the map. Sectors are distinct areas or rooms.
    /// </summary>
    internal List<Sector> Sectors { get; }

    /// <summary>
    /// A list of Wall objects, with each wall defining boundaries of sectors or obstacles within the map.
    /// </summary>
    internal List<Wall> Walls { get; }

    /// <summary>
    /// A list of Sprite objects, representing items, enemies, or other interactable objects in the map.
    /// </summary>
    internal List<Sprite> Sprites { get; set; }

    internal int NumSectors { get; }
    internal int NumWalls { get; }
    internal int NumSprites { get; }

    /// <summary>
    /// Loads a map from a given stream, reading its content and constructing the map file structure.
    /// </summary>
    /// <param name="stream">The stream to load the map from.</param>
    /// <returns>A new MapFile instance populated with the data from the stream.</returns>
    public MapFile(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.Default, leaveOpen: true);

        RawMapVersion = reader.ReadInt32();
        RawPosX = reader.ReadInt32();
        RawPosY = reader.ReadInt32();
        RawPosZ = reader.ReadInt32();
        RawAng = reader.ReadInt16();
        RawCurSectNum = reader.ReadInt16();

        var numSectors = reader.ReadUInt16();
        NumSectors = numSectors;
        Sectors = Enumerable
            .Range(0, numSectors)
            .Select(id => new Sector(reader, id, this))
            .ToList();

        var numWalls = reader.ReadUInt16();
        NumWalls = numWalls;
        Walls = Enumerable.Range(0, numWalls).Select(id => new Wall(reader, id, this)).ToList();

        var numSprites = reader.ReadUInt16();
        NumSprites = numSprites;
        Sprites = Enumerable
            .Range(0, numSprites)
            .Select(id => new Sprite(reader, id, this))
            .ToList();
    }

    public MapFile(byte[] mapData)
        : this(new MemoryStream(mapData)) { }
}
