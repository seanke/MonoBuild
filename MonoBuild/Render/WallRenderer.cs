using System;
using MonoBuild.Map;
using MonoBuild.Mesh;

namespace MonoBuild.Render;

public class WallRenderer(
    GraphicsDevice graphicsDevice,
    RawSector sector,
    RawWall wall,
    RawWall point2Wall
) : IDisposable
{
    private WallSolidMesh _wallSolidMesh;
    private WallTopMesh _wallTopMesh;
    private WallBottomMesh _wallBottomMesh;

    public void LoadContent()
    {
        _wallSolidMesh = new WallSolidMesh(graphicsDevice, sector, wall, point2Wall);
        _wallSolidMesh.LoadContent();

        _wallTopMesh = new WallTopMesh(graphicsDevice, sector, wall, point2Wall);
        _wallTopMesh.LoadContent();

        _wallBottomMesh = new WallBottomMesh(graphicsDevice, sector, wall, point2Wall);
        _wallBottomMesh.LoadContent();
    }

    public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
    {
        _wallSolidMesh.Draw(viewMatrix, projectionMatrix);
        _wallTopMesh.Draw(viewMatrix, projectionMatrix);
        _wallBottomMesh.Draw(viewMatrix, projectionMatrix);
    }

    public void Dispose()
    {
        _wallSolidMesh.Dispose();
        _wallTopMesh.Dispose();
        _wallBottomMesh.Dispose();
    }
}
