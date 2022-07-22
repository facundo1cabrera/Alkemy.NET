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
    [Route("generos")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GenerosController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public GenerosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<GeneroDTO>>> GetGenders()
        {
            var gendersDB = await context.Generos.ToListAsync();
            return mapper.Map<List<GeneroDTO>>(gendersDB);
        }

        [HttpPost]
        public async Task<ActionResult> PostGenders([FromForm] GeneroCreacionDTO generoCreacionDTO)
        {
            var gender = mapper.Map<Genero>(generoCreacionDTO);
            context.Add(gender);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> EditGender([FromForm] GeneroCreacionDTO generoCreacionDTO, int id)
        {
            var exists = await context.Generos.AnyAsync(x => x.Id == id);
            if (!exists) { return BadRequest(); }

            var gender = mapper.Map<Genero>(generoCreacionDTO);
            gender.Id = id;

            context.Update(gender);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteGender(int id)
        {
            var exists = await context.Generos.AnyAsync(x => x.Id == id);
            if (!exists) { return BadRequest(); }


            context.Remove(new Genero() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
