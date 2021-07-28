using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularNullable<T> where T : struct
{
    private T? _value;
    public T value { get => _value.Value; }
    public bool hasValue { get => _value.HasValue; }

    public CircularNullable(T value)
    {
        this._value = value;
    }
    public CircularNullable()
    {
        this._value = null;
    }
}

/*
public class CircularNullable<T> where T : struct
{
    public T value { get; }
    public bool hasValue { get; }

    public CircularNullable(T value)
    {
        this.hasValue = true;
        this.value = value;
    }
    public CircularNullable()
    {
        this.hasValue = false;
        this.value = new T();
    }
}
*/