// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: item_type.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Core {

  /// <summary>Holder for reflection information generated from item_type.proto</summary>
  public static partial class ItemTypeReflection {

    #region Descriptor
    /// <summary>File descriptor for item_type.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ItemTypeReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cg9pdGVtX3R5cGUucHJvdG8SBGNvcmUiNQoQTnVsbGFibGVJdGVtVHlwZRIh",
            "CglpdGVtX3R5cGUYASABKA4yDi5jb3JlLkl0ZW1UeXBlKo8DCghJdGVtVHlw",
            "ZRITCg9JbnZhbGlkSXRlbVR5cGUQABINCglMaW1lc3RvbmUQARISCg5MaW1l",
            "c3RvbmVCcmljaxACEhgKFExpbWVzdG9uZURvdWJsZUJyaWNrEAMSCAoERGly",
            "dBAEEggKBFdvb2QQBRIJCgVTdGljaxAGEgoKBkxlYXZlcxAHEg0KCUFycm93",
            "aGVhZBAIEg0KCVRvb2xTaGFmdBAJEgcKA0xvZxAKEgsKB0lyb25CYXIQCxIT",
            "Cg9Jcm9uU2lsaWNvblNsYWcQDBINCglDb3BwZXJCYXIQDRIPCgtJcm9uUGlj",
            "a2F4ZRAOEgwKCENvbnZleW9yEA8SEgoOQW50aHJhY2l0ZUNvYWwQEBISCg5C",
            "aXR1bWlub3VzQ29hbBAREg8KC0xpZ25pdGVDb2FsEBISDQoJTWluZXNoYWZ0",
            "EBMSDwoLQ2xheUZ1cm5hY2UQFBIKCgZTb3J0ZXIQFRIJCgVEZXBvdBAWEhAK",
            "DENoYWxjb3B5cml0ZRAXEg0KCU1hZ25ldGl0ZRAYYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::Core.ItemType), }, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Core.NullableItemType), global::Core.NullableItemType.Parser, new[]{ "ItemType" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Enums
  public enum ItemType {
    [pbr::OriginalName("InvalidItemType")] InvalidItemType = 0,
    [pbr::OriginalName("Limestone")] Limestone = 1,
    [pbr::OriginalName("LimestoneBrick")] LimestoneBrick = 2,
    [pbr::OriginalName("LimestoneDoubleBrick")] LimestoneDoubleBrick = 3,
    [pbr::OriginalName("Dirt")] Dirt = 4,
    [pbr::OriginalName("Wood")] Wood = 5,
    [pbr::OriginalName("Stick")] Stick = 6,
    [pbr::OriginalName("Leaves")] Leaves = 7,
    [pbr::OriginalName("Arrowhead")] Arrowhead = 8,
    [pbr::OriginalName("ToolShaft")] ToolShaft = 9,
    [pbr::OriginalName("Log")] Log = 10,
    [pbr::OriginalName("IronBar")] IronBar = 11,
    [pbr::OriginalName("IronSiliconSlag")] IronSiliconSlag = 12,
    [pbr::OriginalName("CopperBar")] CopperBar = 13,
    [pbr::OriginalName("IronPickaxe")] IronPickaxe = 14,
    [pbr::OriginalName("Conveyor")] Conveyor = 15,
    [pbr::OriginalName("AnthraciteCoal")] AnthraciteCoal = 16,
    [pbr::OriginalName("BituminousCoal")] BituminousCoal = 17,
    [pbr::OriginalName("LigniteCoal")] LigniteCoal = 18,
    [pbr::OriginalName("Mineshaft")] Mineshaft = 19,
    [pbr::OriginalName("ClayFurnace")] ClayFurnace = 20,
    [pbr::OriginalName("Sorter")] Sorter = 21,
    [pbr::OriginalName("Depot")] Depot = 22,
    [pbr::OriginalName("Chalcopyrite")] Chalcopyrite = 23,
    [pbr::OriginalName("Magnetite")] Magnetite = 24,
  }

  #endregion

  #region Messages
  [global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
  public sealed partial class NullableItemType : pb::IMessage<NullableItemType>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<NullableItemType> _parser = new pb::MessageParser<NullableItemType>(() => new NullableItemType());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<NullableItemType> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Core.ItemTypeReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public NullableItemType() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public NullableItemType(NullableItemType other) : this() {
      itemType_ = other.itemType_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public NullableItemType Clone() {
      return new NullableItemType(this);
    }

    /// <summary>Field number for the "item_type" field.</summary>
    public const int ItemTypeFieldNumber = 1;
    private global::Core.ItemType itemType_ = global::Core.ItemType.InvalidItemType;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::Core.ItemType ItemType {
      get { return itemType_; }
      set {
        itemType_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as NullableItemType);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(NullableItemType other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (ItemType != other.ItemType) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (ItemType != global::Core.ItemType.InvalidItemType) hash ^= ItemType.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (ItemType != global::Core.ItemType.InvalidItemType) {
        output.WriteRawTag(8);
        output.WriteEnum((int) ItemType);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (ItemType != global::Core.ItemType.InvalidItemType) {
        output.WriteRawTag(8);
        output.WriteEnum((int) ItemType);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      if (ItemType != global::Core.ItemType.InvalidItemType) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) ItemType);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(NullableItemType other) {
      if (other == null) {
        return;
      }
      if (other.ItemType != global::Core.ItemType.InvalidItemType) {
        ItemType = other.ItemType;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            ItemType = (global::Core.ItemType) input.ReadEnum();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 8: {
            ItemType = (global::Core.ItemType) input.ReadEnum();
            break;
          }
        }
      }
    }
    #endif

  }

  #endregion

}

#endregion Designer generated code
