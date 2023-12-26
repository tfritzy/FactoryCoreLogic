// For copy paste: ₀₁₂₃₄₅₆₇₈₉

namespace Core
{
    public static class Constants
    {
        public const float HEX_APOTHEM = 0.86602540378f;
        public const float HEX_RADIUS = 1f;
        public const float HEX_HEIGHT = .5f;

        public static class Alliance
        {
            public const int NEUTRAL = 0;
        }

        public static class SpecificHeatCapacity
        {
            public static float Aluminum = 903f;
            public static float Copper = 401f;
            public static float Iron = 80.2f;
            public static float Lead = 129f;
            public static float Alumina = 765f;
            public static float Sulfur = 708f;
            public static float Plywood = 1300f;
            public static float FireclayBrick = 1000f;
            public static float Concrete = 880f;
            public static float AluminaRefractory = 880f;

        }

        public static class HeatTransferCoefficient
        {
            public static float Aluminum = 205f;
            public static float Copper = 350f;
            public static float Iron = 100f;
            public static float Lead = 35f;
            public static float Alumina = 30f;
            public static float Sulfur = 0.205f;
            public static float Plywood = 15f;
            public static float FireclayBrick = 10f;
            public static float AluminaRefractory = 2f;
        }
    }
}