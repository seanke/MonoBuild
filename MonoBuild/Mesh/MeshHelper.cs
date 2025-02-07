using System;
using System.Linq;
using LibTessDotNet;
using MonoBuild.Map;

namespace MonoBuild.Mesh;

public static class MeshHelper
{
    public static Tess GetTessellatedSectorWallLoop(List<RawWall> sectorWallLoop, int height)
    {
        // Create a list of unique points for the floor.
        var floorPoints = sectorWallLoop
            .Select(w => MapHelper.ConvertDuke3DToMono(new Vector3(w.X, w.Y, height)))
            //.Distinct() // Remove duplicates
            .ToList();

        // Ensure we have a valid polygon (at least 3 distinct points)
        if (floorPoints.Count < 3)
        {
            return null;
        }

        // Close the contour if necessary
        if (floorPoints[0] != floorPoints.Last())
        {
            floorPoints.Add(floorPoints[0]);
        }

        // Strip redundant points (collinear, degenerate, or very close)
        StripLoop(floorPoints);

        // Ensure we still have a valid polygon after cleaning
        if (floorPoints.Count < 3)
        {
            return null;
        }

        // Convert to LibTessDotNet's ContourVertex format
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

        // Create and set up tessellator
        var tess = new Tess();
        tess.AddContour(contour, ContourOrientation.Original);

        // Try a more robust winding rule (NonZero instead of EvenOdd)
        tess.Tessellate(WindingRule.NonZero, ElementType.Polygons, 3);

        return tess;
    }

    /// <summary>
    /// Removes collinear and redundant points from the loop.
    /// Inspired by Build Engine's `StripLoop` function.
    /// </summary>
    private static void StripLoop(List<Vector3> points)
    {
        const float tolerance = 1 / 2560f;

        for (int p = 0; p < points.Count; p++)
        {
            int prev = (p == 0) ? points.Count - 1 : p - 1;
            int next = (p == points.Count - 1) ? 0 : p + 1;

            // If two neighboring points are equal, remove this one
            if (points[next] == points[prev])
            {
                points.RemoveAt(p);
                p = Math.Max(0, p - 1); // Backtrack to recheck
                continue; // Skip to next iteration to avoid out-of-bounds errors
            }

            // Remove collinear points (same X or Y direction)
            bool isCollinear =
                (
                    Math.Abs(points[prev].X - points[p].X) < tolerance
                    && Math.Abs(points[next].X - points[p].X) < tolerance
                    && Math.Sign(points[next].Z - points[p].Z)
                        == Math.Sign(points[prev].Z - points[p].Z)
                )
                || (
                    Math.Abs(points[prev].Z - points[p].Z) < tolerance
                    && Math.Abs(points[next].Z - points[p].Z) < tolerance
                    && Math.Sign(points[next].X - points[p].X)
                        == Math.Sign(points[prev].X - points[p].X)
                )
                || Vector3.Distance(points[prev], points[next]) < tolerance; // Very close points

            if (isCollinear)
            {
                points.RemoveAt(p);
                p = Math.Max(0, p - 1); // Backtrack to recheck
            }
        }
    }
}
