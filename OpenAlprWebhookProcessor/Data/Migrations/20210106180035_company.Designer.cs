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
    [Migration("20210106180035_company")]
    partial class company
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.10");

            modelBuilder.Entity("OpenAlprWebhookProcessor.Data.Company", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("CompanyId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("OpenAlprWebhookProcessor.Data.PlateGroup", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("AlertDescription")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsAlert")
                        .HasColumnType("INTEGER");

                    b.Property<int>("OpenAlprCameraId")
                        .HasColumnType("INTEGER");

                    b.Property<double>("OpenAlprProcessingTimeMs")
                        .HasColumnType("REAL");

                    b.Property<double>("PlateConfidence")
                        .HasColumnType("REAL");

                    b.Property<string>("PlateJpeg")
                        .HasColumnType("TEXT");

                    b.Property<string>("PlateNumber")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("ReceivedOn")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("PlateGroups");
                });
#pragma warning restore 612, 618
        }
    }
}