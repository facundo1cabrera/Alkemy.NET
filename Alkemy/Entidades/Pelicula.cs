using System.ComponentModel.DataAnnotations;

namespace Alkemy.Entidades
{
    public class Pelicula
    {
        public int Id { get; set; }
        public string Imagen { get; set; }
        public string Titulo { get; set; }
        public DateTime FechaCreacion { get; set; }
        [Range(1,5)]
        public int Calificacion { get; set; }
        public List<PeliculasPersonajes> PeliculasPersonajes { get; set; }
        public int GeneroId { get; set; }
        public Genero Genero { get; set; }
    }
}
