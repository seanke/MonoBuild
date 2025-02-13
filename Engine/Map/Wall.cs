using System.Collections.Immutable;
using System.Numerics;
using Engine.Art;

namespace Engine.Map;

// Define the Wall structure
public class Wall
{
    internal int Id { get; private set; }
    internal Vector2 PositionStart { get; }
    internal Vector2 PositionEnd => _map.Walls[RawPoint2].PositionStart;
    internal bool IsBottomTextureSwapped => (RawCStat & 2) != 0;
    internal bool IsWallAlignedBottom => (RawCStat & 3) != 0;

    /// <summary>
    /// The X coordinate of the wall's starting point.
    /// </summary>
    private int RawX { get; }

    /// <summary>
    /// The Y coordinate of the wall's starting point.
    /// </summary>
    private int RawY { get; }

    /// <summary>
    /// Index of the next wall in the sector, forming a linked list of walls.
    /// </summary>
    internal short RawPoint2 { get; }

    /// <summary>
    /// Index of the wall on the opposite side of this wall, used for connecting sectors.
    /// </summary>
    private short RawNextWall { get; }

    /// <summary>
    /// Index of the sector on the opposite side of this wall, helping in defining sector neighbors.
    /// </summary>
    private short RawNextSector { get; }

    /// <summary>
    /// A bitfield containing flags related to the wall's rendering and behavior.
    /// </summary>
    private short RawCStat { get; }

    /// <summary>
    /// The texture index for the wall, referencing an entry in an ART file.
    /// </summary>
    private short RawPicnum { get; }

    /// <summary>
    /// The texture index for the over (masking) texture, used for features like doors and windows.
    /// </summary>
    private short RawOverPicnum { get; }

    /// <summary>
    /// The shade offset for the wall, affecting its brightness.
    /// </summary>
    private sbyte RawShade { get; }

    /// <summary>
    /// The palette number for the wall texture, which can change the color palette used.
    /// </summary>
    private byte RawPal { get; }

    /// <summary>
    /// Horizontal and vertical repeat factors for the wall texture, affecting its size.
    /// </summary>
    private byte RawXRepeat { get; }

    /// <summary>
    /// Horizontal and vertical repeat factors for the wall texture, affecting its size.
    /// </summary>
    private byte RawYRepeat { get; }

    /// <summary>
    /// Horizontal and vertical panning offsets for the wall texture, used for alignment.
    /// </summary>
    private byte RawXPanning { get; }

    /// <summary>
    /// Vertical panning offset for the wall texture, used for alignment.
    /// </summary>
    private byte RawYPanning { get; }

    /// <summary>
    /// Game-specific tags for triggering events or actions when interacting with the wall.
    /// </summary>
    private short RawLotag { get; }

    /// <summary>
    /// Additional game-specific tag, similar to Lotag but for different or additional purposes.
    /// </summary>
    private short RawHitag { get; }

    /// <summary>
    /// An extra value for game-specific use, can hold any additional information needed by the game engine.
    /// </summary>
    private short RawExtra { get; }

    private bool YIsFlipped => (RawCStat & 9) != 0;
    private bool IsPortal => RawNextSector > -1;

    private Mesh? UpperWallMesh { get; set; }
    private Mesh? LowerWallMesh { get; set; }
    private Mesh? SolidWallMesh { get; set; }

    private Sector? NextSector => RawNextSector > -1 ? _map.Sectors[RawNextSector] : null;
    private Wall NextWall => RawNextWall > -1 ? _map.Walls[RawNextWall] : null;

    public List<Mesh> Meshes { get; private set; }

    private Tile Tile => _map.GroupFile.Tiles[RawPicnum];

    private readonly MapFile _map;
    private Sector _sector;

    public Wall(BinaryReader reader, int indexInRawWallsArray, MapFile map)
    {
        RawX = reader.ReadInt32();
        RawY = reader.ReadInt32();
        RawPoint2 = reader.ReadInt16();
        RawNextWall = reader.ReadInt16();
        RawNextSector = reader.ReadInt16();
        RawCStat = reader.ReadInt16();
        RawPicnum = reader.ReadInt16();
        RawOverPicnum = reader.ReadInt16();
        RawShade = reader.ReadSByte();
        RawPal = reader.ReadByte();
        RawXRepeat = reader.ReadByte();
        RawYRepeat = reader.ReadByte();
        RawXPanning = reader.ReadByte();
        RawYPanning = reader.ReadByte();
        RawLotag = reader.ReadInt16();
        RawHitag = reader.ReadInt16();
        RawExtra = reader.ReadInt16();

        PositionStart = new Vector2(
            RawX * Constants.BuildWidthUnitMeterRatio,
            RawY * Constants.BuildWidthUnitMeterRatio
        );

        Id = indexInRawWallsArray;
        _map = map;
    }

