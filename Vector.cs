using System;

namespace SharpEngine
{
    public struct Vector
    {
        public float x, y, z;

        public Vector(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0;
        }

        public static Vector operator +(Vector lhs, Vector rhs)
        {
            return new Vector(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
        }
        
        public static Vector operator -(Vector lhs, Vector rhs)
        {
            return new Vector(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
        }
        
        
        public static Vector operator *(Vector v, float f)
        {
            return new Vector(v.x * f, v.y * f, v.z * f);
        }
        
        public static Vector operator /(Vector v, float f)
        {
            return new Vector(v.x / f, v.y / f, v.z / f);
        }

        public static Vector Max(Vector v, Vector u)
        {
            return new Vector(MathF.Max(v.x, u.x), MathF.Max(v.y, u.y), MathF.Max(v.z, u.z));
        }
        
        public static Vector Min(Vector v, Vector u)
        {
            return new Vector(MathF.Min(v.x, u.x), MathF.Min(v.y, u.y), MathF.Min(v.z, u.z));
        }
        
    }
}