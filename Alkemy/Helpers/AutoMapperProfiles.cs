using Alkemy.DTOs;
using Alkemy.Entidades;
using AutoMapper;

namespace Alkemy.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Pelicula, PeliculaDTO>()
                .ForMember(dest => dest.Personajes, opt => opt.MapFrom(MapPersonajesPelicula));
            CreateMap<PeliculaCreacionDTO, Pelicula>()
                .ForMember(dest => dest.PeliculasPersonajes, opt => opt.MapFrom(MapPersonajesPeliculaCreacion))
                .ForMember(dest => dest.Imagen, opt => opt.Ignore());
            CreateMap<Pelicula, PeliculaDetalleDTO>()
                .ForMember(dest => dest.Personajes, opt => opt.MapFrom(MapPersonajesToPeliculaDetalle));

            CreateMap<Personaje, PersonajeDetalleDTO>()
                .ForMember(dest => dest.Peliculas, opt => opt.MapFrom(MapPeliculasToPersonajes));
            CreateMap<Personaje, PersonajeDTO>();

            CreateMap<PersonajeCreacionDTO, Personaje>()
                .ForMember(dest => dest.Imagen, opt => opt.Ignore());

            CreateMap<Genero, GeneroDTO>();
            CreateMap<GeneroCreacionDTO, Genero>()
                .ForMember(dest => dest.Imagen, opt => opt.Ignore());
        }

        private List<PersonajeDTO> MapPersonajesToPeliculaDetalle(Pelicula pelicula, PeliculaDetalleDTO peliculaDetalleDTO)
        {
            var result = new List<PersonajeDTO>();

            if (pelicula.PeliculasPersonajes != null)
            {
                foreach (var peliculaPersonaje in pelicula.PeliculasPersonajes)
                {
                    result.Add(new PersonajeDTO()
                    {
                        Nombre = peliculaPersonaje.Personaje.Nombre,
                        Imagen = peliculaPersonaje.Personaje.Imagen
                    });

                }
            }
            return result;
        }

        private List<PersonajeDTO> MapPersonajesPelicula(Pelicula pelicula, PeliculaDTO peliculaDTO)
        {
            var result = new List<PersonajeDTO>();

            if (pelicula.PeliculasPersonajes != null)
            {
                foreach (var peliculaPersonaje in pelicula.PeliculasPersonajes)
                {
                    result.Add(new PersonajeDTO()
                    {
                        Nombre = peliculaPersonaje.Personaje.Nombre,
                        Imagen = peliculaPersonaje.Personaje.Imagen
                    });

                }
            }
            return result;
        }

        private List<PeliculasPersonajes> MapPersonajesPeliculaCreacion(PeliculaCreacionDTO peliculaCreacionDTO, Pelicula pelicula)
        {
            var result = new List<PeliculasPersonajes>();

            if(peliculaCreacionDTO.Personajes == null) { return result; }

            foreach (var personaje in peliculaCreacionDTO.Personajes)
            {
                result.Add(new PeliculasPersonajes() { PersonajeId = personaje });
            }

            return result;
        }

        private List<PeliculaDTO> MapPeliculasToPersonajes(Personaje personaje, PersonajeDetalleDTO personajeDTO)
        {
            var result = new List<PeliculaDTO>();

            if (personaje.PeliculasPersonajes != null)
            {
                foreach (var peliculasPersonajes in personaje.PeliculasPersonajes)
                {
                    result.Add(new PeliculaDTO()
                    {
                        Id = peliculasPersonajes.Pelicula.Id,
                        Calificacion = peliculasPersonajes.Pelicula.Calificacion,
                        GeneroId = peliculasPersonajes.Pelicula.GeneroId,
                        Imagen = peliculasPersonajes.Pelicula.Imagen,
                        Titulo = peliculasPersonajes.Pelicula.Titulo,
                        FechaCreacion = peliculasPersonajes.Pelicula.FechaCreacion
                    });

                }
            }

            return result;
        }

        private List<PeliculasPersonajes> MapPeliculaPersonajesCreacion(PersonajeCreacionDTO personajeCreacionDTO, Personaje personaje)
        {
            var result = new List<PeliculasPersonajes>();

            if (personajeCreacionDTO.Peliculas == null) { return result; }

            foreach (var pelicula in personajeCreacionDTO.Peliculas)
            {
                result.Add(new PeliculasPersonajes() { PeliculaId = pelicula });
            }

            return result;
        }
    }
}
