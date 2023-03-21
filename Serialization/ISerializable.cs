namespace FactoryCore
{
    public interface ISerializable<T>
    {
        string ToSchema();
        T FromSchema(string text);
    }
}