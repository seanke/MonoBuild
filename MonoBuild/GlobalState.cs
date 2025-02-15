namespace MonoBuild;

public static class GlobalState
{
    public static Mesh? HighlightedMesh { get; set; }
    public static List<Mesh> Meshes { get; set; } = new();
}
