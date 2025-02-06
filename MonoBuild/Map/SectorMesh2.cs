using System;
using System.Collections.Generic;
using System.Linq;
using LibTessDotNet;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoBuild.Map
{
    public class SectorMesh2 : IDisposable
    {
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;
        private BasicEffect _effect;
        private GraphicsDevice graphicsDevice;

        public SectorMesh2(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
        }

        // Tessellate the floor polygon (all vertices on sector.FloorZ).
        private Tess GetTessellatedSector(RawSector sector)
        {
            var walls = State.GetSectorWalls(sector).ToArray();

            // Create a list of points (as Vector3) for the floor.
            var floorPoints = walls
                .Select(w => MapHelper.ConvertDuke3DToMono(new Vector3(w.X, w.Y, sector.FloorZ)))
                .ToList();

            // Use LibTessDotNet to triangulate the polygon.
            var tess = new Tess();

            // Convert your floorPoints to LibTessDotNet's ContourVertex format.
            var contour = floorPoints
                .Select(p => new ContourVertex
                {
                    Position = new Vec3
                    {
                        X = p.X,
                        Y = p.Y,
                        Z = p.Z
                    }
                })
                .ToArray();

            // Add the contour (polygon). Specify the original orientation.
            tess.AddContour(contour, ContourOrientation.Original);

            // Tessellate using an appropriate winding rule and specify that we want triangles.
            tess.Tessellate(WindingRule.EvenOdd, ElementType.Polygons, 3);

            return tess;
        }

        public void LoadContent(RawSector sector)
        {
            var walls = State.GetSectorWalls(sector).ToArray();
            if (walls.Length < 3)
                return; // At least three points are needed.

            // Dispose old buffers (if any)
            _vertexBuffer?.Dispose();
            _indexBuffer?.Dispose();

            // Tessellate the floor polygon.
            var tessFloor = GetTessellatedSector(sector);

            // We'll accumulate vertices and indices from three parts:
            // (1) Floor, (2) Ceiling, and (3) Walls.
            var vertices = new List<VertexPositionColor>();

            // --- 1. Floor ---
            var floorStartIndex = vertices.Count;
            vertices.AddRange(
                tessFloor.Vertices.Select(v => new VertexPositionColor(
                    new Vector3(v.Position.X, v.Position.Y, v.Position.Z),
                    Color.Lerp(Color.Red, Color.Blue, v.Position.Y)
                ))
            );
            var indices = tessFloor
                .Elements.Select(index => (short)(floorStartIndex + index))
                .ToList();

            // --- 2. Ceiling ---
            var ceilingStartIndex = vertices.Count;
            vertices.AddRange(
                tessFloor.Vertices.Select(v => new VertexPositionColor(
                    new Vector3(
                        v.Position.X,
                        sector.CeilingZ * MapHelper.BuildHeightUnitMeterRatio,
                        v.Position.Z
                    ),
                    Color.Lerp(Color.DarkGreen, Color.DarkMagenta, v.Position.Y)
                ))
            );
            // Reverse the winding order for the ceiling
            for (var i = 0; i < tessFloor.Elements.Length; i += 3)
            {
                indices.Add((short)(ceilingStartIndex + tessFloor.Elements[i]));
                indices.Add((short)(ceilingStartIndex + tessFloor.Elements[i + 2]));
                indices.Add((short)(ceilingStartIndex + tessFloor.Elements[i + 1]));
            }

            // --- 3. Walls ---
            for (int i = 0; i < walls.Length; i++)
            {
                int next = (i + 1) % walls.Length;
                Vector3 floorA = MapHelper.ConvertDuke3DToMono(
                    new Vector3(walls[i].X, walls[i].Y, sector.FloorZ)
                );
                Vector3 floorB = MapHelper.ConvertDuke3DToMono(
                    new Vector3(walls[next].X, walls[next].Y, sector.FloorZ)
                );
                Vector3 ceilA = MapHelper.ConvertDuke3DToMono(
                    new Vector3(walls[i].X, walls[i].Y, sector.CeilingZ)
                );
                Vector3 ceilB = MapHelper.ConvertDuke3DToMono(
                    new Vector3(walls[next].X, walls[next].Y, sector.CeilingZ)
                );

                int idxFloorA = vertices.Count;
                vertices.Add(
                    new VertexPositionColor(floorA, Color.Lerp(Color.Red, Color.Blue, floorA.Y))
                );
                int idxFloorB = vertices.Count;
                vertices.Add(
                    new VertexPositionColor(floorB, Color.Lerp(Color.Red, Color.Blue, floorB.Y))
                );
                int idxCeilA = vertices.Count;
                vertices.Add(
                    new VertexPositionColor(ceilA, Color.Lerp(Color.Red, Color.Blue, ceilA.Y))
                );
                int idxCeilB = vertices.Count;
                vertices.Add(
                    new VertexPositionColor(ceilB, Color.Lerp(Color.Red, Color.Blue, ceilB.Y))
                );

                // Triangle 1: floorA, floorB, ceilA.
                indices.Add((short)idxFloorA);
                indices.Add((short)idxFloorB);
                indices.Add((short)idxCeilA);

                // Triangle 2: floorB, ceilB, ceilA.
                indices.Add((short)idxFloorB);
                indices.Add((short)idxCeilB);
                indices.Add((short)idxCeilA);
            }

            // Create the vertex buffer.
            _vertexBuffer = new VertexBuffer(
                graphicsDevice,
                typeof(VertexPositionColor),
                vertices.Count,
                BufferUsage.WriteOnly
            );
            _vertexBuffer.SetData(vertices.ToArray());

            // Create the index buffer.
            _indexBuffer = new IndexBuffer(
                graphicsDevice,
                IndexElementSize.SixteenBits,
                indices.Count,
                BufferUsage.WriteOnly
            );
            _indexBuffer.SetData(indices.ToArray());

            // Set up a basic effect for rendering.
            _effect = new BasicEffect(graphicsDevice)
            {
                VertexColorEnabled = true,
                World = Matrix.Identity,
                View = Matrix.CreateLookAt(new Vector3(0, 0, 5), Vector3.Zero, Vector3.Up),
                Projection = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.PiOver4,
                    graphicsDevice.Viewport.AspectRatio,
                    0.1f,
                    100f
                )
            };
        }

        public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
        {
            if (_vertexBuffer == null || _indexBuffer == null)
                return;

            _effect.View = viewMatrix;
            _effect.Projection = projectionMatrix;

            graphicsDevice.SetVertexBuffer(_vertexBuffer);
            graphicsDevice.Indices = _indexBuffer;

            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    0,
                    0,
                    _indexBuffer.IndexCount / 3
                );
            }
        }

        public void Dispose()
        {
            _vertexBuffer?.Dispose();
            _indexBuffer?.Dispose();
            _effect?.Dispose();
        }
    }
}
