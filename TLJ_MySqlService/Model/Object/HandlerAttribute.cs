using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class HandlerAttribute : Attribute
{
    public string Tag;

    public HandlerAttribute(string tag)
    {
        this.Tag = tag;
    }
}