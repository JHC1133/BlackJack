﻿using EL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Handler
    {
        #region Game

        
        /// <summary>
        /// Creates a Game in the Game table
        /// </summary>
        /// <param name="playerNames"></param>
        /// <param name="dealerName"></param>
        public void CreateNewGame(List<string> playerNames, string dealerName)
        {
            using (var context = new GameDbContext())
            {
                var game = new Game
                {
                    DatePlayed = DateTime.Now,
                    DealerName = dealerName
                };

                // Check if the dealer name already exists
                var existingDealer = context.DealerStatistics.FirstOrDefault(d => d.Name == dealerName);
                if (existingDealer != null)
                {
                    game.DealerStatistics = existingDealer;
                }
                else
                {
                    game.DealerStatistics = new DealerStatistics
                    {
                        Name = dealerName
                    };
                }

                foreach (var playerName in playerNames)
                {
                    if (game.GamePlayerStatisticsIntermediary == null)
                    {
                        game.GamePlayerStatisticsIntermediary = new List<GamePlayerStatisticsIntermediary>();
                    }

                    game.GamePlayerStatisticsIntermediary.Add(new GamePlayerStatisticsIntermediary
                    {
                        PlayerName = playerName
                    });
                }

                context.Games.Add(game);
                context.SaveChanges();
            }
        }


        /// <summary>
        /// Updates the current game in the Game table. Should be called just before the shutting down of the Blackjack application
        /// </summary>
        public void UpdateGame()
        {
            using (var context = new GameDbContext())
            {             

                // Gets the latest (current) game added
                // .ThenInclude used for getting to PlayerStatistics via the GamePlayerStatIntermediary
                var game = context.Games
                    .Include(g => g.GamePlayerStatisticsIntermediary)
                    .ThenInclude(gp => gp.PlayerStatistics)
                    .Include(g => g.DealerStatistics)
                    .OrderByDescending(g => g.DatePlayed)
                    .FirstOrDefault();

                if (game == null)
                {
                    Debug.WriteLine("There was no game in the table");
                    return;
                }

                foreach (var gamePlayerStats in game.GamePlayerStatisticsIntermediary)
                {
                    var playerName = gamePlayerStats.PlayerName;
                    var updatedPlayerStats = GetPlayerStatistics(playerName);

                    if (updatedPlayerStats != null)
                    {
                        // Update player statistics
                        gamePlayerStats.PlayerStatistics.Wins = updatedPlayerStats.Wins;
                        gamePlayerStats.PlayerStatistics.Blackjacks = updatedPlayerStats.Blackjacks;
                        gamePlayerStats.PlayerStatistics.Busts = updatedPlayerStats.Busts;
                        gamePlayerStats.PlayerStatistics.Losses = updatedPlayerStats.Losses;
                        gamePlayerStats.PlayerStatistics.Ties = updatedPlayerStats.Ties;
                    }
                }

                var dealerStats = game.DealerStatistics;

                if (dealerStats != null)
                {
                    var updatedDealerStats = GetDealerStatistics();

                    if (updatedDealerStats != null)
                    {
                        dealerStats.Wins = updatedDealerStats.Wins;
                        dealerStats.Blackjacks = updatedDealerStats.Blackjacks;
                        dealerStats.Busts = updatedDealerStats.Busts;
                        dealerStats.Losses = updatedDealerStats.Losses;
                        dealerStats.Ties = updatedDealerStats.Ties;
                    }
                }

                context.SaveChanges();
            }

            PrintGameTableContent();
        }

        /// <summary>
        /// Returns Games from Game table to list, for use with the DataGridView in the UI
        /// </summary>
        /// <returns></returns>
        public List<Game> GetGames()
        {
            using (var context = new GameDbContext())
            {

                return context.Games
                    .Include(g => g.GamePlayerStatisticsIntermediary)
                    .ThenInclude(gp => gp.PlayerStatistics)
                    .Include(g => g.DealerStatistics)
                    .ToList();
            }
        }

        #endregion

        #region Player

        /// <summary>
        /// Returns playerStatistics from the database as a list. Used for DataGridView in UI
        /// </summary>
        /// <returns></returns>
        public List<PlayerStatistics> GetPlayers()
        {
            using (var context = new GameDbContext())
            {
                return context.PlayerStatistics.Include(g => g.GamesPlayerIntermediary).ToList();
            }
        }

        /// <summary>
        /// Returns PlayerStatistics from the database for the playerName input
        /// </summary>
        /// <param name="playerName"></param>
        /// <returns></returns>
        public PlayerStatistics GetPlayerStatistics(string playerName)
        {
            using (var context = new GameDbContext())
            {
                PlayerStatistics playerStats = context.PlayerStatistics.FirstOrDefault(p => p.PlayerName == playerName);

                return playerStats;
            }
        }  

        /// <summary>
        /// Updates playerstats if exisiting, if not adds them to a new row in the table
        /// </summary>
        /// <param name="playerStats"></param>
        public void UpdatePlayerStatistics(PlayerStatistics playerStats)
        {

            using (var context = new GameDbContext())
            {
                // Retrieve the existing PlayerStatistics record from the database
                var existingPlayerStats = context.PlayerStatistics.FirstOrDefault(p => p.PlayerName == playerStats.PlayerName);

                if (existingPlayerStats != null)
                {
                    existingPlayerStats.Wins += playerStats.Wins;
                    existingPlayerStats.Busts += playerStats.Busts;
                    existingPlayerStats.Ties += playerStats.Ties;
                    existingPlayerStats.Losses += playerStats.Losses;
                    existingPlayerStats.Blackjacks += playerStats.Blackjacks;

                    context.SaveChanges();
                }
                else
                {
                    context.PlayerStatistics.Add(playerStats);
                }

                    context.SaveChanges();
            }

        }

        #endregion

        #region Dealer

        /// <summary>
        /// Returns the DealerStatistics as a list. Used for the DataGridView in the UI
        /// </summary>
        /// <returns></returns>
        public List<DealerStatistics> GetDealer()
        {
            using (var context = new GameDbContext())
            {
                return context.DealerStatistics.Include(g => g.Games).ToList();
            }
        }

        /// <summary>
        /// Returns the DealerStatistics
        /// </summary>
        /// <returns></returns>
        public DealerStatistics GetDealerStatistics()
        {
            using (var context = new GameDbContext()) 
            {
              
                DealerStatistics dealerStats = context.DealerStatistics.FirstOrDefault();

                return dealerStats;
            }
        }

        /// <summary>
        /// Updates dealerStats if exisiting, if not adds them to a new row in the table
        /// </summary>
        /// <param name="dealerStats"></param>
        public void UpdateDealerStatistics(DealerStatistics dealerStats)
        {

            using (var context = new GameDbContext())
            {
                // Retrieve the existing DealerStatistics record from the database
                var existingDealerStats = context.DealerStatistics.FirstOrDefault();

                if (existingDealerStats != null)
                {
                    existingDealerStats.Wins += dealerStats.Wins;
                    existingDealerStats.Busts += dealerStats.Busts;
                    existingDealerStats.Ties += dealerStats.Ties;
                    existingDealerStats.Losses += dealerStats.Losses;
                    existingDealerStats.Blackjacks += dealerStats.Blackjacks;

                    context.SaveChanges();
                }
                else
                {
                    context.DealerStatistics.Add(dealerStats);
                    context.SaveChanges();
                }

                PrintDealerTableContent();
            }
        }

        #endregion

        #region Remove

        
        /// <summary>
        /// Takes the player name (can also be the dealer name) and removes it from either the playerStatistics or dealerStatistics table. As the names are the primary key for the tables.
        /// </summary>
        /// <param name="playerName"></param>
        public void RemoveItemFromTable(string playerName)
        {
            using (var context = new GameDbContext())
            {

                var playerToRemove = (from player in context.PlayerStatistics
                                      where player.PlayerName == playerName
                                      select player).SingleOrDefault();

                if (playerToRemove != null)
                {
                    context.PlayerStatistics.Remove(playerToRemove);
                    context.SaveChanges();
                    Debug.WriteLine($"{playerToRemove} has been removed from the table");

                    PrintPlayerTableContent();
                }
            }

            if (playerName == "Croupier")
            {
                using (var context = new GameDbContext())
                {

                    var playerToRemove = (from player in context.DealerStatistics
                                          where player.Name == playerName
                                          select player).SingleOrDefault();

                    if (playerToRemove != null)
                    {
                        context.DealerStatistics.Remove(playerToRemove);
                        context.SaveChanges();
                        Debug.WriteLine($"{playerToRemove} has been removed from the table");

                        PrintPlayerTableContent();
                    }
                }
            }
        }

        /// <summary>
        /// Removes a row from the Game table, using the gameID primary key
        /// </summary>
        /// <param name="gameID"></param>
        public void RemoveItemFromTable(int gameID)
        {
            using (var context = new GameDbContext())
            {

                var gameToRemove = (from game in context.Games
                                    where game.ID == gameID
                                    select game).SingleOrDefault();

                if (gameToRemove != null)
                {
                    context.Games.Remove(gameToRemove);
                    context.SaveChanges();

                    Debug.WriteLine($"{gameToRemove} has been removed from the table");
                    PrintGameTableContent();
                }
            }
        }

        #endregion

        #region Print

       

        private void PrintDealerTableContent()
        {
            using (var context = new GameDbContext())
            {
                var dealerStats = context.DealerStatistics.SingleOrDefault();

                Debug.WriteLine($"Dealer Name: {dealerStats.Name}");
                Debug.WriteLine($"Wins: {dealerStats.Wins}");
                Debug.WriteLine($"Busts: {dealerStats.Busts}");
                Debug.WriteLine($"Ties: {dealerStats.Ties}");
                Debug.WriteLine($"Losses: {dealerStats.Losses}");
                Debug.WriteLine($"Blackjacks: {dealerStats.Blackjacks}");
                Debug.WriteLine("");
            }
        }

        private void PrintGameTableContent()
        {
            using (var context = new GameDbContext())
            {
                var games = context.Games
                    .Include(g => g.GamePlayerStatisticsIntermediary)
                    .ThenInclude(gp => gp.PlayerStatistics)
                    .Include(g => g.DealerStatistics)
                    .ToList();

                foreach (var game in games)
                {
                    Debug.WriteLine($"Game ID: {game.ID}");
                    Debug.WriteLine($"Date: {game.DatePlayed}");
                    Debug.WriteLine($"Dealer name: {game.DealerName}");

                    Debug.WriteLine($"Players:");

                    foreach (var gamePlayerStats in game.GamePlayerStatisticsIntermediary)
                    {
                        Debug.WriteLine($"Player name: {gamePlayerStats.PlayerName}");
                        Debug.WriteLine($"Player Blackjacks: {gamePlayerStats.PlayerStatistics.Blackjacks}");
                        Debug.WriteLine($"Player Wins: {gamePlayerStats.PlayerStatistics.Wins}");
                        Debug.WriteLine($"Player Ties: {gamePlayerStats.PlayerStatistics.Ties}");
                        Debug.WriteLine($"Player Losses: {gamePlayerStats.PlayerStatistics.Losses}");
                    }

                    Debug.WriteLine("Dealer Statistics:");
                    Debug.WriteLine($"Dealer Blackjacks: {game.DealerStatistics.Blackjacks}");
                    Debug.WriteLine($"Dealer Wins: {game.DealerStatistics.Wins}");
                    Debug.WriteLine($"Dealer Ties: {game.DealerStatistics.Ties}");
                    Debug.WriteLine($"Dealer Losses: {game.DealerStatistics.Losses}");

                    Debug.WriteLine("");
                }
            }
        }

        
        public void PrintPlayerTableContent()
        {
            using (var context = new GameDbContext())
            {
                var playerStats = context.PlayerStatistics.ToList();

                foreach (var playerStat in playerStats)
                {
                    Debug.WriteLine($"Player Name: {playerStat.PlayerName}");
                    Debug.WriteLine($"Wins: {playerStat.Wins}");
                    Debug.WriteLine($"Busts: {playerStat.Busts}");
                    Debug.WriteLine($"Ties: {playerStat.Ties}");
                    Debug.WriteLine($"Losses: {playerStat.Losses}");
                    Debug.WriteLine($"Blackjacks: {playerStat.Blackjacks}");
                    Debug.WriteLine("Playertable printed");
                }
            }

                    Debug.WriteLine("");
        }

        #endregion
    }
}
