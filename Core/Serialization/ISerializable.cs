namespace BookFair.Core.Serialization;

// Source: OISISI, Vezbe 3, SerializationExample/Serializable.cs

public interface ISerializable
{
    string[] ToCSV();

    void FromCSV(string[] values);
}
