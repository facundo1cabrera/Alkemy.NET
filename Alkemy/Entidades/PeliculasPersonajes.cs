namespace Alkemy.Entidades
{
    public class PeliculasPersonajes
    {
        public int Id { get; set; }
        public int PersonajeId { get; set; }
        public int PeliculaId { get; set; }
        public Personaje Personaje { get; set; }
        public Pelicula Pelicula { get; set; }
    }
}
