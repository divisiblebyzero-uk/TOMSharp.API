﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TOMSharp_Loader.Service;

namespace TOMSharp_Loader.Migrations
{
    [DbContext(typeof(TogglContext))]
    [Migration("20200820123548_updating scenarioreports view")]
    partial class updatingscenarioreportsview
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TOMSharp_Loader.Model.FinanceProjectMapping", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Capitalised")
                        .HasColumnType("bit");

                    b.Property<string>("FinanceName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProjectName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("FinanceProjectMappings");
                });

            modelBuilder.Entity("TOMSharp_Loader.Model.Forecast", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("Created")
                        .HasColumnType("date");

                    b.Property<string>("FinanceYear")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("date");

                    b.HasKey("Id");

                    b.ToTable("Forecasts");
                });

            modelBuilder.Entity("TOMSharp_Loader.Model.ForecastEntry", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ForecastId")
                        .HasColumnType("int");

                    b.Property<decimal>("Hours")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Project")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Sprint")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ForecastId");

                    b.ToTable("ForecastEntry");
                });

            modelBuilder.Entity("TOMSharp_Loader.Model.Person", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("EmailAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Employer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("date");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ResourceType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("date");

                    b.Property<string>("Team")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasFilter("[Name] IS NOT NULL");

                    b.ToTable("People");
                });

            modelBuilder.Entity("TOMSharp_Loader.Model.Scenario", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("Created")
                        .HasColumnType("date");

                    b.Property<string>("FinanceYear")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("date");

                    b.HasKey("Id");

                    b.ToTable("Scenarios");
                });

            modelBuilder.Entity("TOMSharp_Loader.Model.ScenarioForecast", b =>
                {
                    b.Property<int?>("ScenarioId")
                        .HasColumnType("int");

                    b.Property<int?>("ForecastId")
                        .HasColumnType("int");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.HasKey("ScenarioId", "ForecastId");

                    b.HasIndex("ForecastId");

                    b.ToTable("ScenarioForecast");
                });

            modelBuilder.Entity("TOMSharp_Loader.Model.ScenarioProject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Project")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ScenarioId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ScenarioId");

                    b.ToTable("ScenarioProject");
                });

            modelBuilder.Entity("TOMSharp_Loader.Model.Sprint", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("date");

                    b.Property<string>("FinanceMonth")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FinanceYear")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("date");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasFilter("[Name] IS NOT NULL");

                    b.ToTable("Sprints");
                });

            modelBuilder.Entity("TOMSharp_Loader.Model.TimeEntry", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<decimal>("Billable")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Client")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Cur")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("Dur")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("End")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsBillable")
                        .HasColumnType("bit");

                    b.Property<long>("Pid")
                        .HasColumnType("bigint");

                    b.Property<string>("Project")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProjectColor")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProjectHexColor")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Start")
                        .HasColumnType("datetime2");

                    b.Property<string>("Task")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("Tid")
                        .HasColumnType("bigint");

                    b.Property<long>("Uid")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("Updated")
                        .HasColumnType("datetime2");

                    b.Property<bool>("UseStop")
                        .HasColumnType("bit");

                    b.Property<string>("User")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("TimeEntries");
                });

            modelBuilder.Entity("TOMSharp_Loader.Model.ForecastEntry", b =>
                {
                    b.HasOne("TOMSharp_Loader.Model.Forecast", null)
                        .WithMany("Entries")
                        .HasForeignKey("ForecastId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TOMSharp_Loader.Model.ScenarioForecast", b =>
                {
                    b.HasOne("TOMSharp_Loader.Model.Forecast", "Forecast")
                        .WithMany("ScenarioForecasts")
                        .HasForeignKey("ForecastId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TOMSharp_Loader.Model.Scenario", "Scenario")
                        .WithMany("ScenarioForecasts")
                        .HasForeignKey("ScenarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TOMSharp_Loader.Model.ScenarioProject", b =>
                {
                    b.HasOne("TOMSharp_Loader.Model.Scenario", "Scenario")
                        .WithMany("ScenarioProjects")
                        .HasForeignKey("ScenarioId");
                });
#pragma warning restore 612, 618
        }
    }
}
