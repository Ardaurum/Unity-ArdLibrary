namespace ArdEngine.ResourceReference
{
    public interface IResourceReferenceRepository<out T>
    {
        T GetValue(int key);
    }
}