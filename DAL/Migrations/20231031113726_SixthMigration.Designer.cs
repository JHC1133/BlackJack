﻿// <auto-generated />
using System;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DAL.Migrations
{
    [DbContext(typeof(GameDbContext))]
    [Migration("20231031113726_SixthMigration")]
    partial class SixthMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("EL.DealerStatistics", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Blackjacks")
                        .HasColumnType("int");

                    b.Property<int>("Busts")
                        .HasColumnType("int");

                    b.Property<int>("Losses")
                        .HasColumnType("int");

                    b.Property<int>("Ties")
                        .HasColumnType("int");

                    b.Property<int>("Wins")
                        .HasColumnType("int");

                    b.HasKey("Name");

                    b.ToTable("DealerStatistics");
                });

            modelBuilder.Entity("EL.Game", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<DateTime>("DatePlayed")
                        .HasColumnType("datetime2");

                    b.Property<string>("DealerName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("DealerStatisticsName")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ID");

                    b.HasIndex("DealerName");

                    b.HasIndex("DealerStatisticsName");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("EL.GamePlayerStatisticsIntermediary", b =>
                {
                    b.Property<int>("GameID")
                        .HasColumnType("int");

                    b.Property<string>("PlayerName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("GamePlayerStatisticsID")
                        .HasColumnType("int");

                    b.HasKey("GameID", "PlayerName");

                    b.HasIndex("PlayerName");

                    b.ToTable("GamePlayerStatisticsIntermediary");
                });

            modelBuilder.Entity("EL.PlayerStatistics", b =>
                {
                    b.Property<string>("PlayerName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Blackjacks")
                        .HasColumnType("int");

                    b.Property<int>("Busts")
                        .HasColumnType("int");

                    b.Property<int>("Losses")
                        .HasColumnType("int");

                    b.Property<int>("Ties")
                        .HasColumnType("int");

                    b.Property<int>("Wins")
                        .HasColumnType("int");

                    b.HasKey("PlayerName");

                    b.ToTable("PlayerStatistics");
                });

            modelBuilder.Entity("EL.Game", b =>
                {
                    b.HasOne("EL.DealerStatistics", "DealerStatistics")
                        .WithMany()
                        .HasForeignKey("DealerName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EL.DealerStatistics", null)
                        .WithMany("Games")
                        .HasForeignKey("DealerStatisticsName");

                    b.Navigation("DealerStatistics");
                });

            modelBuilder.Entity("EL.GamePlayerStatisticsIntermediary", b =>
                {
                    b.HasOne("EL.Game", "Game")
                        .WithMany("GamePlayerStatisticsIntermediary")
                        .HasForeignKey("GameID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EL.PlayerStatistics", "PlayerStatistics")
                        .WithMany("GamesPlayerIntermediary")
                        .HasForeignKey("PlayerName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");

                    b.Navigation("PlayerStatistics");
                });

            modelBuilder.Entity("EL.DealerStatistics", b =>
                {
                    b.Navigation("Games");
                });

            modelBuilder.Entity("EL.Game", b =>
                {
                    b.Navigation("GamePlayerStatisticsIntermediary");
                });

            modelBuilder.Entity("EL.PlayerStatistics", b =>
                {
                    b.Navigation("GamesPlayerIntermediary");
                });
#pragma warning restore 612, 618
        }
    }
}
