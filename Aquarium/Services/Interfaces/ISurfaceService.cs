namespace Aquarium.Services
{
    public interface ISurfaceService
    {
        void Run();
        void Stop();
        int GetValue();
        int SaveValue(int value);
    }
}