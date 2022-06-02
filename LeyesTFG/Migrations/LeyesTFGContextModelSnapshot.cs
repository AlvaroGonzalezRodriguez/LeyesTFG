﻿// <auto-generated />
using System;
using LeyesTFG.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace LeyesTFG.Migrations
{
    [DbContext(typeof(LeyesTFGContext))]
    partial class LeyesTFGContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("LeyesTFG.Models.Articulo", b =>
                {
                    b.Property<int>("ArticuloId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ArticuloId"), 1L, 1);

                    b.Property<int>("LeyId")
                        .HasColumnType("int");

                    b.Property<string>("Texto")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TextoAnterior")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Titulo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ArticuloId");

                    b.HasIndex("LeyId");

                    b.ToTable("Articulo");
                });

            modelBuilder.Entity("LeyesTFG.Models.Ley", b =>
                {
                    b.Property<int>("LeyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LeyId"), 1L, 1);

                    b.Property<string>("Departamento")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("FechaPublicacion")
                        .HasColumnType("datetime2");

                    b.Property<string>("Titulo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("LeyId");

                    b.ToTable("Ley");
                });

            modelBuilder.Entity("LeyesTFG.Models.Modificacion", b =>
                {
                    b.Property<int>("ModificacionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ModificacionId"), 1L, 1);

                    b.Property<bool>("Aceptado")
                        .HasColumnType("bit");

                    b.Property<int>("ArticuloId")
                        .HasColumnType("int");

                    b.Property<bool>("PendienteEva")
                        .HasColumnType("bit");

                    b.Property<string>("Texto")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Titulo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ModificacionId");

                    b.HasIndex("ArticuloId");

                    b.ToTable("Modificacion");
                });

            modelBuilder.Entity("LeyesTFG.Models.Articulo", b =>
                {
                    b.HasOne("LeyesTFG.Models.Ley", "Ley")
                        .WithMany("Articulos")
                        .HasForeignKey("LeyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ley");
                });

            modelBuilder.Entity("LeyesTFG.Models.Modificacion", b =>
                {
                    b.HasOne("LeyesTFG.Models.Articulo", "Articulo")
                        .WithMany("Modificaciones")
                        .HasForeignKey("ArticuloId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Articulo");
                });

            modelBuilder.Entity("LeyesTFG.Models.Articulo", b =>
                {
                    b.Navigation("Modificaciones");
                });

            modelBuilder.Entity("LeyesTFG.Models.Ley", b =>
                {
                    b.Navigation("Articulos");
                });
#pragma warning restore 612, 618
        }
    }
}
