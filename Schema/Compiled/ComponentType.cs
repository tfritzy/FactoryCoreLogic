// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: component_type.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Core {

  /// <summary>Holder for reflection information generated from component_type.proto</summary>
  public static partial class ComponentTypeReflection {

    #region Descriptor
    /// <summary>File descriptor for component_type.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ComponentTypeReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChRjb21wb25lbnRfdHlwZS5wcm90bxIEY29yZSrFAQoNQ29tcG9uZW50VHlw",
            "ZRIUChBJbnZhbGlkQ29tcG9uZW50EAASFQoRQ29udmV5b3JDb21wb25lbnQQ",
            "ARINCglJbnZlbnRvcnkQAhIICgRMaWZlEAMSDQoJV29ybkl0ZW1zEAQSDwoL",
            "QWN0aXZlSXRlbXMQBRIKCgZBdHRhY2sQBhISCg5Ub3dlclRhcmdldGluZxAH",
            "EggKBE1pbmUQCBIMCghJdGVtUG9ydBAJEgkKBVNtZWx0EAoSCwoHQ29tbWFu",
            "ZBALYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::Core.ComponentType), }, null, null));
    }
    #endregion

  }
  #region Enums
  public enum ComponentType {
    [pbr::OriginalName("InvalidComponent")] InvalidComponent = 0,
    [pbr::OriginalName("ConveyorComponent")] ConveyorComponent = 1,
    [pbr::OriginalName("Inventory")] Inventory = 2,
    [pbr::OriginalName("Life")] Life = 3,
    [pbr::OriginalName("WornItems")] WornItems = 4,
    [pbr::OriginalName("ActiveItems")] ActiveItems = 5,
    [pbr::OriginalName("Attack")] Attack = 6,
    [pbr::OriginalName("TowerTargeting")] TowerTargeting = 7,
    [pbr::OriginalName("Mine")] Mine = 8,
    [pbr::OriginalName("ItemPort")] ItemPort = 9,
    [pbr::OriginalName("Smelt")] Smelt = 10,
    [pbr::OriginalName("Command")] Command = 11,
  }

  #endregion

}

#endregion Designer generated code
