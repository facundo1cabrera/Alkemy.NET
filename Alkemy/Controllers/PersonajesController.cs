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
    [Route("characters")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PersonajesController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly string contenedor = "personajes";
        public PersonajesController(ApplicationDbContext context, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }

        [HttpGet]
        public async Task<ActionResult<List<PersonajeDTO>>> GetCharacters([FromQuery] string name, [FromQuery] int age, [FromQuery] int movies)
        {
            var charactersDB = await context.Personajes.Include(x => x.PeliculasPersonajes)
                .ThenInclude(x => x.Pelicula).ToListAsync();

            if (name != null)
            {
                charactersDB = charactersDB.Where(x => x.Nombre.Contains(name)).ToList();
            }
            if (age != 0)
            {
                charactersDB = charactersDB.Where(x => x.Edad == age).ToList();
            }
            if (movies != 0)
            {
                var movie = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == movies);
                if (movie != null)
                {
                    var ids = await context.PeliculasPersonajes.Where(x => x.Id == movies).Select(x => x.PersonajeId).ToListAsync();
                    charactersDB = charactersDB.Where(x => ids.Contains(x.Id)).ToList();
                }
            }
            return mapper.Map<List<PersonajeDTO>>(charactersDB);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PersonajeDetalleDTO>> GetCharacterDetails(int id)
        {
            var character = await context.Personajes.Include(x => x.PeliculasPersonajes)
                .ThenInclude(x => x.Pelicula).FirstOrDefaultAsync(x => x.Id == id);
            if(character == null) { return NotFound(); }

            return mapper.Map<PersonajeDetalleDTO>(character);
        }

        [HttpPost]
        public async Task<ActionResult> PostCharacter([FromForm] PersonajeCreacionDTO personajeCreacionDTO)
        {
            var movies = await context.Peliculas.Where(x => personajeCreacionDTO.Peliculas.Contains(x.Id)).ToListAsync();
            if (movies.Count != personajeCreacionDTO.Peliculas.Count)
            {
                return BadRequest("Id de alguna de las peliculas es incorrecto");
            }

            var characterDB = mapper.Map<Personaje>(personajeCreacionDTO);

            if (personajeCreacionDTO.Imagen != null)
            {
                using(var memoryStream = new MemoryStream())
                {
                    await personajeCreacionDTO.Imagen.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(personajeCreacionDTO.Imagen.FileName);
                    characterDB.Imagen = await almacenadorArchivos.GuardarArchivo(contenido, extension, contenedor,
                        personajeCreacionDTO.Imagen.ContentType);

                }
            }

            context.Add(characterDB);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> EditCharacter(int id, [FromForm] PersonajeCreacionDTO personajeCreacionDTO)
        {
            var personajeDB = await context.Personajes.FirstOrDefaultAsync(x => x.Id == id);

            if(personajeDB == null) { return NotFound(); }

            personajeDB = mapper.Map(personajeCreacionDTO, personajeDB);

            if (personajeCreacionDTO.Imagen != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await personajeCreacionDTO.Imagen.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(personajeCreacionDTO.Imagen.FileName);
                    personajeDB.Imagen = await almacenadorArchivos.EditarArchivo(contenido, extension, contenedor,
                        personajeDB.Imagen,
                        personajeCreacionDTO.Imagen.ContentType);

                }
            }

            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteCharacter(int id)
        {
            var exists = await context.Personajes.AnyAsync(x => x.Id == id);
            if (!exists) { return BadRequest(); }

            context.Remove(new Personaje() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
