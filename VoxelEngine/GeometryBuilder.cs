using System;
using System.Collections.Generic;
using System.Text;
using Urho3DNet;

namespace VoxelEngine
{
    public struct SimpleVertex
    {
        public Vector3 _position;
        public Vector3 _normal; 
        public Color _color;

        public SimpleVertex(Vector3 position, Vector3 normal, Color color) : this()
        {
            _position = position;
            _normal = normal;
            _color = color;
        }
    }

    public class GeometryBuilder
    {
        CustomGeometry geometry;

        public GeometryBuilder(CustomGeometry geometry)
        {
            this.geometry = geometry;
        }

        public void Append(SimpleVertex[] vertices, int[] indices)
        {
            if (indices.Length % 3 != 0)
            {
                throw new Exception("wrong length indices");
            }

            for (int triIndex = 0; triIndex < indices.Length / 3; triIndex++)
            {
                SimpleVertex v0 = vertices[indices[triIndex * 3 + 0]];
                SimpleVertex v1 = vertices[indices[triIndex * 3 + 1]];
                SimpleVertex v2 = vertices[indices[triIndex * 3 + 2]];
                bool hasTransparency = v0._color.A < 1.0f || v1._color.A < 1.0f || v2._color.A < 1.0f;

                // Добавляем вершины с нормалями
                geometry.DefineVertex(v0._position);
                geometry.DefineNormal(v0._normal); 
                geometry.DefineColor(v0._color);

                geometry.DefineVertex(v1._position);
                geometry.DefineNormal(v1._normal); 
                geometry.DefineColor(v1._color);

                geometry.DefineVertex(v2._position);
                geometry.DefineNormal(v2._normal); 
                geometry.DefineColor(v2._color);
            }
        }


        public void BuildSolidTriangle(SimpleVertex[] frame, bool bCW = true)
        {

            //Vertex order:
            // 3 2
            // 1 0
            if (bCW)
            {
                int[] indices = { 0, 1, 2 };
                Append(frame, indices);
            }
            else
            {
                int[] indices = { 2, 1, 0 };
                Append(frame, indices);
            }
        }

        public void BuildSolidQuad(SimpleVertex[] frame, bool bCW = true)
        {
            //Vertex order:
            // 3 2
            // 1 0
            if (bCW)
            {
                int[] indices = { 0, 1, 2, 2, 3, 0 };
                Append(frame, indices);
            }
            else
            {
                int[] indices = { 2, 1, 0, 0, 3, 2 };
                Append(frame, indices);
            }
        }
    }
}
