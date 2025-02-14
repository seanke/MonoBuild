using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using Engine.Art;
using Engine.Map;

namespace Engine;

public static class Utils
{
    public static float Test { get; set; } = 128f;

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
        var yPan = yPanning / Test;

        //if (wall.IsBottomAligned)
        //    yPan = 0f;

        var xScale = xRepeat * 8f / tile.Width + xPan;
        var yScale = wallHeight / tile.Height * (yRepeat / 8f) * -1 + yPan;

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
}
