using Engine.Group;
using Engine.Map;

namespace Tests;

public class SectorMeshTests
{
    [Fact]
    public void SomeWallMeshesExist()
    {
        var group = new GroupFile(new FileInfo("DUKE3D.GRP"));
        var map = new MapFile(new FileInfo("E1L1.MAP"), group);

        var wallMeshes = map.Meshes.Where(mesh =>
            mesh.Type == MeshType.UpperWall
            || mesh.Type == MeshType.LowerWall
            || mesh.Type == MeshType.SolidWall
        );

        Assert.NotEmpty(wallMeshes);
    }

    [Fact]
    public void RedSectorWithInnerRedSector_CutsAWholeInTheSector()
    {
        var group = new GroupFile(new FileInfo("DUKE3D.GRP"));
        var map = new MapFile(new FileInfo("E1L1.MAP"), group);

        var sector179Meshes = map
            .Meshes.Where(mesh => mesh is { SectorId: 179, Type: MeshType.Floor })
            .ToList();

        // There should be only one mesh for sector 179
        Assert.Single(sector179Meshes);

        // The mesh should have a hole in the middle
        var mesh = sector179Meshes.Single();
        Assert.True(mesh.Vertices.Count == 8);
    }
}
