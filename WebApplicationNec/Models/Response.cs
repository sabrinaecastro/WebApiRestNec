using Newtonsoft.Json;

namespace WebApplicationMostrar.Models
{
    public class Response<T>
    {
        [JsonProperty("data")]
        public T Data { get; set; }

        [JsonProperty("status")]
        public Status Status { get; set; }
    }
}
