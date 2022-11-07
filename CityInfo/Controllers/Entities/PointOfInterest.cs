﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfo.API.Entities
{
    public class PointOfInterest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [ForeignKey("CityId")]          //Not necessary, but better for readability
        public City? City { get; set; }
        public int CityId { get; set; }
        public string? Description { get; set; }

        public PointOfInterest(string name)
        {
            Name = name;
        }
    }
}

