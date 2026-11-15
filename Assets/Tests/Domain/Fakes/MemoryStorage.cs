using Domain.Ports.Driven;
using System.Collections.Generic;

namespace Domain.Tests
{
    // An IStorage that keeps everything in memory, so tests never touch PlayerPrefs.
    public class MemoryStorage : IStorage
    {
        private readonly Dictionary<string, object> _data = new Dictionary<string, object>();

        public void Save<T>(string key, T data) => _data[key] = data;
        public T Load<T>(string key) => _data.TryGetValue(key, out var value) ? (T)value : default;
        public bool HasKey(string key) => _data.ContainsKey(key);
        public void Delete(string key) => _data.Remove(key);
    }
}
