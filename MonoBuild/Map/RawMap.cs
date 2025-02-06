using System.IO;
using System.Linq;
using System.Text;

namespace MonoBuild.Map;

/// <summary>
/// Represents a map file containing all the data needed to define a map, including sectors, walls, and sprites.
/// </summary>
public class RawMap
{
    /// <summary>
    /// The version number of the map file format. Different versions may have different features or limits.
    /// </summary>
    public int MapVersion { get; set; }

    /// <summary>
    /// The X coordinate of the player's starting position in the map.
    /// </summary>
    public int PosX { get; set; }

    /// <summary>
    /// The Y coordinate of the player's starting position in the map.
    /// </summary>
    public int PosY { get; set; }

    /// <summary>
    /// The Z coordinate (height) of the player's starting position in the map.
    /// </summary>
    public int PosZ { get; set; }

    /// <summary>
    /// The angle representing the direction the player is facing at the start.
    /// </summary>
    public short Ang { get; set; }

    /// <summary>
    /// The sector number where the player starts. This is an index to the Sectors list.
    /// </summary>
    public short CurSectNum { get; set; }

    /// <summary>
    /// A list of Sector objects, each representing a sector in the map. Sectors are distinct areas or rooms.
    /// </summary>
    public List<RawSector> Sectors { get; set; }

    /// <summary>
    /// A list of Wall objects, with each wall defining boundaries of sectors or obstacles within the map.
    /// </summary>
    public List<RawWall> Walls { get; set; }

    /// <summary>
    /// A list of Sprite objects, representing items, enemies, or other interactable objects in the map.
    /// </summary>
    public List<RawSprite> Sprites { get; set; }

    public RawMap()
    {
        Sectors = [];
        Walls = [];
        Sprites = [];
    }

    /// <summary>
    /// Loads a map from a given stream, reading its content and constructing the map file structure.
    /// </summary>
    /// <param name="stream">The stream to load the map from.</param>
    /// <returns>A new MapFile instance populated with the data from the stream.</returns>
    public static RawMap LoadFromStream(Stream stream)
    {
        var map = new RawMap();

        using var reader = new BinaryReader(stream, Encoding.Default, leaveOpen: true);

        map.MapVersion = reader.ReadInt32();
        map.PosX = reader.ReadInt32();
        map.PosY = reader.ReadInt32();
        map.PosZ = reader.ReadInt32();
        map.Ang = reader.ReadInt16();
        map.CurSectNum = reader.ReadInt16();

        var numSectors = reader.ReadUInt16();
        map.Sectors = Enumerable
            .Range(0, numSectors)
            .Select(id => RawSector.ReadSector(reader, id))
            .ToList();

        var numWalls = reader.ReadUInt16();
        map.Walls = Enumerable
            .Range(0, numWalls)
            .Select(id => RawWall.ReadWall(reader, id))
            .ToList();

        var numSprites = reader.ReadUInt16();
        map.Sprites = Enumerable
            .Range(0, numSprites)
            .Select(id => RawSprite.ReadSprite(reader, id))
            .ToList();

        return map;
    }

    public static RawMap LoadFromBytes(byte[] mapData)
    {
        using var stream = new MemoryStream(mapData);
        return LoadFromStream(stream);
    }
}
