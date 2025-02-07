using System;

namespace MonoBuild.Render;

public class MapRenderer(GraphicsDevice graphicsDevice)
{
    private List<SectorRenderer> _sectorMeshes;

    public void LoadContent()
    {
        _sectorMeshes = [];

        if (!State.IsMapLoaded)
            throw new InvalidOperationException("No map is loaded.");

        Console.WriteLine($"Sector count: {State.LoadedRawMap!.Sectors.Count}");

        foreach (var sector in State.LoadedRawMap!.Sectors)
        {
            var sectorMesh = new SectorRenderer(graphicsDevice);
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
