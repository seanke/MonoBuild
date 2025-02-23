using System.Collections.Immutable;
using System.Numerics;
using Engine.Art;

namespace Engine.Map;

// Define the Wall structure
public class Wall
{
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
    internal int RawXRepeat { get; }

    /// <summary>
    /// Horizontal and vertical repeat factors for the wall texture, affecting its size.
    /// </summary>
    internal int RawYRepeat { get; }

    /// <summary>
    /// Horizontal and vertical panning offsets for the wall texture, used for alignment.
    /// </summary>
    internal int RawXPanning { get; }

    /// <summary>
    /// Vertical panning offset for the wall texture, used for alignment.
    /// </summary>
    internal int RawYPanning { get; }

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

    // Bit 0: Blocking wall
    private bool IsBlocking => (RawCStat & 1) != 0;

    // Bit 1: Bottoms of invisible walls swapped
    private bool IsBottomSwapped => (RawCStat & (1 << 1)) != 0;

    // Bit 2: Align picture on bottom (for doors)
    internal bool IsBottomAligned => (RawCStat & (1 << 2)) != 0;

    // Bit 3: x-flipped
    internal bool IsXFlipped => (RawCStat & (1 << 3)) != 0;

    // Bit 4: Masking wall (masked wall for textures)
    private bool IsMasked => (RawCStat & (1 << 4)) != 0;

    // Bit 5: 1-way wall (wall is only visible from one side)
    private bool IsOneWay => (RawCStat & (1 << 5)) != 0;

    // Bit 6: Blocking wall (hitscan / cliptype 1)
    private bool IsHitscanBlocking => (RawCStat & (1 << 6)) != 0;

    // Bit 7: Translucence (makes the wall semi-transparent)
    private bool IsTranslucent => (RawCStat & (1 << 7)) != 0;

    // Bit 8: y-flipped (flips the texture vertically)
    internal bool IsYFlipped => (RawCStat & (1 << 8)) != 0;

    // Bit 9: Translucence reversing (alternate translucency effect)
    private bool IsTranslucenceReversed => (RawCStat & (1 << 9)) != 0;

    internal bool IsPortal => RawNextSector > -1;

    private Mesh? UpperWallMesh { get; set; }
    private Mesh? LowerWallMesh { get; set; }
    private Mesh? SolidWallMesh { get; set; }

    private Sector? NextSector => RawNextSector > -1 ? _map.Sectors[RawNextSector] : null;
    internal Wall NextWall => RawNextWall > -1 ? _map.Walls[RawNextWall] : null;
    private float WallWidth => Vector2.Distance(PositionStart, PositionEnd);
    internal int Id { get; private set; }
    internal Vector2 PositionStart { get; }
    internal Vector2 PositionEnd => _map.Walls[RawPoint2].PositionStart;
    internal bool IsBottomTextureSwapped => (RawCStat & 2) != 0;
    internal bool IsWallAlignedBottom => (RawCStat & 3) != 0;

    public string DebugInfo =>
        $"Wall={Id} XR={RawXRepeat} YR={RawYRepeat} XP={RawXPanning} YP={RawYPanning} IBS={IsBottomSwapped} IBA={IsBottomAligned} PIC={RawPicnum} OPN={RawOverPicnum} CSTAT={RawCStat}";

    public List<Mesh> Meshes { get; private set; }

    public Tile Tile => _map.GroupFile.Tiles[RawPicnum];
    public Tile OverTile => _map.GroupFile.Tiles[RawOverPicnum];

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
        var wallHeight = top - bottom;

        if (bottom >= top)
            return null;

        //var tile = !IsBottomTextureSwapped ? Tile : _map.GroupFile.Tiles[RawOverPicnum];
        var tile = Tile;

        var uvPositions = Utils.CreateWallUvs(this, tile, wallHeight, MeshType.UpperWall);

        var points = new List<Vector3>
        {
            new(PositionStart.X, bottom, PositionStart.Y),
            new(PositionEnd.X, bottom, PositionEnd.Y),
            new(PositionEnd.X, top, PositionEnd.Y),
            new(PositionStart.X, top, PositionStart.Y)
        };

        var vertices = new List<Vertex>();
        for (var i = 0; i < 4; i++)
            vertices.Add(new Vertex(points[i], uvPositions[i]));

        // Define indices
        var indices = ImmutableList.Create<int>(0, 1, 2, 2, 3, 0);
        // Create the mesh
        var mesh = new Mesh(vertices, indices, tile, _sector, MeshType.UpperWall, this);
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

        var tile = !IsBottomTextureSwapped ? Tile : _map.GroupFile.Tiles[RawOverPicnum];

        var wallHeight = top - bottom;
        var uvPositions = Utils.CreateWallUvs(this, tile, wallHeight, MeshType.LowerWall);

        var points = new List<Vector3>
        {
            new(PositionStart.X, bottom, PositionStart.Y),
            new(PositionEnd.X, bottom, PositionEnd.Y),
            new(
                PositionEnd.X,
                NextSector == null
                    ? top
                    : Utils.GetFloorHeightAt(new Vector2(PositionEnd.X, PositionEnd.Y), NextSector),
                PositionEnd.Y
            ),
            new(
                PositionStart.X,
                NextSector == null
                    ? top
                    : Utils.GetFloorHeightAt(
                        new Vector2(PositionStart.X, PositionStart.Y),
                        NextSector
                    ),
                PositionStart.Y
            )
        };

        var vertices = new List<Vertex>();
        for (var i = 0; i < 4; i++)
            vertices.Add(new Vertex(points[i], uvPositions[i]));

        // Define indices
        var indices = ImmutableList.Create<int>(0, 1, 2, 2, 3, 0);

        // Create the mesh
        var mesh = new Mesh(vertices, indices, tile, _sector, MeshType.LowerWall, this);
        return mesh;
    }

    private Mesh? CreateSolidWallMesh()
    {
        if (IsPortal)
            return null;

        var top = _sector.CeilingYCoordinate;
        var bottom = _sector.FloorYCoordinate;

        //var tile = !IsBottomTextureSwapped ? Tile : _map.GroupFile.Tiles[RawOverPicnum];
        var tile = RawOverPicnum == 0 ? Tile : OverTile;

        var wallHeight = top - bottom;
        var uvPositions = Utils.CreateWallUvs(this, tile, wallHeight, MeshType.SolidWall);

        var points = new List<Vector3>
        {
            new(PositionStart.X, bottom, PositionStart.Y),
            new(PositionEnd.X, bottom, PositionEnd.Y),
            new(PositionEnd.X, top, PositionEnd.Y),
            new(PositionStart.X, top, PositionStart.Y)
        };

        var vertices = new List<Vertex>();
        for (var i = 0; i < 4; i++)
            vertices.Add(new Vertex(points[i], uvPositions[i]));

        // Define indices
        var indices = ImmutableList.Create<int>(0, 1, 2, 2, 3, 0);

        // Create the mesh
        var mesh = new Mesh(vertices, indices, tile, _sector, MeshType.SolidWall, this);
        return mesh;
    }
}
