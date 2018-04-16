using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class HttpHandlerAttribute : Attribute
{
    public string Tag;

    public HttpHandlerAttribute(string tag)
    {
        this.Tag = tag;
    }
}