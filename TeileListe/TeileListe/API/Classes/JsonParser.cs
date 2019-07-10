using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace TeileListe.API.Classes
{
    internal class JsonParser
    {
        public object ConvertJson(Type T, string json)
        {
            var serializer = new DataContractJsonSerializer(T);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            using (var stream = new MemoryStream(bytes))
            {
                var deserialized = serializer.ReadObject(stream);
                return deserialized;
            }
        }
    }
}
