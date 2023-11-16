using InnerCore.Api.DeConz.Models.Sensors;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace InnerCore.Api.DeConz.Models.WebSocket
{
    [DataContract]
    public class Message
    {
        [DataMember(Name = "t")]
        [JsonPropertyName("t")]
        public MessageType? Type { get; set; }

        [DataMember(Name = "e")]
        [JsonPropertyName("e")]
        public EventType? Event { get; set; }

        [DataMember(Name = "r")]
        [JsonPropertyName("r")]
        public ResourceType? ResourceType { get; set; }

        [DataMember]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [DataMember]
        [JsonPropertyName("state")]
        public SensorState State { get; set; }

        [DataMember]
        [JsonPropertyName("config")]
        public SensorConfig Config { get; set; }

        [JsonPropertyName("uniqueid")]
        public string UniqueId { get; set; }
    }
}
