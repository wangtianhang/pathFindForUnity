using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public struct Vector3
    {
        public const float kEpsilon = 1e-005f;

        public float x;
        public float y;
        public float z;

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            Vector3 ret = new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
            return ret;
        }

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            Vector3 ret = new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
            return ret;
        }

        public float sqrMagnitude
        {
            get 
            {
                return x * x + y * y + z * z;
            } 
        }

        public void Normalize()
        {
            float length = (float)Math.Sqrt(x * x + y * y + z * z);
            x = x / length;
            y = y / length;
            z = z / length;
        }

        public static float Dot(Vector3 a, Vector3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }
    }
}
