public class SystemParticipator : ISystemParticipator
{
    public bool As<T>(out T converted) => ConvertToInterface.As(this, out converted);
}
