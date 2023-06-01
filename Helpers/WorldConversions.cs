using System;

namespace Core
{
    public static class WorldConversions
    {
        private const float HORIZONTAL_DIST = Constants.HEX_RADIUS * 1.5f;
        private const float VERTICAL_DIST = Constants.HEX_APOTHEM * 2;

        public static Point3Float HexToUnityPosition(Point3Int hexPosition)
        {
            return HexToUnityPosition(hexPosition.x, hexPosition.y, hexPosition.z);
        }

        public static Point3Float HexToUnityPosition(int x, int y, int z)
        {
            float xF = HORIZONTAL_DIST * x;
            float zF = VERTICAL_DIST * y + (x % 2 == 1 ? Constants.HEX_APOTHEM : 0);
            float yF = z * Constants.HEX_HEIGHT;
            return new Point3Float(xF, yF, zF);
        }

        public static Point3Int UnityToGrid(Point3Float unityPosition)
        {
            return UnityToGrid(unityPosition.x, unityPosition.y, unityPosition.z);
        }

        public static Point3Int UnityToGrid(float x, float y, float z)
        {
            float xF = x / HORIZONTAL_DIST;
            float zF = (z - (xF % 2 == 1 ? Constants.HEX_APOTHEM : 0)) / VERTICAL_DIST;
            float yF = y / Constants.HEX_HEIGHT;
            return new Point3Int((int)Math.Round(xF), (int)Math.Round(yF), (int)Math.Round(zF));
        }
    }
}
