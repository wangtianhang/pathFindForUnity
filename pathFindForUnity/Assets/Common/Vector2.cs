using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    [Serializable]
    public struct Vector2
    {
        public const float kEpsilon = 1e-005f;

        public float x;
        public float y;

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static bool operator ==(Vector2 lhs, Vector2 rhs)
        {
            if (lhs.x == rhs.x
                && lhs.y == rhs.y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator !=(Vector2 lhs, Vector2 rhs)
        {
            if (lhs.x != rhs.x
                || lhs.y != rhs.y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            Vector2 ret = new Vector2(a.x - b.x, a.y - b.y);
            return ret;
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            Vector2 ret = new Vector2(a.x + b.x, a.y + b.y);
            return ret;
        }

        public override string ToString()
        {
            return "(" + x.ToString("f6") + "," + y.ToString("f6") + ")";
        }

        public float sqrMagnitude
        {
            get
            {
                return x * x + y * y;
            }
        }

        public void Normalize()
        {
            float length = (float)Math.Sqrt(x * x + y * y);
            x = x / length;
            y = y / length;
        }

        public static Vector2 operator *(float d, Vector2 a)
        {
            return new Vector2(a.x * d, a.y * d);
        }

        public static Vector2 operator *(Vector2 a, float d)
        {
            return new Vector2(a.x * d, a.y * d);
        }
    }
}
