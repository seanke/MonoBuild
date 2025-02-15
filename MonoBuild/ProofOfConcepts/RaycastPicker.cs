using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoBuild.ProofOfConcepts;

public class RaycastPicker(GraphicsDevice graphicsDevice)
{
    /// <summary>
    /// Generates a ray from the center of the screen into the 3D world.
    /// </summary>
    private List<Ray> GetSampledRays(Matrix viewMatrix, Matrix projectionMatrix)
    {
        var viewport = graphicsDevice.Viewport;
        var screenCenter = new Vector2(viewport.Width / 2, viewport.Height / 2);

        // Small offsets to create multiple rays
        Vector2[] offsets =
        {
            new Vector2(0, 0), // Center
            new Vector2(2, 0), // Slightly right
            new Vector2(-2, 0), // Slightly left
            new Vector2(0, 2), // Slightly up
            new Vector2(0, -2) // Slightly down
        };

        List<Ray> rays = new List<Ray>();

        foreach (var offset in offsets)
        {
            Vector2 samplePoint = screenCenter + offset;

            var nearPoint = viewport.Unproject(
                new Vector3(samplePoint, 0),
                projectionMatrix,
                viewMatrix,
                Matrix.Identity
            );
            var farPoint = viewport.Unproject(
                new Vector3(samplePoint, 1),
                projectionMatrix,
                viewMatrix,
                Matrix.Identity
            );
            Vector3 direction = Vector3.Normalize(farPoint - nearPoint);

            rays.Add(new Ray(nearPoint, direction));
        }

        return rays;
    }

    /// <summary>
    /// Checks if a ray intersects a triangle and returns the distance to the hit.
    /// </summary>
    private bool RayIntersectsTriangle(
        Ray ray,
        Vector3 v0,
        Vector3 v1,
        Vector3 v2,
        out float distance
    )
    {
        const float epsilon = 1e-6f;
        distance = 0;

        var edge1 = v1 - v0;
        var edge2 = v2 - v0;
        var h = Vector3.Cross(ray.Direction, edge2);
        var a = Vector3.Dot(edge1, h);

        if (Math.Abs(a) < epsilon)
            return false; // Ray is parallel to the triangle

        var f = 1.0f / a;
        var s = ray.Position - v0;
        var u = f * Vector3.Dot(s, h);

        if (u < 0.0f || u > 1.0f)
            return false;

        var q = Vector3.Cross(s, edge1);
        var v = f * Vector3.Dot(ray.Direction, q);

        if (v < 0.0f || u + v > 1.0f)
            return false;

        distance = f * Vector3.Dot(edge2, q);
        return distance > epsilon;
    }

    /// <summary>
    /// Finds the nearest mesh that the ray intersects.
    /// </summary>
    private Mesh? FindNearestMesh(Ray ray)
    {
        float? closestDistance = null;
        Mesh? closestMesh = null;

        foreach (var mesh in GlobalState.Meshes)
        {
            var vertexBuffer = mesh.VertexBuffer;
            var indexBuffer = mesh.IndexBuffer;

            // Retrieve vertex and index data
            var vertices = new VertexPositionTexture[vertexBuffer.VertexCount];
            vertexBuffer.GetData(vertices);

            var indices = new short[indexBuffer.IndexCount];
            indexBuffer.GetData(indices);

            for (var i = 0; i < indices.Length; i += 3)
            {
                var v0 = vertices[indices[i]].Position;
                var v1 = vertices[indices[i + 1]].Position;
                var v2 = vertices[indices[i + 2]].Position;

                if (RayIntersectsTriangle(ray, v0, v1, v2, out var distance))
                {
                    if (closestDistance == null || distance < closestDistance.Value)
                    {
                        closestDistance = distance;
                        closestMesh = mesh;
                    }
                }
            }
        }

        return closestMesh;
    }

    /// <summary>
    /// Updates the raycaster to find and highlight the nearest mesh.
    /// </summary>
    public void Update(Matrix viewMatrix, Matrix projectionMatrix)
    {
        var ray = GetSampledRays(viewMatrix, projectionMatrix);

        foreach (var r in ray)
        {
            var m = FindNearestMesh(r);
            if (m != null)
            {
                GlobalState.HighlightedMesh = m;
                break;
            }
        }
    }
}
