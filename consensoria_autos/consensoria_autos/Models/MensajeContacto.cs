using System;

namespace consensoria_autos.Models
{
    public class Message
    {
        public int id { get; set; }
        public string first_name { get; set; } = string.Empty;
        public string? last_name { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public string content { get; set; } = string.Empty;
        public int? car_id { get; set; }
        public DateTime? received_at { get; set; }
        public bool? is_read { get; set; }

        // 🔹 Relación opcional (coherente con Prisma)
        public Car? cars { get; set; }
    }
}
