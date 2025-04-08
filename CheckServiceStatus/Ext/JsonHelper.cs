using CheckServiceStatus.Models;
using System.Text.Json;


namespace CheckServiceStatus.Ext
{
    public static class JsonHelper
    {
        public static string ToStringJson(this ServiceModel obj) 
            => JsonSerializer.Serialize(obj);


        public static string ToStringJson(this object obj)
            => JsonSerializer.Serialize(obj);
    }
}
