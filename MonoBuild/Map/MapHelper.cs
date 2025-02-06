namespace MonoBuild.Map;

public static class MapHelper
{
    public const float BuildHeightUnitMeterRatio = -0.001905f; // Scale factor from Duke3D to MonoGame
    public const float BuildWidthUnitMeterRatio = 0.01905f; // Scale factor from Duke3D to MonoGame

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
}
