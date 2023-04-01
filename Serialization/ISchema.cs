public interface ISchema<T>
{
    T FromSchema(params object[] context);
    string ToSchema(T toSerialize);
}