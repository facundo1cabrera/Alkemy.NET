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
    [Route("movies")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PeliculasController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public PeliculasController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<PeliculaDTO>>> GetMovies()
        {
            var movies = await context.Peliculas.Include(x => x.PeliculasPersonajes)
                .ThenInclude(x => x.Personaje).ToListAsync();
            var moviesDTO = mapper.Map<List<PeliculaDTO>>(movies);
            return Ok(moviesDTO);
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
            context.Add(movie);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> EditMovie([FromForm] PeliculaCreacionDTO peliculaCreacionDTO, int id)
        {
            var exists = await context.Peliculas.AnyAsync(x => x.Id == id);
            if (!exists) { return BadRequest(); }

            var characters = await context.Personajes.Where(x => peliculaCreacionDTO.Personajes.Contains(x.Id)).ToListAsync();
            if (characters.Count != peliculaCreacionDTO.Personajes.Count)
            {
                return BadRequest("Id de alguno de los personajes incorrecto");
            }

            var gender = await context.Generos.FirstOrDefaultAsync(x => x.Id == peliculaCreacionDTO.GeneroId);
            if (gender == null) { return BadRequest("Genero no existente"); }

            var movie = mapper.Map<Pelicula>(peliculaCreacionDTO);
            movie.Id = id;
            
            context.Update(movie);
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
