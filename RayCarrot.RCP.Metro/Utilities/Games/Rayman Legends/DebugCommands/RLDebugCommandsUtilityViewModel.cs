﻿using RayCarrot.Common;
using RayCarrot.IO;
using RayCarrot.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RayCarrot.RCP.Metro
{
    /// <summary>
    /// View model for the Rayman Legends debug commands utility
    /// </summary>
    public class RLDebugCommandsUtilityViewModel : BaseRCPViewModel
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public RLDebugCommandsUtilityViewModel()
        {
            // Create properties
            DebugCommands = new Dictionary<string, string>();
            AvailableMaps = Files.RL_Levels.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Where(x => !x.Contains("graph") && !x.Contains("subscene") && !x.Contains("brick") && !x.Contains("actor")).ToArray();

            // Create commands
            LaunchGameCommand = new AsyncRelayCommand(LaunchGameAsync);

            // Get the Rayman Origins install directory
            var instDir = Games.RaymanLegends.GetInstallDir(false);

            GameFilePath = instDir.DirectoryExists
                ? instDir + Games.RaymanLegends.GetGameInfo().DefaultFileName
                : FileSystemPath.EmptyPath;
        }

        #endregion

        #region Private Constants

        private const string InvincibilityKey = "player_nodamage";

        private const string MouseHiddenKey = "nomouse";

        private const string MapKey = "map";

        private const string DisableSaveKey = "nosave";

        #endregion

        #region Commands

        public ICommand LaunchGameCommand { get; }

        #endregion

        #region Public Properties

        /// <summary>
        /// The Rayman Legends executable file path
        /// </summary>
        public FileSystemPath GameFilePath { get; }

        /// <summary>
        /// The available debug commands
        /// </summary>
        public Dictionary<string, string> DebugCommands { get; }

        /// <summary>
        /// Indicates if invincibility is enabled
        /// </summary>
        public bool? IsInvincibilityEnabled
        {
            get
            {
                var value = DebugCommands.TryGetValue(InvincibilityKey);

                if (value == null)
                    return null;

                return value == "1";
            }
            set
            {
                if (value is bool v)
                    DebugCommands.SetValue(InvincibilityKey, v ? "1" : "0");
                else
                    DebugCommands.Remove(InvincibilityKey);
            }
        }

        /// <summary>
        /// Indicates if the mouse should be hidden
        /// </summary>
        public bool? IsMouseHidden
        {
            get
            {
                var value = DebugCommands.TryGetValue(MouseHiddenKey);

                if (value == null)
                    return null;

                return value == "1";
            }
            set
            {
                if (value is bool v)
                    DebugCommands.SetValue(MouseHiddenKey, v ? "1" : "0");
                else
                    DebugCommands.Remove(MouseHiddenKey);
            }
        }

        /// <summary>
        /// The available map paths to choose from
        /// </summary>
        public string[] AvailableMaps { get; }

        /// <summary>
        /// The path of the map to load
        /// </summary>
        public string MapPath
        {
            get => DebugCommands.TryGetValue(MapKey);
            set
            {
                if (value is string v && !v.IsNullOrWhiteSpace())
                    DebugCommands.SetValue(MapKey, v);
                else
                    DebugCommands.Remove(MapKey);
            }
        }

        /// <summary>
        /// Indicates if save should be disabled
        /// </summary>
        public bool? IsSaveDisabled
        {
            get
            {
                var value = DebugCommands.TryGetValue(DisableSaveKey);

                if (value == null)
                    return null;

                return value == "1";
            }
            set
            {
                if (value is bool v)
                    DebugCommands.SetValue(DisableSaveKey, v ? "1" : "0");
                else
                    DebugCommands.Remove(DisableSaveKey);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Launches the game with the specified debug commands
        /// </summary>
        /// <returns>The task</returns>
        public async Task LaunchGameAsync()
        {
            await RCPServices.File.LaunchFileAsync(GameFilePath, false, DebugCommands.Select(x => $"{x.Key}={x.Value}").JoinItems(";"));
        }

        #endregion
    }
}