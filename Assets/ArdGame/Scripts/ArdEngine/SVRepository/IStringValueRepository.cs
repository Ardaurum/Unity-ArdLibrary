namespace ArdEngine.SVRepository
{
    public interface IStringValueRepository<out T>
    {
        T GetValue(int key);
    }
}