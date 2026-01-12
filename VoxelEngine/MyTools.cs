using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Urho3DNet;

namespace VoxelEngine
{
    internal static class MyTools
    {
        
        internal static Vector3 Round(Vector3 v)
        {
            var r = new Vector3((float)Math.Round(v.X), (float)Math.Round(v.Y), (float)Math.Round(v.Z));
            return r;
        }

        //save to csv
        public static string filePath = "model.csv";
        internal static void SaveBuild(List<Voxel> blocks)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Записываем данные
                foreach (var block in blocks)
                {
                }
            }
        }
    }
}
