namespace Domain.Ports.Driven
{
    public interface IStorage
    {
        void Save<T>(string key, T data);
        T Load<T>(string key);
        bool HasKey(string key);
        void Delete(string key);
    }
}