    internal void Load(Sector sector)
    {
        _sector = sector;
        UpperWallMesh = CreateUpperWallMesh();
        LowerWallMesh = CreateLowerWallMesh();
        SolidWallMesh = CreateSolidWallMesh();
        Meshes = new List<Mesh> { UpperWallMesh, LowerWallMesh, SolidWallMesh }
            .Where(wall => wall != null)
            .ToList();
    }

    private Mesh? CreateUpperWallMesh()
    {
        if (!IsPortal)
            return null;

        var top = _sector.CeilingYCoordinate;
        var bottom = NextSector.CeilingYCoordinate;

        if (bottom >= top)
            return null;

        var wallPoints = new List<Vector3>
        {
            new(PositionStart.X, bottom, PositionStart.Y),
            new(NextWall.PositionStart.X, bottom, NextWall.PositionStart.Y),
            new(NextWall.PositionStart.X, top, NextWall.PositionStart.Y),
            new(PositionStart.X, top, PositionStart.Y)
        };

        // Map point of the texture
        var vertices = wallPoints
            .Select(position => new Vertex(
                position,
                new Vector2(position.X / Tile.Width, position.Y / Tile.Height)
            ))
            .ToImmutableList();

        // Define indices
        var indices = ImmutableList.Create<int>(0, 1, 2, 2, 3, 0);

        // Create the mesh
        var mesh = new Mesh(vertices, indices, Tile, _sector, MeshType.UpperWall);
        return mesh;
    }

    private Mesh? CreateLowerWallMesh()
    {
        if (!IsPortal)
            return null;

        var bottom = _sector.FloorYCoordinate;
        var top = NextSector.FloorYCoordinate;

        if (bottom >= top)
            return null;

        var points = new List<Vector3>
        {
            new(PositionStart.X, bottom, PositionStart.Y),
            new(NextWall.PositionStart.X, bottom, NextWall.PositionStart.Y),
            new(NextWall.PositionStart.X, top, NextWall.PositionStart.Y),
            new(PositionStart.X, top, PositionStart.Y)
        };

        var tile = !IsBottomTextureSwapped ? Tile : _map.GroupFile.Tiles[RawOverPicnum];
        var minX = points.Min(p => p.X);
        var minY = points.Min(p => p.Y);
        var maxX = points.Max(p => p.X);
        var maxY = points.Max(p => p.Y);

        var width = maxX - minX;
        var height = maxY - minY;

        var vertices = points
            .Select(position => new Vertex(
                position,
                new Vector2(
                    (position.X - minX) / width, // Normalize X
                    (position.Y - minY) / height // Normalize Y
                )
            ))
            .ToImmutableList();

        // Define indices
        var indices = ImmutableList.Create<int>(0, 1, 2, 2, 3, 0);

        // Create the mesh
        var mesh = new Mesh(vertices, indices, tile, _sector, MeshType.LowerWall);
        return mesh;
    }

    private Mesh? CreateSolidWallMesh()
    {
        if (IsPortal)
            return null;

        var top = _sector.CeilingYCoordinate;
        var bottom = _sector.FloorYCoordinate;

        var wallPoints = new List<Vector3>
        {
            new(PositionStart.X, bottom, PositionStart.Y),
            new(PositionEnd.X, bottom, PositionEnd.Y),
            new(PositionEnd.X, top, PositionEnd.Y),
            new(PositionStart.X, top, PositionStart.Y)
        };

        // Map point of the texture
        var vertices = wallPoints
            .Select(position => new Vertex(
                position,
                new Vector2(position.X / Tile.Width, position.Y / Tile.Height)
            ))
            .ToImmutableList();

        // Define indices
        var indices = ImmutableList.Create<int>(0, 1, 2, 2, 3, 0);

        // Create the mesh
        var mesh = new Mesh(vertices, indices, Tile, _sector, MeshType.SolidWall);
        return mesh;
    }
}
