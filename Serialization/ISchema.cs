public interface SchemaOf<T>
{
    public T FromSchema(params object[] context);
}