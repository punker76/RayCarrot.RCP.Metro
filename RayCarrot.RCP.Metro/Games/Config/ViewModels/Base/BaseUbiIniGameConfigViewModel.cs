﻿using System;
using System.Threading.Tasks;
using IniParser.Model;
using Nito.AsyncEx;
using RayCarrot.Common;
using RayCarrot.IO;
using RayCarrot.Logging;
using RayCarrot.Rayman.UbiIni;
using RayCarrot.UI;

namespace RayCarrot.RCP.Metro
{
    /// <summary>
    /// Base class for ubi.ini game config view models
    /// </summary>
    /// <typeparam name="Handler">The config handler</typeparam>
    public abstract class BaseUbiIniGameConfigViewModel<Handler> : GameConfigViewModel
        where Handler : UbiIniHandler
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="game">The game</param>
        protected BaseUbiIniGameConfigViewModel(Games game)
        {
            // Create the async lock
            AsyncLock = new AsyncLock();

            // Create the commands
            SaveCommand = new AsyncRelayCommand(SaveAsync);

            // Set properties
            Game = game;
            CanModifyGame = RCPServices.File.CheckDirectoryWriteAccess(Game.GetInstallDir(false));

            if (!CanModifyGame)
                RL.Logger?.LogInformationSource($"The game {Game} can't be modified");
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The game
        /// </summary>
        public Games Game { get; }

        /// <summary>
        /// Indicates if the game can be modified
        /// </summary>
        public bool CanModifyGame { get; }

        #endregion

        #region Protected Properties

        /// <summary>
        /// The async lock to use for saving the configuration
        /// </summary>
        protected AsyncLock AsyncLock { get; }

        /// <summary>
        /// The configuration data
        /// </summary>
        protected Handler ConfigData { get; set; }

        #endregion

        #region Commands

        public AsyncRelayCommand SaveCommand { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads and sets up the current configuration properties
        /// </summary>
        /// <returns>The task</returns>
        public override async Task SetupAsync()
        {
            RL.Logger?.LogInformationSource($"{Game} config is being set up");

            // Run setup code
            await OnSetupAsync();

            // Load the configuration data
            ConfigData = await LoadConfigAsync();

            RL.Logger?.LogInformationSource($"The ubi.ini file has been loaded");

            // Keep track if the data had to be recreated
            bool recreated = false;

            // Re-create the section if it doesn't exist
            if (!ConfigData.Exists)
            {
                ConfigData.ReCreate();
                recreated = true;
                RL.Logger?.LogInformationSource($"The ubi.ini section for {Game} was recreated");
            }

            // Import config data
            await ImportConfigAsync();

            // If the data was recreated we mark that there are unsaved changes available
            UnsavedChanges = recreated;

            RL.Logger?.LogInformationSource($"All section properties have been loaded");
        }

        /// <summary>
        /// Saves the changes
        /// </summary>
        /// <returns>The task</returns>
        public async Task SaveAsync()
        {
            using (await AsyncLock.LockAsync())
            {
                RL.Logger?.LogInformationSource($"{Game} configuration is saving...");

                try
                {
                    // Update the config data
                    await UpdateConfigAsync();

                    // Save the config data
                    ConfigData.Save();

                    RL.Logger?.LogInformationSource($"{Game} configuration has been saved");
                }
                catch (Exception ex)
                {
                    ex.HandleError("Saving ubi.ini data");
                    await WPF.Services.MessageUI.DisplayExceptionMessageAsync(ex, String.Format(Resources.Config_SaveError, Game.GetGameInfo().DisplayName), Resources.Config_SaveErrorHeader);
                    return;
                }

                try
                {
                    // Run save code
                    await OnSaveAsync();
                }
                catch (Exception ex)
                {
                    ex.HandleError("On save config");
                    await WPF.Services.MessageUI.DisplayExceptionMessageAsync(ex, Resources.Config_SaveWarning, Resources.Config_SaveErrorHeader);

                    return;
                }

                UnsavedChanges = false;

                await WPF.Services.MessageUI.DisplaySuccessfulActionMessageAsync(Resources.Config_SaveSuccess);

                OnSave();
            }
        }

        #endregion

        #region Protected Virtual Methods

        /// <summary>
        /// Override to run on setup
        /// </summary>
        /// <returns>The task</returns>
        protected virtual Task OnSetupAsync() => Task.CompletedTask;

        /// <summary>
        /// Override to run on saving
        /// </summary>
        /// <returns>The task</returns>
        protected virtual Task OnSaveAsync() => Task.CompletedTask;

        #endregion

        #region Protected Abstract Methods

        /// <summary>
        /// Loads the <see cref="ConfigData"/>
        /// </summary>
        /// <returns>The config data</returns>
        protected abstract Task<Handler> LoadConfigAsync();

        /// <summary>
        /// Imports the <see cref="ConfigData"/>
        /// </summary>
        /// <returns>The task</returns>
        protected abstract Task ImportConfigAsync();

        /// <summary>
        /// Updates the <see cref="ConfigData"/>
        /// </summary>
        /// <returns>The task</returns>
        protected abstract Task UpdateConfigAsync();

        #endregion

        #region Protected Classes

        /// <summary>
        /// Provides support to duplicate a section
        /// in a ubi ini file
        /// </summary>
        protected class DuplicateSectionUbiIniHandler : UbiIniHandler
        {
            /// <summary>
            /// Default constructor
            /// </summary>
            /// <param name="path">The path of the ubi.ini file</param>
            /// <param name="sectionKey">The name of the section to retrieve, usually the name of the game</param>
            public DuplicateSectionUbiIniHandler(FileSystemPath path, string sectionKey) : base(path, sectionKey)
            {

            }

            public void Duplicate(KeyDataCollection sectionData)
            {
                // Recreate the section
                ReCreate();

                // Add all new keys
                sectionData.ForEach(x => Section.AddKey(x));
            }
        }

        #endregion
    }
}