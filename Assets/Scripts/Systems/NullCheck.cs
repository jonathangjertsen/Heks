public static class NotNull
{
    public static void Check(object obj)
    {
        if (obj == null)
        {
            throw new System.Exception("Object is null");
        }
    }
}
