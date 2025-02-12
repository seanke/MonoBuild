using System.Numerics;

namespace Engine;

internal static class Utilities
{
    internal const float BuildHeightUnitMeterRatio = -1 / 256f; // Scale factor from Duke3D to MonoGame
    private const float BuildWidthUnitMeterRatio = 1 / 16f; // Scale factor from Duke3D to MonoGame

    /// <summary>
    /// Converts a position from Build engine coordinate system to a right-handed coordinate system.
    ///     X-axis increases to the right.
    ///     Y-axis increases upward.
    ///     Z-axis increases forward (out of the screen, towards the viewer).
    /// </summary>
    /// <param name="position">The original Duke3D position.</param>
    /// <returns>A Vector3 with the corrected coordinate system.</returns>
    internal static Vector3 ConvertBuildToRightHandedCoordinates(Vector3 position)
    {
        return new Vector3(
            position.X * BuildWidthUnitMeterRatio, // X stays the same (scaling applied)
            position.Z * BuildHeightUnitMeterRatio, // Invert Y
            position.Y * BuildWidthUnitMeterRatio // Invert and scale Z
        );
    }
}
