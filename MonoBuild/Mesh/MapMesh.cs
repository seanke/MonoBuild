using System;
using System.Linq;
using Engine.Map;
using MonoBuild.Loaders;

namespace MonoBuild.ProofOfConcepts;

public class MapMesh(GraphicsDevice graphicsDevice) : IDisposable
{
    private List<Mesh> _meshes = new();

    public void LoadContent(MapFile mapFile)
    {
        _meshes = mapFile
            .Meshes.Select(mesh =>
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
                    BufferUsage.None
                );
                vertexBuffer.SetData(vertices);

                var indexBuffer = new IndexBuffer(
                    graphicsDevice,
                    typeof(short),
                    mesh.Indices.Count,
                    BufferUsage.None
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

                return new Mesh
                {
                    VertexBuffer = vertexBuffer,
                    IndexBuffer = indexBuffer,
                    Effect = effect,
                    Texture = texture,
                    Sector = mesh.Sector,
                    Wall = mesh.Wall,
                    Type = mesh.Type
                };
            })
            .ToList();

        GlobalState.Meshes = _meshes;
    }

    public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
    {
        graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

        foreach (var mesh in _meshes)
        {
            if (mesh.VertexBuffer == null || mesh.IndexBuffer == null || mesh.Effect == null)
                continue;

            mesh.Effect.View = viewMatrix; // Update the View matrix from the camera
            mesh.Effect.Projection = projectionMatrix; // Update the Projection matrix
            mesh.Effect.Texture = mesh.Texture; // Ensure the texture is set

            mesh.Effect.DiffuseColor =
                GlobalState.HighlightedMesh == mesh
                    ? Color.Red.ToVector3()
                    : Color.White.ToVector3();

            graphicsDevice.SetVertexBuffer(mesh.VertexBuffer);
            graphicsDevice.Indices = mesh.IndexBuffer;

            foreach (var pass in mesh.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    0,
                    0,
                    mesh.IndexBuffer.IndexCount / 3
                );
            }
        }
    }

    public void Dispose()
    {
        foreach (var mesh in _meshes)
            mesh.Dispose();

        _meshes.Clear();
    }
}
