using System;

public interface SchemaOf<T>
{
    public T FromSchema(params object[] context);
}

public interface ISchema<TSchema>
{
    TSchema ToSchema();
}