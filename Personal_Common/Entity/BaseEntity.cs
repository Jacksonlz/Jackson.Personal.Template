using System.Text.Json.Serialization;

namespace Personal.Common.Entity
{
    public abstract record BaseEntity
    {
        [JsonIgnore]
        public bool IsDeleted { get; set; }
        [JsonIgnore]
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;
        [JsonIgnore]
        public DateTime? UpdatedTime { get; set; }
        public string? CreateBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}