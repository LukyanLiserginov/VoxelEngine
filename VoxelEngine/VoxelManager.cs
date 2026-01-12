using System;
using System.Collections.Generic;
using System.Text;
using Urho3DNet;

namespace VoxelEngine
{
    internal static class VoxelManager
    {
        public static Dictionary<Vector3, Voxel> Voxels = new Dictionary<Vector3, Voxel>();

        public static void RegisterVoxel(Vector3 position, Voxel voxel)
        {
            Voxels[position] = voxel;
            UpdateNeighborsVisibility(position);
        }

        public static void UnregisterVoxel(Vector3 position)
        {
            if (Voxels.ContainsKey(position))
            {
                Voxels.Remove(position);
                UpdateNeighborsVisibility(position);
            }
        }

        private static void UpdateNeighborsVisibility(Vector3 changedPosition)
        {
            // ќбновл€ем видимость граней у соседних вокселей
            Vector3[] directions = {
            new Vector3(1, 0, 0),  // right
            new Vector3(-1, 0, 0), // left
            new Vector3(0, 1, 0),  // up
            new Vector3(0, -1, 0), // down
            new Vector3(0, 0, 1),  // forward
            new Vector3(0, 0, -1)  // back
        };
            

            foreach (var direction in directions)
            {
                Vector3 neighborPos = changedPosition + direction;
                if (Voxels.TryGetValue(neighborPos, out Voxel neighbor))
                {
                    neighbor.UpdateVisibility();
                }
            }
        }

        public static bool HasNeighbor(Vector3 position, Vector3 direction)
        {
            return Voxels.ContainsKey(position + direction);
        }
    }
}
