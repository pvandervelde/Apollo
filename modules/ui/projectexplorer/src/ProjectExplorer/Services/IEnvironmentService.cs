namespace ContactManager.Services
{
    public interface IEnvironmentService
    {
        void GarbageCollect();
        void CloseApplication();
    }
}


