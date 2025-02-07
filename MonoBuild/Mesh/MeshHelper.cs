using System.Linq;
using LibTessDotNet;
using MonoBuild.Map;

namespace MonoBuild.Mesh;

public static class MeshHelper
{
    public static Tess GetTessellatedSector(RawSector sector, int height)
    {
        var walls = MapHelper.GetSectorWalls(sector).ToArray();

        // Create a list of points (as Vector3) for the floor.
        var floorPoints = walls
            .Select(w => MapHelper.ConvertDuke3DToMono(new Vector3(w.X, w.Y, height)))
            .ToList();

        // Use LibTessDotNet to triangulate the polygon.
        var tess = new Tess();

        // Convert your floorPoints to LibTessDotNet's ContourVertex format.
        var contour = floorPoints
            .Select(p => new ContourVertex
            {
                Position = new Vec3
                {
                    X = p.X,
                    Y = p.Y,
                    Z = p.Z
                }
            })
            .ToArray();

        // Add the contour (polygon). Specify the original orientation.
        tess.AddContour(contour, ContourOrientation.Original);

        // Tessellate using an appropriate winding rule and specify that we want triangles.
        tess.Tessellate(WindingRule.EvenOdd, ElementType.Polygons, 3);

        return tess;
    }
}
