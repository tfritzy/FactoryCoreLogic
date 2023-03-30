public interface ISchema<T>
{
    T FromSchema(params object[] context);
}