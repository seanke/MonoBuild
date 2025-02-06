using System;

namespace MonoBuild.Map;

public class MapRenderer(GraphicsDevice graphicsDevice)
{
    private List<SectorMesh> _sectorMeshes;

    public void LoadContent()
    {
        _sectorMeshes = [];

        if (!MapState.IsMapLoaded)
            throw new InvalidOperationException("No map is loaded.");

        Console.WriteLine($"Sector count: {MapState.LoadedRawMap!.Sectors.Count}");

        foreach (var sector in MapState.LoadedRawMap!.Sectors)
        {
            var sectorMesh = new SectorMesh(graphicsDevice);
            sectorMesh.LoadContent(sector);
            _sectorMeshes.Add(sectorMesh);
        }
    }

    public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
    {
        foreach (var sectorMesh in _sectorMeshes)
        {
            sectorMesh.Draw(viewMatrix, projectionMatrix);
        }
    }
}
