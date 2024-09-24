﻿// <auto-generated />
using System;
using EXE201_Lockey.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EXE201_Lockey.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20240924095830_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("EXE201_Lockey.Models.Order", b =>
                {
                    b.Property<int>("OrderID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrderID"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("ProductID")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("OrderID");

                    b.HasIndex("ProductID");

                    b.ToTable("Order", (string)null);
                });

            modelBuilder.Entity("EXE201_Lockey.Models.Payment", b =>
                {
                    b.Property<int>("PaymentID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PaymentID"));

                    b.Property<int>("OrderID")
                        .HasColumnType("int");

                    b.Property<DateTime>("PaymentDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("PaymentMethod")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PaymentID");

                    b.HasIndex("OrderID")
                        .IsUnique();

                    b.ToTable("Payment", (string)null);
                });

            modelBuilder.Entity("EXE201_Lockey.Models.Product", b =>
                {
                    b.Property<int>("ProductID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductID"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("File2DLink")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Preview3D")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("TemplateID")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("ProductID");

                    b.HasIndex("TemplateID");

                    b.HasIndex("UserID");

                    b.ToTable("Product", (string)null);
                });

            modelBuilder.Entity("EXE201_Lockey.Models.Template", b =>
                {
                    b.Property<int>("TemplateID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TemplateID"));

                    b.Property<double>("FileTemplate")
                        .HasColumnType("float");

                    b.Property<string>("TemplateImage")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TemplateName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ThemeID")
                        .HasColumnType("int");

                    b.HasKey("TemplateID");

                    b.HasIndex("ThemeID");

                    b.ToTable("Template", (string)null);
                });

            modelBuilder.Entity("EXE201_Lockey.Models.Theme", b =>
                {
                    b.Property<int>("ThemeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ThemeID"));

                    b.Property<string>("ThemeName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ThemeID");

                    b.ToTable("Theme", (string)null);
                });

            modelBuilder.Entity("EXE201_Lockey.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("User", (string)null);
                });

            modelBuilder.Entity("EXE201_Lockey.Models.Order", b =>
                {
                    b.HasOne("EXE201_Lockey.Models.Product", "Product")
                        .WithMany("Orders")
                        .HasForeignKey("ProductID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("EXE201_Lockey.Models.Payment", b =>
                {
                    b.HasOne("EXE201_Lockey.Models.Order", "Order")
                        .WithOne("Payment")
                        .HasForeignKey("EXE201_Lockey.Models.Payment", "OrderID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("EXE201_Lockey.Models.Product", b =>
                {
                    b.HasOne("EXE201_Lockey.Models.Template", "Template")
                        .WithMany("Products")
                        .HasForeignKey("TemplateID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("EXE201_Lockey.Models.User", "User")
                        .WithMany("Products")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Template");

                    b.Navigation("User");
                });

            modelBuilder.Entity("EXE201_Lockey.Models.Template", b =>
                {
                    b.HasOne("EXE201_Lockey.Models.Theme", "Theme")
                        .WithMany("Templates")
                        .HasForeignKey("ThemeID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Theme");
                });

            modelBuilder.Entity("EXE201_Lockey.Models.Order", b =>
                {
                    b.Navigation("Payment")
                        .IsRequired();
                });

            modelBuilder.Entity("EXE201_Lockey.Models.Product", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("EXE201_Lockey.Models.Template", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("EXE201_Lockey.Models.Theme", b =>
                {
                    b.Navigation("Templates");
                });

            modelBuilder.Entity("EXE201_Lockey.Models.User", b =>
                {
                    b.Navigation("Products");
                });
#pragma warning restore 612, 618
        }
    }
}
