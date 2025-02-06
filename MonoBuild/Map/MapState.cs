using System;
using System.IO;
using System.Linq;

namespace MonoBuild.Map;

public static class MapState
{
    public static bool IsMapLoaded { get; private set; }
    public static RawMap? LoadedRawMap { get; private set; }

    public static bool LoadMapFromFile(FileInfo filePath)
    {
        Unload();
        try
        {
            LoadedRawMap = RawMap.LoadFromStream(
                filePath.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None)
            );

            if (LoadedRawMap == null)
                throw new Exception("Failed to load map from file.");

            IsMapLoaded = true;
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public static void Unload()
    {
        IsMapLoaded = false;
        LoadedRawMap = null;
    }

    /*public static IEnumerable<RawWall> GetSectorWalls(RawSector sector)
    {
        if (!IsMapLoaded)
            throw new InvalidOperationException("No map is loaded.");

        return LoadedRawMap!.Walls.Skip(sector.WallPtr).Take(sector.WallNum);
    }*/

    public static IEnumerable<RawWall> GetSectorWalls(RawSector sector)
    {
        if (!IsMapLoaded)
            throw new InvalidOperationException("No map is loaded.");

        if (
            LoadedRawMap?.Walls == null
            || sector.WallPtr < 0
            || sector.WallPtr >= LoadedRawMap.Walls.Count
        )
            yield break;

        HashSet<int> visitedWalls = new();
        int currentWallIndex = sector.WallPtr;

        while (currentWallIndex >= 0 && currentWallIndex < LoadedRawMap.Walls.Count)
        {
            if (!visitedWalls.Add(currentWallIndex))
                yield break; // Prevent infinite loops in case of incorrect data

            RawWall wall = LoadedRawMap.Walls[currentWallIndex];
            yield return wall;

            currentWallIndex = wall.Point2;
        }
    }
}
