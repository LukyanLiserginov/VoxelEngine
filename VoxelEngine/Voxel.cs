using System;
using System.Collections.Generic;
using System.Text;
using Urho3DNet;

namespace VoxelEngine
{
    [ObjectFactory(Category = "Component/Game")]
    [Preserve(AllMembers = true)]
    internal partial class Voxel : LogicComponent
    {
        private CustomGeometry _customGeometry = null;
        private Material _material;
        private Vector3 _position;
        private float _size = 1;
        public bool Z_Plus = true;
        public bool Z_Minus = true;
        public bool X_Plus = true;
        public bool X_Minus = true;
        public bool Y_Plus = true;
        public bool Y_Minus = true;


        public Voxel(Context context) : base(context)
        {
            _material = new Material(Context);
            //_material = Context.GetSubsystem<ResourceCache>().GetResource<Material>("Materials/Black.material");
            _material.CullMode = CullMode.CullNone;
            _material.NumTechniques = 1;
            _material.SetTechnique(0, GetSubsystem<ResourceCache>().GetResource<Technique>("Techniques/LitOpaque.xml"));
            _material.SetShaderParameter("MatDiffColor", Color.White);
            _material.VertexShaderDefines = "VERTEXCOLOR";
            _material.PixelShaderDefines = "VERTEXCOLOR";
        }

        public override void Start()
        {
            _customGeometry = Node.CreateComponent<CustomGeometry>();
            _position = Node.Position;

            VoxelManager.RegisterVoxel(Node.Position, this);
            UpdateVisibility();
        }

        public void UpdateVisibility()
        {
            // Проверяем соседей и обновляем флаги видимости
            X_Plus = !VoxelManager.HasNeighbor(Node.Position, new Vector3(1, 0, 0));
            X_Minus = !VoxelManager.HasNeighbor(Node.Position, new Vector3(-1, 0, 0));
            Y_Plus = !VoxelManager.HasNeighbor(Node.Position, new Vector3(0, 1, 0));
            Y_Minus = !VoxelManager.HasNeighbor(Node.Position, new Vector3(0, -1, 0));
            Z_Plus = !VoxelManager.HasNeighbor(Node.Position, new Vector3(0, 0, 1));
            Z_Minus = !VoxelManager.HasNeighbor(Node.Position, new Vector3(0, 0, -1));

            // Перестраиваем геометрию
            UpdateGeometry();
        }

        public void UpdateGeometry()
        {
            _customGeometry.SetMaterial(_material);

            float halfSize = _size / 2;

            // Задаем грани
            int[][] faces = new int[][]
            {
                new int[] { 0, 1, 5, 4 }, // Y-
                new int[] { 2, 3, 7, 6 }, // Y+
                new int[] { 4, 5, 6, 7 }, // Z+ (передняя)
                new int[] { 1, 2, 6, 5 }, // X+ (правая)
                new int[] { 0, 3, 2, 1 }, // Z- (задняя)
                new int[] { 0, 4, 7, 3 }  // X- (левая)
            };

            // Задаем нормали граням
            Vector3[] faceNormals = new Vector3[]
            {
                new Vector3(0, -1, 0),   // Y-
                new Vector3(0, 1, 0),    // Y+
                new Vector3(0, 0, 1),    // Z+
                new Vector3(1, 0, 0),    // X+
                new Vector3(0, 0, -1),   // Z-
                new Vector3(-1, 0, 0)    // X-
            };

            // Определяем видимость граней
            bool[] visible = new bool[faces.Length];
            visible[0] = Y_Minus;
            visible[1] = Y_Plus;
            visible[2] = Z_Plus;
            visible[3] = X_Plus;
            visible[4] = Z_Minus;
            visible[5] = X_Minus;

            // Создаем массив точек куба (8 вершин)
            Vector3[] points = new Vector3[8];

            // Присваиваем координаты точкам куба
            // Задняя грань 
            points[0] = new Vector3(-halfSize, -halfSize, -halfSize); // задний-левый-нижний
            points[1] = new Vector3(halfSize, -halfSize, -halfSize);  // задний-правый-нижний
            points[2] = new Vector3(halfSize, halfSize, -halfSize);   // передний-правый-нижний
            points[3] = new Vector3(-halfSize, halfSize, -halfSize);  // передний-левый-нижний

            // Передняя грань
            points[4] = new Vector3(-halfSize, -halfSize, halfSize);  // задний-левый-верхний
            points[5] = new Vector3(halfSize, -halfSize, halfSize);   // задний-правый-верхний
            points[6] = new Vector3(halfSize, halfSize, halfSize);    // передний-правый-верхний
            points[7] = new Vector3(-halfSize, halfSize, halfSize);   // передний-левый-верхний

            Color[] faceColors = new Color[]
            {
                Color.Red,    // нижняя
                Color.Blue,   // верхняя
                Color.Green,  // передняя
                Color.Yellow, // правая
                Color.Cyan,   // задняя
                Color.Magenta // левая
            };

            _customGeometry.BeginGeometry(0, PrimitiveType.TriangleList);
            GeometryBuilder builder = new GeometryBuilder(_customGeometry);
            for (int i = 0; i < faces.Length; i++)
            {
                if (visible[i])
                {
                    SimpleVertex[] face = new SimpleVertex[4];
                    Vector3 normal = faceNormals[i]; // Нормаль для текущей грани

                    for (int j = 0; j < 4; j++)
                    {
                        // Создаем вершину с нормалью
                        face[j] = new SimpleVertex(points[faces[i][j]], normal, faceColors[i]);
                    }
                    builder.BuildSolidQuad(face);
                }
            }

            _customGeometry.Commit();
        }

        public void Unregister()
        {
            VoxelManager.UnregisterVoxel(_position);
        }

        public override void Stop()
        {
            Unregister();
            base.Stop();
        }
    }
}
