using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Translator.Core.Utility
{
    public static class BinarySerializer
    {
        public static T Deserialize<T>(this byte[] stream)
        {
            var bf = new BinaryFormatter();
            using (var memoryStream = new MemoryStream(stream))
            {
                return (T)bf.Deserialize(memoryStream);
            }
        }

        public static byte[] Serialize<T>(this T obj)
        {
            var bf = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                bf.Serialize(memoryStream, obj);
                return memoryStream.GetBuffer();
            }
        }

        public static T CreateCopy<T>(this T obj)
        {
            return obj.Serialize().Deserialize<T>();
        }        
    }
}

