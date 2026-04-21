using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NyahahahahaMap;

public static class BinarySaver {
    public static void SaveList<T>(List<T> dataList, string path) {
        var formatter = new BinaryFormatter();
        using var stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, dataList);
    }

    public static List<T> LoadList<T>(string path) {
        if (!File.Exists(path)) return new List<T>();

        var formatter = new BinaryFormatter();
        using var stream = new FileStream(path, FileMode.Open);
        return (List<T>)formatter.Deserialize(stream);
    }
}