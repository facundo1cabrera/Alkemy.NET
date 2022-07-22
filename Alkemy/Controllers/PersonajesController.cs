using Alkemy.DTOs;
using Alkemy.Entidades;
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

        public PersonajesController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<PersonajeDTO>>> GetCharacters()
        {
            var characters = await context.Personajes.Include(x => x.PeliculasPersonajes)
                .ThenInclude(x => x.Pelicula).ToListAsync();
            return mapper.Map<List<PersonajeDTO>>(characters);
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
            context.Add(characterDB);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> EditCharacter(int id, [FromForm] PersonajeCreacionDTO personajeCreacionDTO)
        {
            var exists = await context.Personajes.AnyAsync(x => x.Id == id);
            if(!exists) { return BadRequest(); }

            var movies = await context.Peliculas.Where(x => personajeCreacionDTO.Peliculas.Contains(x.Id)).ToListAsync();
            if (movies.Count != personajeCreacionDTO.Peliculas.Count)
            {
                return BadRequest("Id de alguna de las peliculas es incorrecto");
            }

            var characterDB = mapper.Map<Personaje>(personajeCreacionDTO);
            characterDB.Id = id;

            context.Update(characterDB);
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
