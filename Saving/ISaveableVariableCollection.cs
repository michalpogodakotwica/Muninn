namespace Saving
{
    public interface ISaveableVariableCollection
    {
        void FromFile(string path);
        void ToFile(string path);
        void RestoreDefaultValues();
        void UnloadVariables();
    }
}