﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OpenAlprWebhookProcessor.Data;

namespace OpenAlprWebhookProcessor.Migrations
{
    [DbContext(typeof(ProcessorContext))]
    [Migration("20210131155232_daynighturl")]
    partial class daynighturl
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("OpenAlprWebhookProcessor.Data.Agent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("EndpointUrl")
                        .HasColumnType("TEXT");

                    b.Property<string>("Hostname")
                        .HasColumnType("TEXT");

                    b.Property<string>("OpenAlprWebServerApiKey")
                        .HasColumnType("TEXT");

                    b.Property<string>("OpenAlprWebServerUrl")
                        .HasColumnType("TEXT");

                    b.Property<string>("Uid")
                        .HasColumnType("TEXT");

                    b.Property<string>("Version")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Agents");
                });

            modelBuilder.Entity("OpenAlprWebhookProcessor.Data.Alert", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsStrictMatch")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PlateNumber")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Alerts");
                });

            modelBuilder.Entity("OpenAlprWebhookProcessor.Data.Camera", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("CameraPassword")
                        .HasColumnType("TEXT");

                    b.Property<string>("CameraUsername")
                        .HasColumnType("TEXT");

                    b.Property<string>("LatestProcessedPlateUuid")
                        .HasColumnType("TEXT");

                    b.Property<double>("Latitude")
                        .HasColumnType("REAL");

                    b.Property<double>("Longitude")
                        .HasColumnType("REAL");

                    b.Property<int>("Manufacturer")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ModelNumber")
                        .HasColumnType("TEXT");

                    b.Property<long>("OpenAlprCameraId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("OpenAlprName")
                        .HasColumnType("TEXT");

                    b.Property<int>("PlatesSeen")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UpdateDayNightModeUrl")
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdateOverlayTextUrl")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Cameras");
                });

            modelBuilder.Entity("OpenAlprWebhookProcessor.Data.Ignore", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsStrictMatch")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PlateNumber")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Ignores");
                });

            modelBuilder.Entity("OpenAlprWebhookProcessor.Data.PlateGroup", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("AlertDescription")
                        .HasColumnType("TEXT");

                    b.Property<double>("Confidence")
                        .HasColumnType("REAL");

                    b.Property<double>("Direction")
                        .HasColumnType("REAL");

                    b.Property<bool>("IsAlert")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Jpeg")
                        .HasColumnType("TEXT");

                    b.Property<string>("Number")
                        .HasColumnType("TEXT");

                    b.Property<int>("OpenAlprCameraId")
                        .HasColumnType("INTEGER");

                    b.Property<double>("OpenAlprProcessingTimeMs")
                        .HasColumnType("REAL");

                    b.Property<string>("OpenAlprUuid")
                        .HasColumnType("TEXT");

                    b.Property<string>("PlateCoordinates")
                        .HasColumnType("TEXT");

                    b.Property<long>("ReceivedOnEpoch")
                        .HasColumnType("INTEGER");

                    b.Property<string>("VehicleDescription")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("PlateGroups");
                });
#pragma warning restore 612, 618
        }
    }
}
