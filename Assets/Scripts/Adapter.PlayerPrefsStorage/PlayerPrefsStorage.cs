using UnityEngine;

public class PlayerPrefsStorage : IStorage
{
    public void Delete(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }

    public bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    public T Load<T>(string key)
    {
        var type = typeof(T);
        if (type == typeof(int)) return (T)(object)PlayerPrefs.GetInt(key);
        else if (type == typeof(float)) return (T)(object)PlayerPrefs.GetFloat(key);
        else if (type == typeof(string)) return (T)(object)PlayerPrefs.GetString(key);
        else throw new System.NotSupportedException($"Type {type} is not supported by PlayerPrefsStorage.");
    }

    public void Save<T>(string key, T data)
    {
        var type = typeof(T);
        if (type == typeof(int)) PlayerPrefs.SetInt(key, (int)(object)data);
        else if (type == typeof(float)) PlayerPrefs.SetFloat(key, (float)(object)data);
        else if (type == typeof(string)) PlayerPrefs.SetString(key, (string)(object)data);
        else throw new System.NotSupportedException($"Type {type} is not supported by PlayerPrefsStorage.");
    }
}
