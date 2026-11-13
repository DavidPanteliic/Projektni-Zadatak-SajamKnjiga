using BookFair.Core.Serialization;
using BookFair.Core.Utils;

namespace BookFair.Core.Storage;

// Source: OISISI, Vezbe 3, SerializationExample/Storage.cs

public class Storage<T> where T : ISerializable, new()
{
    private readonly string _filePath;
    private readonly Serializer<T> _serializer = new();

    public Storage(string fileName)
    {
        _filePath = DataPaths.GetFilePath(fileName);
    }

    public List<T> Load()
    {
        if (!File.Exists(_filePath))
        {
            return new List<T>();
        }

        IEnumerable<string> lines = File.ReadLines(_filePath);
        List<T> objects = _serializer.FromCSV(lines);

        return objects;
    }

    public void Save(List<T> objects)
    {
        string serializedObjects = _serializer.ToCSV(objects);

        Directory.CreateDirectory(DataPaths.DataDirectory);
        using (StreamWriter streamWriter = new StreamWriter(_filePath))
        {
            streamWriter.Write(serializedObjects);
        }
    }
}
