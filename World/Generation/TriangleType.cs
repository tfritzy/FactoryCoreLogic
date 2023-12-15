public enum TraingleClass
{
    Invalid,
    Land,
    Brick,
    Liquid
}

public enum TriangleType
{
    Invalid,
    Dirt,
    Stone,
    StoneBrick,
    Water,
}

// https://twitter.com/OskSta/status/1448248658865049605/photo/3

public enum TriangleSubType
{
    Invalid,
    LandActuallyFull, // Optimization to combine 6 full triangles into 1. Doesn't actually live in the datastructure
    LandFull,
    LandInnyLeft,
    LandInnyRight,
    LandInnyBoth,
    LandOuty,
    BrickHalf1,
    BrickHalf2,
    FullBrick,
    Liquid
}