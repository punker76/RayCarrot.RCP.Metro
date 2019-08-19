﻿using RayCarrot.CarrotFramework.Abstractions;
using RayCarrot.Extensions;
using RayCarrot.IO;
using RayCarrot.UI;
using RayCarrot.Windows.Shell;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel;

namespace RayCarrot.RCP.Metro
{
    /// <summary>
    /// View model for a game options dialog
    /// </summary>
    public class GameOptionsViewModel : BaseRCPViewModel
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="game">The game to show the options for</param>
        public GameOptionsViewModel(Games game)
        {
            // Create the commands
            RemoveCommand = new AsyncRelayCommand(RemoveAsync);
            ShortcutCommand = new AsyncRelayCommand(CreateShortcutAsync);

            // TODO: Refresh this when game refresh event is called
            // TODO: Get dynamically as list of VMs from managers
            Game = game;
            DisplayName = game.GetDisplayName();
            IconSource = game.GetIconSource();
            GameInfo = game.GetInfo();
            LaunchInfo = GameInfo.GameType == GameType.Win32 || GameInfo.GameType == GameType.DosBox ? game.GetGameManager().GetLaunchInfo() : null;
            InstallDir = GameInfo.InstallDirectory;
            if (GameInfo.GameType == GameType.WinStore)
                AddPackageInfo();

            // TODO: Get property from manager?
            // Check if the launch mode can be changed
            CanChangeLaunchMode = GameInfo.GameType == GameType.Win32 || GameInfo.GameType == GameType.DosBox;

            // Get the utilities view models
            Utilities = App.GetUtilities(Game).Select(x => new RCPUtilityViewModel(x)).ToArray();

            // Get the options and config content, if available
            ConfigContent = game.GetConfigContent();
            OptionsContent = game.GetOptionsContent();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The game
        /// </summary>
        public Games Game { get; }

        /// <summary>
        /// The game info
        /// </summary>
        public GameInfo GameInfo { get; set; }

        /// <summary>
        /// The game options content
        /// </summary>
        public object OptionsContent { get; }

        /// <summary>
        /// The display name
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// The icons source
        /// </summary>
        public string IconSource { get; }

        /// <summary>
        /// The game launch info, if available
        /// </summary>
        public GameLaunchInfo LaunchInfo { get; }

        /// <summary>
        /// The game's Steam ID
        /// </summary>
        public string SteamID => GameInfo.GameType == GameType.Steam ? Game.GetGameManager<SteamGameManager>().GetSteamID() : null;

        /// <summary>
        /// The game install directory
        /// </summary>
        public string InstallDir { get; set; }

        /// <summary>
        /// The Windows Store app dependencies
        /// </summary>
        public string WinStoreDependencies { get; set; }

        /// <summary>
        /// The Windows Store app full name
        /// </summary>
        public string WinStoreFullName { get; set; }

        /// <summary>
        /// The Windows Store app architecture
        /// </summary>
        public string WinStoreArchitecture { get; set; }

        /// <summary>
        /// The Windows Store app version
        /// </summary>
        public string WinStoreVersion { get; set; }

        /// <summary>
        /// The Windows Store app install date
        /// </summary>
        public DateTime WinStoreInstallDate { get; set; }

        /// <summary>
        /// Indicates if the launch mode can be changed
        /// </summary>
        public bool CanChangeLaunchMode { get; }

        /// <summary>
        /// The game's launch mode
        /// </summary>
        public GameLaunchMode LaunchMode
        {
            get => GameInfo.LaunchMode;
            set
            {
                GameInfo.LaunchMode = value;
                _ = App.OnRefreshRequiredAsync(new RefreshRequiredEventArgs(Game, false, true, false, false));
            }
        }

        /// <summary>
        /// The utilities for the game
        /// </summary>
        public RCPUtilityViewModel[] Utilities { get; }

        /// <summary>
        /// Indicates if the game has utilities content
        /// </summary>
        public bool HasUtilities => Utilities.Any();

        /// <summary>
        /// The config content for the game
        /// </summary>
        public object ConfigContent { get; set; }

        /// <summary>
        /// Indicates if the game has config content
        /// </summary>
        public bool HasConfigContent => ConfigContent != null;

        #endregion

        #region Commands

        /// <summary>
        /// The command for removing the game from the program
        /// </summary>
        public ICommand RemoveCommand { get; }

        /// <summary>
        /// The command for creating a shortcut to launch the game
        /// </summary>
        public ICommand ShortcutCommand { get; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Adds Windows store package information
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AddPackageInfo()
        {
            if (!(Game.GetGameManager<WinStoreGameManager>().GetGamePackage() is Package package))
            {
                RCFCore.Logger?.LogErrorSource("Game options WinStore package is null");
                return;
            }

            WinStoreDependencies = package.Dependencies.Select(x => x.Id.Name).JoinItems(", ");
            WinStoreFullName = package.Id.FullName;
            WinStoreArchitecture = package.Id.Architecture.ToString();
            WinStoreVersion = $"{package.Id.Version.Major}.{package.Id.Version.Minor}.{package.Id.Version.Build}.{package.Id.Version.Revision}";
            WinStoreInstallDate = package.InstalledDate.DateTime;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Removes the game from the program
        /// </summary>
        /// <returns>The task</returns>
        public async Task RemoveAsync()
        {
            // Ask the user
            if (!await RCFUI.MessageUI.DisplayMessageAsync(String.Format(Resources.RemoveGameQuestion, DisplayName), Resources.RemoveGameQuestionHeader,  MessageType.Question, true))
                return;

            // Remove the game
            await RCFRCP.App.RemoveGameAsync(Game, false);
        }

        /// <summary>
        /// Creates a shortcut to launch the game
        /// </summary>
        /// <returns>The task</returns>
        public async Task CreateShortcutAsync()
        {
            try
            {
                var result = await RCFUI.BrowseUI.BrowseDirectoryAsync(new DirectoryBrowserViewModel()
                {
                    DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    Title = Resources.GameShortcut_BrowseHeader
                });

                if (result.CanceledByUser)
                    return;

                var gameInfo = Game.GetInfo();
                var shortcutName = String.Format(Resources.GameShortcut_ShortcutName, Game.GetDisplayName());

                if (gameInfo.GameType == GameType.Steam)
                {
                    WindowsHelpers.CreateURLShortcut(shortcutName, result.SelectedDirectory, $@"steam://rungameid/{Game.GetGameManager<SteamGameManager>().GetSteamID()}");

                    RCFCore.Logger?.LogTraceSource($"A shortcut was created for {Game} under {result.SelectedDirectory}");

                    await RCFUI.MessageUI.DisplaySuccessfulActionMessageAsync(Resources.GameShortcut_Success);
                }
                else
                {
                    var launchInfo = Game.GetGameManager().GetLaunchInfo();

                    await RCFRCP.File.CreateFileShortcutAsync(shortcutName, result.SelectedDirectory, launchInfo.Path, launchInfo.Args);
                }
            }
            catch (Exception ex)
            {
                ex.HandleError("Creating game shortcut", Game);
                await RCFUI.MessageUI.DisplayMessageAsync(Resources.GameShortcut_Error, Resources.GameShortcut_ErrorHeader, MessageType.Error);
            }
        }

        #endregion
    }
}