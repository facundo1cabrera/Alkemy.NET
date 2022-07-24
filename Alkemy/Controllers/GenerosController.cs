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
    [Route("generos")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GenerosController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly string contenedor = "generos";

        public GenerosController(ApplicationDbContext context, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
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

            if (generoCreacionDTO.Imagen != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await generoCreacionDTO.Imagen.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(generoCreacionDTO.Imagen.FileName);
                    gender.Imagen = await almacenadorArchivos.GuardarArchivo(contenido, extension, contenedor,
                        generoCreacionDTO.Imagen.ContentType);

                }
            }

            context.Add(gender);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> EditGender([FromForm] GeneroCreacionDTO generoCreacionDTO, int id)
        {
            var genero = await context.Generos.FirstOrDefaultAsync(x => x.Id == id);

            if (genero == null) { return NotFound(); }

            genero = mapper.Map(generoCreacionDTO, genero);

            if (generoCreacionDTO.Imagen != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await generoCreacionDTO.Imagen.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(generoCreacionDTO.Imagen.FileName);
                    genero.Imagen = await almacenadorArchivos.EditarArchivo(contenido, extension, contenedor,
                        genero.Imagen,
                        generoCreacionDTO.Imagen.ContentType);

                }
            }
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
