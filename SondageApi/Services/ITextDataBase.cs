namespace SondageApi.Services
{
    public interface ITextDataBase
    {
        void SaveEntry(string entry);
        void SaveEntries(string[] entries);
    }
}