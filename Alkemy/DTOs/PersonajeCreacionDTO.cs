using Alkemy.Validaciones;

namespace Alkemy.DTOs
{
    public class PersonajeCreacionDTO
    {
        [PesoArchivoValidacion(PesoMaximoEnMB: 5)]
        [TipoArchivoValidacion(grupoTipoArchivo: GrupoTipoArchivo.Imagen)]
        public IFormFile Imagen { get; set; }
        public string Nombre { get; set; }
        public int Edad { get; set; }
        public int Peso { get; set; }
        public string Historia { get; set; }
        public List<int> Peliculas { get; set; }
    }
}
