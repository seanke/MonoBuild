using MapToFbx.Map;

namespace MapToFbx;

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

    public static IEnumerable<RawWall> GetSectorWalls(RawSector sector)
    {
        if (!IsMapLoaded)
            throw new InvalidOperationException("No map is loaded.");

        return LoadedRawMap!.Walls.Skip(sector.WallPtr).Take(sector.WallNum);
    }
}
