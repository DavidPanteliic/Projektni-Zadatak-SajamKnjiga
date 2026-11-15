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

            // Pokvaren red (rucno izmenjen fajl, fali polje, los broj ili datum)
            // se preskace, da ne bi srusio ceo program pri pokretanju
            try
            {
                obj.FromCSV(csvValues);
            }
            catch (Exception ex) when (ex is FormatException or IndexOutOfRangeException
                                           or OverflowException or ArgumentException)
            {
                continue;
            }

            objects.Add(obj);
        }

        return objects;
    }

    // Provera pre izmene liste u DAO-u, da los objekat ne bi ostao u memoriji
    public void Validate(T obj)
    {
        CheckValues(obj.ToCSV());
    }

    private static void CheckValues(string[] values)
    {
        foreach (string value in values)
        {
            if (value != null && (value.Contains(Delimiter) || value.Contains('\n') || value.Contains('\r')))
            {
                throw new ArgumentException(
                    $"Vrednost \"{value}\" ne sme da sadrzi znak '{Delimiter}' niti prelazak u novi red.");
            }
        }
    }
}
