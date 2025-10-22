using System;
using System.Collections.Generic;

namespace consensoria_autos.Models
{
    public class Car
    {
        public int id { get; set; }
        public string model { get; set; } = "";
        public string? description { get; set; }
        public int year { get; set; }
        public decimal? price { get; set; }
        public string? image_url { get; set; }     // Imagen principal (opcional)
        public bool is_published { get; set; } = true;
        public int? transmission_id { get; set; }
        public int? condition_id { get; set; }
        public int? created_by { get; set; } = 1;
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? sold_at { get; set; }
        public bool is_sold { get; set; } = false;

        // propiedad recibirá las URLs subidas como JSON desde el formulario
        public string? images { get; set; }

        //  lista mostrará las imágenes ya asociadas al carro
        public List<CarImage>? car_images { get; set; }

        // Relaciones (opcionales)
        public Transmission? transmissions { get; set; }
        public CarCondition? car_conditions { get; set; }
    }

    // 🔸 clase para manejar las imágenes asociadas
    public class CarImage
    {
        public string url { get; set; } = "";
    }
}
