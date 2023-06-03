using System;

namespace Core
{
    public class CubeCoord
    {
        public int q;
        public int r;
        public int s;

        public CubeCoord(int q, int r, int s)
        {
            this.q = q;
            this.r = r;
            this.s = s;
        }

        public CubeCoord(int q, int r)
        {
            this.q = q;
            this.r = r;
            this.s = -q - r;
        }

        public override bool Equals(object? obj)
        {
            if (obj is CubeCoord other)
            {
                return q == other.q && r == other.r && s == other.s;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return q.GetHashCode() ^ r.GetHashCode() ^ s.GetHashCode();
        }

        public override string ToString()
        {
            return $"({q}, {r}, {s})";
        }

        // Overloads the math operators
        public static CubeCoord operator +(CubeCoord a, CubeCoord b)
        {
            return new CubeCoord(a.q + b.q, a.r + b.r, a.s + b.s);
        }

        public static CubeCoord operator -(CubeCoord a, CubeCoord b)
        {
            return new CubeCoord(a.q - b.q, a.r - b.r, a.s - b.s);
        }

        public static CubeCoord operator *(CubeCoord a, int b)
        {
            return new CubeCoord(a.q * b, a.r * b, a.s * b);
        }

        public static bool operator ==(CubeCoord a, CubeCoord b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(CubeCoord a, CubeCoord b)
        {
            return !a.Equals(b);
        }

        public CubeCoord Sign()
        {
            return new CubeCoord(Math.Sign(q), Math.Sign(r), Math.Sign(s));
        }
    }
}