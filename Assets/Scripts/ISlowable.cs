public interface ISlowable
{
    public abstract void OnSlowStart(float slowMultiplier);
    public abstract void OnSlowStop();
}
