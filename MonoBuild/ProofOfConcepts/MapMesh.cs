using System;
using System.Linq;
using Engine.Map;
using MonoBuild.Loaders;

namespace MonoBuild.ProofOfConcepts;

public class MapMesh(GraphicsDevice graphicsDevice) : IDisposable
{
    List<Tuple<VertexBuffer, IndexBuffer, BasicEffect, Texture2D>> _meshes = new();

    public void LoadContent(MapFile mapFile)
    {
        _meshes = mapFile
            .Meshes.Where(mesh => mesh.Type == MeshType.LowerWall || mesh.Type == MeshType.Floor)
            .Select(mesh =>
            {
                var vertices = mesh
                    .Vertices.Select(vertex => new VertexPositionTexture(
                        vertex.Position,
                        vertex.TextureCoordinate
                    ))
                    .ToArray();

                var indices = mesh.Indices.Select(i => (short)i).ToArray();

                var vertexBuffer = new VertexBuffer(
                    graphicsDevice,
                    typeof(VertexPositionTexture),
                    mesh.Vertices.Count,
                    BufferUsage.WriteOnly
                );
                vertexBuffer.SetData(vertices);

                var indexBuffer = new IndexBuffer(
                    graphicsDevice,
                    typeof(short),
                    mesh.Indices.Count,
                    BufferUsage.WriteOnly
                );
                indexBuffer.SetData(indices);

                var texture = TextureLoader.LoadTextureFromTile(graphicsDevice, mesh.Texture);

                var effect = new BasicEffect(graphicsDevice)
                {
                    Texture = texture,
                    TextureEnabled = true,
                    VertexColorEnabled = false,
                    World = Matrix.Identity,
                    View = Matrix.CreateLookAt(new Vector3(0, 0, 5), Vector3.Zero, Vector3.Up),
                    Projection = Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.PiOver4,
                        graphicsDevice.Viewport.AspectRatio,
                        0.1f,
                        100000f
                    )
                };

                return Tuple.Create(vertexBuffer, indexBuffer, effect, texture);
            })
            .ToList();
    }

    public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
    {
        graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

        foreach (var mesh in _meshes)
        {
            // Draw mesh using view and projection matrices
            var vertexBuffer = mesh.Item1;
            var indexBuffer = mesh.Item2;
            var effect = mesh.Item3;
            var texture = mesh.Item4;

            if (vertexBuffer == null || indexBuffer == null || effect == null)
                continue;

            effect.View = viewMatrix; // Update the View matrix from the camera
            effect.Projection = projectionMatrix; // Update the Projection matrix
            effect.Texture = texture; // Ensure the texture is set

            graphicsDevice.SetVertexBuffer(vertexBuffer);
            graphicsDevice.Indices = indexBuffer;

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    0,
                    0,
                    indexBuffer.IndexCount / 3
                );
            }
        }
    }

    public void Dispose()
    {
        foreach (var mesh in _meshes)
        {
            mesh.Item1?.Dispose();
            mesh.Item2?.Dispose();
            mesh.Item3?.Dispose();
            mesh.Item4?.Dispose();
        }

        _meshes.Clear();
    }
}
