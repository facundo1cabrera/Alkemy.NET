using Alkemy.Controllers;
using Alkemy.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkemy.Tests.UnitTesting
{
    [TestClass]
    public class GenerosControllerTests: BaseTests
    {
        [TestMethod]
        public async Task ObtenerTodosLosGeneros()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            context.Generos.Add(new Genero() { Nombre = "Genero 1", Imagen = "example" });
            context.Generos.Add(new Genero() { Nombre = "Genero 2", Imagen = "Example" });
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreDB);

            var controller = new GenerosController(context2, mapper, null);
            var respuesta = await controller.GetGenders();

            var generos = respuesta.Value;
            Assert.AreEqual(2, generos.Count);
        }
        [TestMethod]
        public async Task ObtenerGeneroPorIdNoExistente()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            var controller = new GenerosController(context, mapper, null);
            var respuesta = await controller.GetById(1);

            var resultado = respuesta.Result as StatusCodeResult;
            Assert.AreEqual(404, resultado.StatusCode);
        }
    }
}
