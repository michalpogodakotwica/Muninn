namespace Saving
{
    public interface ISaveable
    {
        void Restore();
        void Load(object data);
        object Save();
    }
}