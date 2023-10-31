﻿using EL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    internal class GameDbContext : DbContext
    {

        public DbSet<PlayerStatistics> PlayerStatistics { get; set; }
        public DbSet<DealerStatistics> DealerStatistics { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<GameStatistics> GameStatistics { get; set;}

        //public DbSet<Player> Players { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=GameDB");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Game>().Property(g => g.ID).ValueGeneratedOnAdd();

            modelBuilder.Entity<GameStatistics>()
        .HasKey(gs => new { gs.GameID, gs.PlayerName, gs.DealerName });

            modelBuilder.Entity<GameStatistics>()
                .HasOne(gs => gs.Game)
                .WithMany(g => g.GameStatistics)
                .HasForeignKey(gs => gs.GameID);

            modelBuilder.Entity<GameStatistics>()
                .HasOne(gs => gs.PlayerStatistics)
                .WithMany(p => p.GameStatistics)
                .HasForeignKey(gs => gs.PlayerName);

            modelBuilder.Entity<GameStatistics>()
                .HasOne(gs => gs.DealerStatistics)
                .WithMany(d => d.GameStatistics)
                .HasForeignKey(gs => gs.DealerName);

            base.OnModelCreating(modelBuilder);
        }

    }
}
