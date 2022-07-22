﻿using System.ComponentModel.DataAnnotations;

namespace Alkemy.DTOs
{
    public class PeliculaCreacionDTO
    {
        public FormFile Imagen { get; set; }
        public string Titulo { get; set; }
        public DateTime FechaCreacion { get; set; }
        [Range(1, 5)]
        public int Calificacion { get; set; }
        public List<int> Personajes { get; set; }
        public int GeneroId { get; set; }
    }
}
