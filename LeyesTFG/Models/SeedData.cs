using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using LeyesTFG.Data;

namespace LeyesTFG.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new LeyesTFGContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<LeyesTFGContext>>()))
            {
                if (!context.Ley.Any())
                {
                    context.Ley.AddRange(
                    new Ley
                    {
                        Titulo = "Ley 4/2017, de 13 de julio, del Suelo y de los Espacios Naturales Protegidos de Canarias.",
                        FechaPublicacion = DateTime.Parse("2017-09-01"),
                        Departamento = "Comunidad Autónoma de Canarias"
                    }
                    );
                    context.SaveChanges();
                }

                if (!context.Articulo.Any())
                {
                    context.Articulo.AddRange(
                    new Articulo
                    {
                        Titulo = "Articulo 1",
                        Texto = "La presente ley tiene por objeto regular en el ámbito de la Comunidad Autónoma de Canarias: a) El régimen jurídico general de los recursos naturales, en particular del suelo, la ordenación del territorio y la ordenación urbanística. b) La coordinación de las políticas públicas relativas a la planificación y gestión del territorio y a la protección del medioambiente. c) La intervención en las actividades públicas y privadas con incidencia relevante sobre el territorio y los recursos naturales. d) La protección de la legalidad urbanística mediante el ejercicio, en su caso, de la potestad sancionadora.",
                        LeyId = 01,
                        TextoAnterior = "Este articulo no ha sido modificado nunca"
                    },

                    new Articulo
                    {
                        Titulo = "Articulo 2",
                        Texto = "1. A los efectos de esta ley, los conceptos utilizados tienen el significado y el alcance determinado en los apartados siguientes, siempre que la legislación sectorial aplicable no establezca uno más preciso",
                        LeyId = 01,
                        TextoAnterior = "1. A los efectos de esta ley, los conceptos utilizados tienen el MOD y el alcance, siempre que la legislación sectorial aplicable no establezca uno más preciso"
                    },

                    new Articulo
                    {
                        Titulo = "Articulo 3",
                        Texto = "1. Las intervenciones, tanto públicas como privadas, que se lleven a cabo en el archipiélago canario preservarán y cuidarán sus valores naturales y la calidad de sus recursos, de modo que permitan su uso y disfrute responsable por las generaciones presentes sin mermar la capacidad de las generaciones futuras",
                        LeyId = 01,
                        TextoAnterior = "Este articulo no ha sido modificado nunca"
                    },

                    new Articulo
                    {
                        Titulo = "Articulo 4",
                        Texto = "1. La ordenación de los recursos naturales se llevará a cabo conforme al interés general, la igualdad de género y la solidaridad intergeneracional.",
                        LeyId = 01,
                        TextoAnterior = "Este articulo no ha sido modificado nunca"
                    }
                    );
                    context.SaveChanges();
                }

                if(!context.Modificacion.Any())
                {
                    context.Modificacion.AddRange(
                    new Modificacion
                    {
                        Titulo = "Modificacion Articulo 1",
                        Texto = "La presente ley tiene por objeto regular en el ámbito de la Comunidad Autónoma de Canarias: a) MODIFICACION b) La coordinación de las políticas públicas relativas a la planificación y gestión del territorio y a la protección del medioambiente. c) La intervención en las actividades públicas y privadas con incidencia relevante sobre el territorio y los recursos naturales. d) La protección de la legalidad urbanística mediante el ejercicio, en su caso, de la potestad sancionadora.",
                        ArticuloId = 01,
                        Aceptado = false
                    },

                    new Modificacion
                    {
                        Titulo = "Modificacion Articulo 3",
                        Texto = "1. Las intervenciones, tanto públicas como privadas, MODIFICACION y cuidarán sus valores naturales y la calidad de sus recursos, de modo que permitan su uso y disfrute responsable por las generaciones presentes sin mermar la capacidad de las generaciones futuras",
                        ArticuloId = 03,
                        Aceptado = false
                    }
                    );
                    context.SaveChanges();
                }
            }
        }
    }
}
