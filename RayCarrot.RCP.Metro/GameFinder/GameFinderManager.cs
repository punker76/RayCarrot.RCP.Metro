﻿using RayCarrot.CarrotFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RayCarrot.RCP.Metro
{
    /// <summary>
    /// Checks for installed games based on a collection of <see cref="GameItems"/>
    /// </summary>
    public class GameFinderManager
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public GameFinderManager()
        {
            GameItems = Enum.GetValues(typeof(Games))
                .Cast<Games>()
                .Where(x => !x.IsAdded())
                .ToDictionary(x => x, y => new List<Func<GameFinderActionResult>>());

            RCF.Logger.LogTraceSource($"The following games were added to the game checker: {GameItems.Keys.JoinItems(", ")}");
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The game items to check
        /// </summary>
        public Dictionary<Games, List<Func<GameFinderActionResult>>> GameItems { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Runs the check
        /// </summary>
        /// <returns>The new <see cref="Games"/> which were found</returns>
        public async Task<List<Games>> RunAsync()
        {
            List<Games> foundGames = new List<Games>();

            // Check every game entry
            foreach (var game in GameItems)
            {
                // Run every check action until one is successful
                foreach (var action in game.Value)
                {
                    // Stop running the check actions if the game has been added
                    if (game.Key.IsAdded())
                        break;

                    try
                    {
                        // Get the result from the action
                        var result = action();

                        // Check if the directory exists
                        if (!result.Path.DirectoryExists)
                            continue;

                        RCF.Logger.LogTraceSource($"The game {game.Key} was found from the game checker with the source {result.Source}");

                        // Add the game
                        await RCFRCP.App.AddNewGameAsync(game.Key, result.Type, result.Path);
                        foundGames.Add(game.Key);

                        RCF.Logger.LogInformationSource($"The game {game} has been added from the game finder");

                        break;
                    }
                    catch (Exception ex)
                    {
                        ex.HandleUnexpected("Game check action", action);
                    }
                }
            }

            return foundGames;
        }

        #endregion
    }
}