using System;
using System.IO;
using MonoBuild.Art;
using MonoBuild.Group;
using MonoBuild.Map;
using MonoBuild.Palette;

namespace MonoBuild;

public static class State
{
    public static bool IsMapLoaded { get; private set; }
    public static RawMapFile? LoadedRawMap { get; private set; }

    public static bool IsGroupLoaded { get; private set; }
    public static RawGroupFile? LoadedRawGroup { get; private set; }

    public static RawPaletteFile LoadedPaletteFile { get; private set; }
    public static GroupArt LoadedGroupArt { get; private set; } = new();

    public static bool LoadGroupFromFile(FileInfo filePath)
    {
        try
        {
            LoadedRawGroup = RawGroupFile.LoadFromStream(
                filePath.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None)
            );

            if (LoadedRawGroup == null)
                throw new Exception("Failed to load group from file.");

            LoadedGroupArt = GroupArt.Load(LoadedRawGroup);

            LoadedPaletteFile = RawPaletteFile.Load(LoadedRawGroup);

            IsGroupLoaded = true;
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public static bool LoadMapFromBytes(byte[] mapData)
    {
        UnloadMap();
        try
        {
            LoadedRawMap = RawMapFile.LoadFromBytes(mapData);

            if (LoadedRawMap == null)
                throw new Exception("Failed to load map from bytes.");

            IsMapLoaded = true;
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public static bool LoadMapFromFile(FileInfo filePath)
    {
        UnloadMap();
        try
        {
            LoadedRawMap = RawMapFile.LoadFromStream(
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

    public static void UnloadMap()
    {
        IsMapLoaded = false;
        LoadedRawMap = null;
    }

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

            var wall = LoadedRawMap.Walls[currentWallIndex];
            yield return wall;

            currentWallIndex = wall.Point2;
        }
    }
}
