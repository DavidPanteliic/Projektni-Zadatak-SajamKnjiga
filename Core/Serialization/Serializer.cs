using System.Text;

namespace BookFair.Core.Serialization;

// Source: OISISI, Vezbe 3, SerializationExample/Serializer.cs

public class Serializer<T> where T : ISerializable, new()
{
    private const char Delimiter = '|';

    public string ToCSV(List<T> objects)
    {
        StringBuilder sb = new StringBuilder();

        foreach (ISerializable obj in objects)
        {
            string[] values = obj.ToCSV();
            CheckValues(values);

            string line = string.Join(Delimiter.ToString(), values);
            sb.AppendLine(line);
        }

        return sb.ToString();
    }

    public List<T> FromCSV(IEnumerable<string> lines)
    {
        List<T> objects = new List<T>();

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            string[] csvValues = line.Split(Delimiter);
            T obj = new T();
            obj.FromCSV(csvValues);
            objects.Add(obj);
        }

        return objects;
    }

    private static void CheckValues(string[] values)
    {
        foreach (string value in values)
        {
            if (value != null && (value.Contains(Delimiter) || value.Contains('\n') || value.Contains('\r')))
            {
                throw new ArgumentException(
                    $"Value \"{value}\" must not contain the '{Delimiter}' character or a line break.");
            }
        }
    }
}
