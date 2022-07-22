namespace Alkemy.Entidades
{
    public class Genero
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Imagen { get; set; }
        public List<Pelicula> Peliculas { get; set; }
    }
}
