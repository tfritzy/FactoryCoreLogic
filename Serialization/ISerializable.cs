namespace Schema
{
    public interface SerializesTo<T>
    {
        T ToSchema();
    }
}