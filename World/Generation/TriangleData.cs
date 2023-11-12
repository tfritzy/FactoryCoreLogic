using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

public class TriangleData
{
    public struct TraingleConfig
    {
        public TriangleType[] Types;
        public SubType[] SubTypes;
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
                SubTypes=new SubType[]
                {
                    SubType.LandInterior,
                    SubType.LandExterior,
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
                SubTypes=new SubType[]
                {
                    SubType.BrickHalf1,
                    SubType.BrickHalf1,
                    SubType.FullBrick,
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
                SubTypes=new SubType[]
                {
                    SubType.Liquid,
                }
            }
        }
    };

    private static Dictionary<TriangleType, SubType[]> _availableSubTypes = null!;
    public static Dictionary<TriangleType, SubType[]> AvailableSubTypes
    {
        get
        {
            if (_availableSubTypes == null)
            {
                _availableSubTypes = new Dictionary<TriangleType, SubType[]>();
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