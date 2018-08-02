using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathFind
{

    public struct Vector2i
    {
        public Vector2i(int x, int y)
        {
            m_x = x;
            m_y = y;
        }

        public int m_x;
        public int m_y;

        public float magnitude()
        {
            float sqr_magnitude = m_x * m_x + m_y * m_y;
            return (float)Math.Sqrt(sqr_magnitude);
        }

        public float sqrMagnitude()
        {
            return m_x * m_x + m_y * m_y;
        }

        public static Vector2i operator -(Vector2i a, Vector2i b)
        {
            Vector2i ret = new Vector2i(a.m_x - b.m_x, a.m_y - b.m_y);
            return ret;
        }

        public static bool operator ==(Vector2i lhs, Vector2i rhs)
        {
            if (lhs.m_x == rhs.m_x
                && lhs.m_y == rhs.m_y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Vector2i operator +(Vector2i a, Vector2i b)
        {
            Vector2i ret = new Vector2i(a.m_x + b.m_x, a.m_y + b.m_y);
            return ret;
        }

        public static bool operator !=(Vector2i lhs, Vector2i rhs)
        {
            if (lhs.m_x != rhs.m_x
                || lhs.m_y != rhs.m_y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
