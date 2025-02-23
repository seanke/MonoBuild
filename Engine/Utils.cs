using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using Engine.Art;
using Engine.Map;

namespace Engine;

public static class Utils
{
    public static float Test { get; set; } = 0f;

    internal static Vector2[] CreateWallUvs(
        Wall wall,
        Tile tile,
        float wallHeight,
        MeshType meshType
    )
    {
        if (
            meshType != MeshType.LowerWall
            && meshType != MeshType.UpperWall
            && meshType != MeshType.SolidWall
        )
            throw new ArgumentException("Invalid mesh type for wall UVs");

        var xPanning = wall.RawXPanning;
        var yPanning = wall.RawYPanning;
        var xRepeat = wall.RawXRepeat;
        var yRepeat = wall.RawYRepeat;

        if (wall.IsPortal && wall.IsBottomTextureSwapped)
        {
            tile = wall.NextWall.Tile;
            xPanning = wall.NextWall.RawXPanning;
            yPanning = wall.NextWall.RawYPanning;
            xRepeat = wall.NextWall.RawXRepeat;
            yRepeat = wall.NextWall.RawYRepeat;
        }

        var xPan = xPanning / 128f;
        var yPan = yPanning / 256f;

        if (
            (meshType == MeshType.UpperWall && wall.IsBottomAligned)
            || (!wall.IsBottomAligned && meshType == MeshType.SolidWall)
        )
        {
            // This value will be something like 80.
            var extraTileLeftOver = wallHeight % tile.Height;

            // This value will be something like 0.5.
            var yShiftToAlignWithRoof = extraTileLeftOver / tile.Height;

            yPan += yShiftToAlignWithRoof;
        }
        else if (meshType == MeshType.UpperWall && !wall.IsBottomAligned)
        {
            yPan = 0;
        }

        //yPan = Test;

        //if (wall.IsBottomAligned)
        //    yPan = 0f;

        var xScale = xRepeat * 8f / tile.Width + xPan;
        var yScale = wallHeight / ((float)tile.Height) * (yRepeat / 8f) * -1 + yPan;

        if (wall.IsXFlipped)
            xScale *= -1f;

        if (wall.IsYFlipped)
            yScale *= -1f;

        var uvBottomLeft = new Vector2(xPan, yPan);
        var uvBottomRight = new Vector2(xScale, yPan);
        var uvTopRight = new Vector2(xScale, yScale);
        var uvTopLeft = new Vector2(xPan, yScale);

        // The upper mesh needs to align the each of the textures to the top of the wall
        if (meshType == MeshType.UpperWall)
        {
            //return new[] { uvTopLeft, uvTopRight, uvBottomRight, uvBottomLeft };
        }

        return new[] { uvBottomLeft, uvBottomRight, uvTopRight, uvTopLeft };
    }

    internal static float GetFloorHeightAt(Vector2 point, Sector sector)
    {
        // If the floor isn’t sloped, return the constant height.
        if (!sector.IsFloorSloped)
            return sector.FloorYCoordinate;

        if (sector.Walls == null || sector.Walls.Count == 0)
            return sector.FloorYCoordinate;

        // Use the first wall’s start point as the reference.
        var wallToSlopeFrom = sector.Walls[0];

        // Determine the slope direction from the first two walls.
        // I am just grabbing the second wall here, assuming there's always at least two walls.
        var slopeDirection = Vector2.Normalize(
            wallToSlopeFrom.PositionEnd - wallToSlopeFrom.PositionStart
        );

        // Rotate the slope direction by 90° to get the perpendicular direction.
        slopeDirection = new Vector2(slopeDirection.Y, -slopeDirection.X);

        // The heinum represents the slope “factor.”
        // A value of 4096 corresponds roughly to a 45° slope.
        var slopeFactor = sector.RawFloorHeinum / Constants.BuildSlopeAngleDivider;

        // Compute the offset
        var offsetWorld =
            (
                (point.X - wallToSlopeFrom.PositionStart.X) * slopeDirection.X
                + (point.Y - wallToSlopeFrom.PositionStart.Y) * slopeDirection.Y
            ) * slopeFactor;

        return sector.FloorYCoordinate + offsetWorld;
    }
}
