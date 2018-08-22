namespace Saving
{
    public interface ISerializer
    {
        void Restore();
        void Load(string path);
        void Save(string path);
    }
}