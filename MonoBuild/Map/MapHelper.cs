using System;

namespace MonoBuild.Map;

public static class MapHelper
{
    public const float BuildHeightUnitMeterRatio = -1 / 256f; // Scale factor from Duke3D to MonoGame
    public const float BuildWidthUnitMeterRatio = 1 / 16f; // Scale factor from Duke3D to MonoGame

    /// <summary>
    /// Converts a position from Duke3D (Build Engine) coordinate system to MonoGame's coordinate system.
    /// </summary>
    /// <param name="position">The original Duke3D position.</param>
    /// <returns>A Vector3 with the corrected coordinate system.</returns>
    public static Vector3 ConvertDuke3DToMono(Vector3 position)
    {
        return new Vector3(
            position.X * BuildWidthUnitMeterRatio, // X stays the same (scaling applied)
            position.Z * BuildHeightUnitMeterRatio, // Invert Y
            position.Y * BuildWidthUnitMeterRatio // Invert and scale Z
        );
    }

    public static List<RawWall> GetSectorWalls(RawSector sector)
    {
        if (sector.Id == 285)
        {
            System.Console.WriteLine();
        }

        if (!State.IsMapLoaded)
            throw new InvalidOperationException("No map is loaded.");

        var result = new List<RawWall>();

        if (
            State.LoadedRawMap?.Walls == null
            || sector.WallPtr < 0
            || sector.WallPtr >= State.LoadedRawMap.Walls.Count
        )
            return result;

        int currentWallIndex = sector.WallPtr;

        for (var i = 0; i < sector.WallNum; i++)
        {
            var wall = State.LoadedRawMap.Walls[currentWallIndex];
            result.Add(wall);

            currentWallIndex = wall.Point2;
        }

        return result;
    }
}
