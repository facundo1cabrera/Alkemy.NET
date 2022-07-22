namespace Alkemy.DTOs
{
    public class PeliculaDTO
    {
        public int Id { get; set; }
        public string Imagen { get; set; }
        public string Titulo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int Calificacion { get; set; }
        public List<PersonajeDTO> Personajes { get; set; }
        public int GeneroId { get; set; }
    }
}
