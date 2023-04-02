public interface SchemaOf<T>
{
    T FromSchema(params object[] context);
}