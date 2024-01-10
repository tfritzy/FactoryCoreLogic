using System.Collections.Generic;
using System.Linq;
using Core;
using Schema;

public class TriangleData
{
    public struct TraingleConfig
    {
        public TriangleType[] Types;
        public TriangleSubType[] SubTypes;
    }

    public static readonly List<TraingleConfig> TriangleConfigs = new()
    {
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
        },
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
        },
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
                    foreach (var type in item.Types)
                    {
                        if (!_availableSubTypes.ContainsKey(type))
                        {
                            _availableSubTypes.Add(type, item.SubTypes);
                        }
                        else
                        {
                            _availableSubTypes[type] = _availableSubTypes[type].Concat(item.SubTypes).ToArray();
                        }
                    }
                }
            }

            return _availableSubTypes;
        }
    }
}