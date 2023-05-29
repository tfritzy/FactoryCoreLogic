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
    }
}
