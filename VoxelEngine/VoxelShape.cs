using System;
using System.Collections.Generic;
using System.Text;
using Urho3DNet;

namespace VoxelEngine
{
    public interface IVoxelShape
    {
        bool IsInside(int x, int y, int z, int size);
        string Name { get; } // полезно для логгирования/отладки
    }

    public static class VoxelShapeFactory
    {
        public static IVoxelShape Create<T>() where T : IVoxelShape, new()
        {
            return new T();
        }
    }

    public class SierpinskiTetrahedron : IVoxelShape
    {
        public string Name => "Sierpinski Tetrahedron";

        public bool IsInside(int x, int y, int z, int size)
        {
            // Нормализуем координаты к текущему "уровню" размера size
            // Приводим размер сцены(32) к степени двойки(32 = 2 ^ 5)
            int maxIterations = 5;

            for (int iter = 0; iter < maxIterations; iter++)
            {
                // Смотрим, в какой части куба мы находимся
                if (x >= size / 2)
                {
                    if (y >= size / 2) return false; // Отсекаем один из углов
                    if (z >= size / 2) return false; // Отсекаем другой угол
                    x -= size / 2;
                }
                else if (y >= size / 2)
                {
                    if (z >= size / 2) return false; // Отсекаем третий угол
                    y -= size / 2;
                }
                else if (z >= size / 2)
                {
                    z -= size / 2;
                }
                else
                {
                    // Мы в центральной части, которая всегда заполнена
                    return true;
                }
                size /= 2; // Переходим на следующий, более мелкий уровень
            }
            return true;
        }
    }

    public class Sphere : IVoxelShape
    {
        public string Name => "Sphere";

        public bool IsInside(int x, int y, int z, int size)
        {
            int centerX = size / 2;
            int centerY = size / 2;
            int centerZ = size / 2;
            int radius = size / 2;
            int dx = x - centerX;
            int dy = y - centerY;
            int dz = z - centerZ;
            return dx * dx + dy * dy + dz * dz <= radius * radius;
        }
    }

    public class SpongeCorner : IVoxelShape
    {
        public string Name => "Sponge Corner";

        public bool IsInside(int x, int y, int z, int size)
        {
            while (size > 1)
            {
                size /= 2;
                if (x >= size && y >= size && z >= size)
                    return false;

                x %= size;
                y %= size;
                z %= size;
            }
            return true;
        }
    }

    public class Metaballs : IVoxelShape
    {
        public string Name => "Metaballs";

        public bool IsInside(int x, int y, int z, int size)
        {
            // Используем размер для нормализации
            float scale = 1.2f;
            float nx = x * scale;
            float ny = y * scale;
            float nz = z * scale;

            // Фиксированные центры в нормализованных координатах
            float density = 0f;
            density += 100f / (DistanceSq(nx, ny, nz, 20, 20, 10) + 1);
            //density += 80f / (DistanceSq(nx, ny, nz, 40, 30, 30) + 1);
            density += 90f / (DistanceSq(nx, ny, nz, 20, 20, 30) + 1);

            return density > 1.5f;
        }

        private float DistanceSq(float x1, float y1, float z1, float x2, float y2, float z2)
        {
            float dx = x1 - x2;
            float dy = y1 - y2;
            float dz = z1 - z2;
            return dx * dx + dy * dy + dz * dz;
        }
    }



}
