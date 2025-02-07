using System;
using System.Linq;
using MonoBuild.Map;
using MonoBuild.Mesh;

namespace MonoBuild.Render;

public class SectorRenderer(GraphicsDevice graphicsDevice) : IDisposable
{
    private WallsRenderer _wallsRenderer;
    private FloorMesh _floorMesh;
    private CeilingMesh _ceilingMesh;

    public void LoadContent(RawSector sector)
    {
        var walls = MapHelper.GetSectorWalls(sector).ToArray();
        if (walls.Length < 3)
        {
            Console.WriteLine($"Sector {sector.Id} has less than 3 walls.");
            return; // We need at least three points to form a floor polygon
        }

        _wallsRenderer = new WallsRenderer(graphicsDevice, sector);
        _wallsRenderer.LoadContent();

        _floorMesh = new FloorMesh(graphicsDevice, sector);
        _floorMesh.LoadContent();

        _ceilingMesh = new CeilingMesh(graphicsDevice, sector);
        _ceilingMesh.LoadContent();
    }

    public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
    {
        _wallsRenderer.Draw(viewMatrix, projectionMatrix);
        _floorMesh.Draw(viewMatrix, projectionMatrix);
        _ceilingMesh.Draw(viewMatrix, projectionMatrix);
    }

    public void Dispose()
    {
        _wallsRenderer.Dispose();
        _floorMesh.Dispose();
        _ceilingMesh.Dispose();
    }
}
