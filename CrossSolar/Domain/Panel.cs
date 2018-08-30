﻿using System.ComponentModel.DataAnnotations;

namespace CrossSolar.Domain
{
    public class Panel
    {
        public int Id { get; set; }
        
        [Required]
        [Range(-90, 90)]
        public double Latitude { get; set; }

        [Range(-180, 180)]
        [Required]
        public double Longitude { get; set; }

        [Required]
        [MaxLength(16)]
        public string Serial { get; set; }

        public string Brand { get; set; }
    
    }
}