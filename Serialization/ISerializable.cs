namespace Schema
{
    public interface ISerializable<T>
    {
        T ToSchema();
    }
}