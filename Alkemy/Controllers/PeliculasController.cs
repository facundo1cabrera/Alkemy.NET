using Alkemy.DTOs;
using Alkemy.Entidades;
using Alkemy.Servicios;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Alkemy.Controllers
{
    [ApiController]
    [Route("movies")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PeliculasController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly string contenedor = "peliculas";

        public PeliculasController(ApplicationDbContext context, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }

        [HttpGet]
        public async Task<ActionResult<List<PeliculaDTO>>> GetMovies([FromQuery] string name, [FromQuery] int genre)
        {
            var moviesDB = await context.Peliculas.Include(x => x.PeliculasPersonajes)
                .ThenInclude(x => x.Personaje).ToListAsync();

            if (name != null)
            {
                moviesDB = moviesDB.Where(x => x.Titulo.Contains(name)).ToList();
            }
            if (genre != 0)
            {
                moviesDB = moviesDB.Where(x => x.GeneroId == genre).ToList();
            }
            //Not filtering desc or asc because i dont undeerstand on witch field i am suposed to filter

            return mapper.Map<List<PeliculaDTO>>(moviesDB);
        }

        [HttpPost]
        public async Task<ActionResult> CreateMovie([FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
        {
            var characters = await context.Personajes.Where(x => peliculaCreacionDTO.Personajes.Contains(x.Id)).ToListAsync();
            if(characters.Count != peliculaCreacionDTO.Personajes.Count)
            {
                return BadRequest("Id de alguno de los personajes incorrecto");
            }

            var gender = await context.Generos.FirstOrDefaultAsync(x => x.Id == peliculaCreacionDTO.GeneroId);
            if (gender == null) { return BadRequest("Genero no existente"); }

            var movie = mapper.Map<Pelicula>(peliculaCreacionDTO);

            if (peliculaCreacionDTO.Imagen != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await peliculaCreacionDTO.Imagen.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(peliculaCreacionDTO.Imagen.FileName);
                    movie.Imagen = await almacenadorArchivos.GuardarArchivo(contenido, extension, contenedor,
                        peliculaCreacionDTO.Imagen.ContentType);

                }
            }

            context.Add(movie);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> EditMovie([FromForm] PeliculaCreacionDTO peliculaCreacionDTO, int id)
        {
            var peliculaDB = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);

            if (peliculaDB == null) { return NotFound(); }

            peliculaDB = mapper.Map(peliculaCreacionDTO, peliculaDB);

            if (peliculaCreacionDTO.Imagen != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await peliculaCreacionDTO.Imagen.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(peliculaCreacionDTO.Imagen.FileName);
                    peliculaDB.Imagen = await almacenadorArchivos.EditarArchivo(contenido, extension, contenedor,
                        peliculaDB.Imagen,
                        peliculaCreacionDTO.Imagen.ContentType);

                }
            }
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteMovie(int id)
        {
            var exists = await context.Peliculas.AnyAsync(x => x.Id == id);
            if (!exists) { return BadRequest(); }


            context.Remove(new Pelicula() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
