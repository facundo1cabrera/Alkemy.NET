using Alkemy.Validaciones;

namespace Alkemy.DTOs
{
    public class GeneroCreacionDTO
    {
        public string Nombre { get; set; }
        [PesoArchivoValidacion(PesoMaximoEnMB: 5)]
        [TipoArchivoValidacion(grupoTipoArchivo: GrupoTipoArchivo.Imagen)]
        public IFormFile Imagen { get; set; }
    }
}
