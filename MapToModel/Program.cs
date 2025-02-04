using System.Numerics;
using MapToFbx;
using SharpGLTF.Geometry;
using SharpGLTF.Materials;
using VERTEX = SharpGLTF.Geometry.VertexTypes.VertexPosition;

var buildHeightUnitMeterRatio = 0.01905f;

var material1 = new MaterialBuilder()
    .WithDoubleSide(true)
    .WithMetallicRoughnessShader()
    .WithChannelParam(KnownChannel.BaseColor, KnownProperty.RGBA, new Vector4(1, 0, 0, 1));
var floorMaterial = new MaterialBuilder()
    .WithDoubleSide(true)
    .WithMetallicRoughnessShader()
    .WithChannelParam(KnownChannel.BaseColor, KnownProperty.RGBA, new Vector4(1, 0, 1, 1));
var scene = new SharpGLTF.Scenes.SceneBuilder();

MapState.LoadMapFromFile(new FileInfo("E1L1.MAP"));
var map = MapState.LoadedRawMap;
if (map == null)
    throw new Exception("Failed to load map");

foreach (var sector in map.Sectors)
{
    // Get the walls for this sector.
    // (Assumes MapState.GetSectorWalls(sector) returns the walls in the sector.)
    var walls = MapState.GetSectorWalls(sector).ToArray();
    if (walls.Length < 3)
        continue; // We need at least three points to form a floor polygon

    // Use the sector's FloorZ as the height for the floor plane.
    var floorZ = sector.FloorZ * buildHeightUnitMeterRatio;

    // Create a list of points (as Vector3) for the floor.
    // We use each wall's (X, Y) and the sector's FloorZ.
    var floorPoints = walls.Select(w => new Vector3(w.X, w.Y, floorZ)).ToList();

    // Compute the centroid of the floor polygon (ignoring Z).
    var centroid = new Vector2(floorPoints.Average(p => p.X), floorPoints.Average(p => p.Y));

    // Sort the points by angle around the centroid.
    // This helps ensure the points are in proper winding order.
    floorPoints.Sort(
        (a, b) =>
        {
            var angleA = Math.Atan2(a.Y - centroid.Y, a.X - centroid.X);
            var angleB = Math.Atan2(b.Y - centroid.Y, b.X - centroid.X);
            return angleA.CompareTo(angleB);
        }
    );

    // Create a new mesh builder for the floor plane.
    var floorMesh = new MeshBuilder<VERTEX>();
    var primitive = floorMesh.UsePrimitive(floorMaterial);

    // Triangulate the polygon using a triangle fan.
    // (This works well for convex polygons.)
    for (int i = 1; i < floorPoints.Count - 1; i++)
    {
        primitive.AddTriangle(
            new VERTEX(floorPoints[0]),
            new VERTEX(floorPoints[i]),
            new VERTEX(floorPoints[i + 1])
        );
    }

    // Add the floor mesh to the scene.
    // In this example we do not apply any additional transformation.
    scene.AddRigidMesh(floorMesh, Matrix4x4.Identity);
}
var model = scene.ToGltf2();
model.SaveGLTF("mesh.gltf");
