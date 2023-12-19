using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

public class TriangleData
{
    public struct TraingleConfig
    {
        public TriangleType[] Types;
        public TriangleSubType[] SubTypes;
    }

    public static readonly Dictionary<TraingleClass, TraingleConfig> TriangleConfigs = new()
    {
        {
            TraingleClass.Land,
            new TraingleConfig()
            {
                Types=new TriangleType[]
                {
                    TriangleType.Dirt,
                    TriangleType.Stone,
                },
                SubTypes=new TriangleSubType[]
                {
                    TriangleSubType.LandFull,
                    TriangleSubType.LandInnyLeft,
                    TriangleSubType.LandInnyRight,
                    TriangleSubType.LandInnyBoth,
                    TriangleSubType.LandOuty,
                }
            }
        },
        {
            TraingleClass.Brick,
            new TraingleConfig()
            {
                Types=new TriangleType[]
                {
                    TriangleType.StoneBrick,
                },
                SubTypes=new TriangleSubType[]
                {
                    TriangleSubType.BrickHalf,
                    TriangleSubType.FullBrick,
                }
            }
        },
        {
            TraingleClass.Liquid,
            new TraingleConfig()
            {
                Types=new TriangleType[]
                {
                    TriangleType.Water,
                },
                SubTypes=new TriangleSubType[]
                {
                    TriangleSubType.Liquid,
                }
            }
        }
    };

    private static Dictionary<TriangleType, TriangleSubType[]> _availableSubTypes = null!;
    public static Dictionary<TriangleType, TriangleSubType[]> AvailableSubTypes
    {
        get
        {
            if (_availableSubTypes == null)
            {
                _availableSubTypes = new Dictionary<TriangleType, TriangleSubType[]>();
                foreach (var item in TriangleConfigs)
                {
                    foreach (var type in item.Value.Types)
                    {
                        if (!_availableSubTypes.ContainsKey(type))
                        {
                            _availableSubTypes.Add(type, item.Value.SubTypes);
                        }
                        else
                        {
                            _availableSubTypes[type] = _availableSubTypes[type].Concat(item.Value.SubTypes).ToArray();
                        }
                    }
                }
            }

            return _availableSubTypes;
        }
    }
}