using System;

public class SysParticipator : ISysParticipator
{
    public bool As<T>(out T converted) => ConvertToInterface.As(this, out converted);
}

public static class ConvertToInterface
{
    public static bool As<T>(object anything, out T converted)
    {
        try
        {
            object obj = anything;
            converted = (T)obj;
            return (converted != null);
        }
        catch (InvalidCastException)
        {
            converted = default;
            return false;
        }
    }
}