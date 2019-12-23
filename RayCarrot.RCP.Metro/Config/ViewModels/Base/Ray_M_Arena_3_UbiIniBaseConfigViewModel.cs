﻿using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using ByteSizeLib;
using RayCarrot.CarrotFramework.Abstractions;
using RayCarrot.IO;
using RayCarrot.Rayman;
using RayCarrot.RCP.Core;
using RayCarrot.UI;

namespace RayCarrot.RCP.Metro
{
    /// <summary>
    /// Base config view model for Rayman M, Rayman Arena and Rayman 3
    /// </summary>
    public abstract class Ray_M_Arena_3_UbiIniBaseConfigViewModel<Handler, Language> : BaseUbiIniGameConfigViewModel<Handler>
        where Handler : UbiIniHandler
        where Language : Enum
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="game">The game</param>
        protected Ray_M_Arena_3_UbiIniBaseConfigViewModel(Games game) : base(game)
        {
            if (game != Games.Rayman3 &&
                game != Games.RaymanArena &&
                game != Games.RaymanM)
                throw new ArgumentOutOfRangeException(nameof(game), "The game for the base config VM has to be Rayman M, Arena or 3");
        }

        #endregion

        #region Private Fields

        private int _resX;

        private int _resY;

        private bool _lockToScreenRes;

        private bool _fullscreenMode;

        private bool _triLinear;

        private bool _tnL;

        private bool _isTextures32Bit;

        private bool _compressedTextures;

        private int _videoQuality;

        private bool _autoVideoQuality;

        private bool _isVideo32Bpp;

        private Language _currentLanguage;

        private bool _controllerSupport;

        private bool _isDiscCheckRemoved;

        #endregion

        #region Protected Abstract Properties

        /// <summary>
        /// The offset for when patching the game check or -1 if not available
        /// </summary>
        protected abstract int PatchGameCheckOffset { get; }

        /// <summary>
        /// The original bytes for when patching the game check
        /// </summary>
        protected abstract byte[] PatchGameCheckOriginalBytes { get; }

        /// <summary>
        /// The patched bytes when patching the game check
        /// </summary>
        protected abstract byte[] PatchGameCheckPatchedBytes { get; }

        #endregion

        #region Protected Properties

        /// <summary>
        /// The game patcher to use for patching the disc check
        /// </summary>
        protected GamePatcher Patcher { get; set; }

        #endregion

        #region Public Properties

        /// <summary>
        /// The current horizontal resolution
        /// </summary>
        public int ResX
        {
            get => _resX;
            set
            {
                _resX = value;
                UnsavedChanges = true;
            }
        }

        /// <summary>
        /// The current vertical resolution
        /// </summary>
        public int ResY
        {
            get => _resY;
            set
            {
                _resY = value;
                UnsavedChanges = true;
            }
        }

        /// <summary>
        /// Indicates if the resolution is locked to the current screen resolution
        /// </summary>
        public bool LockToScreenRes
        {
            get => _lockToScreenRes;
            set
            {
                _lockToScreenRes = value;

                if (!value)
                    return;

                ResY = (int)SystemParameters.PrimaryScreenHeight;
                ResX = (int)Math.Round((double)ResY / 3 * 4);
            }
        }

        /// <summary>
        /// Indicates if fullscreen mode is enabled or if the game should run in windowed mode
        /// </summary>
        public bool FullscreenMode
        {
            get => _fullscreenMode;
            set
            {
                _fullscreenMode = value;
                UnsavedChanges = true;
            }
        }

        /// <summary>
        /// Indicated if TriLinear is enabled
        /// </summary>
        public bool TriLinear
        {
            get => _triLinear;
            set
            {
                _triLinear = value;
                UnsavedChanges = true;
            }
        }

        /// <summary>
        /// Indicated if Transform and Lightning is enabled
        /// </summary>
        public bool TnL
        {
            get => _tnL;
            set
            {
                _tnL = value;
                UnsavedChanges = true;
            }
        }

        /// <summary>
        /// True if textures are 32-bit, false if they are 16-bit
        /// </summary>
        public bool IsTextures32Bit
        {
            get => _isTextures32Bit;
            set
            {
                _isTextures32Bit = value;
                UnsavedChanges = true;
            }
        }

        /// <summary>
        /// Indicates if the compressed textures should be used
        /// </summary>
        public bool CompressedTextures
        {
            get => _compressedTextures;
            set
            {
                _compressedTextures = value;
                UnsavedChanges = true;
            }
        }

        /// <summary>
        /// The video quality, between 0 and 4
        /// </summary>
        public int VideoQuality
        {
            get => _videoQuality;
            set
            {
                _videoQuality = value;
                UnsavedChanges = true;
            }
        }

        /// <summary>
        /// Indicates if the video quality should be auto adjusted
        /// </summary>
        public bool AutoVideoQuality
        {
            get => _autoVideoQuality;
            set
            {
                _autoVideoQuality = value;
                UnsavedChanges = true;
            }
        }

        /// <summary>
        /// True if videos are 32 bits per pixel, false if they are 16 bits per pixel
        /// </summary>
        public bool IsVideo32Bpp
        {
            get => _isVideo32Bpp;
            set
            {
                _isVideo32Bpp = value;
                UnsavedChanges = true;
            }
        }

        /// <summary>
        /// The currently selected language
        /// </summary>
        public Language CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                _currentLanguage = value;
                UnsavedChanges = true;
            }
        }

        /// <summary>
        /// Indicates if controller support is enabled
        /// </summary>
        public bool ControllerSupport
        {
            get => _controllerSupport;
            set
            {
                _controllerSupport = value;
                UnsavedChanges = true;
            }
        }

        /// <summary>
        /// Indicates if the option to remove the disc check from the game is available
        /// </summary>
        public bool CanRemoveDiscCheck { get; set; }

        /// <summary>
        /// Indicates if the disc check is set to be removed
        /// </summary>
        public bool IsDiscCheckRemoved
        {
            get => _isDiscCheckRemoved;
            set
            {
                _isDiscCheckRemoved = value;
                UnsavedChanges = true;
            }
        }

        #endregion

        #region Protected Override Methods

        /// <summary>
        /// Setup
        /// </summary>
        /// <returns>The task</returns>
        protected override async Task OnSetupAsync()
        {
            // Get the current dinput type
            var dinputType = CanModifyGame ? GetCurrentDinput() : DinputType.Unknown;

            RCFCore.Logger?.LogInformationSource($"The dinput type has been retrieved as {dinputType}");

            ControllerSupport = dinputType == DinputType.Controller;

            // Check if the disc check has been removed
            CanRemoveDiscCheck = PatchGameCheckOffset != -1;

            if (CanRemoveDiscCheck)
            {
                // Get the game file path
                var gameFile = Game.GetInstallDir(false) + Game.GetGameInfo().DefaultFileName;

                // Check if it exists
                if (gameFile.FileExists)
                {
                    Patcher = new GamePatcher(gameFile, PatchGameCheckOriginalBytes, PatchGameCheckPatchedBytes, PatchGameCheckOffset);

                    var result = Patcher.GetIsOriginal();

                    if (result == true)
                    {
                        IsDiscCheckRemoved = false;
                        CanRemoveDiscCheck = true;

                        RCFCore.Logger?.LogInformationSource($"The game has not been modified to remove the disc checker");
                    }
                    else if (result == false)
                    {
                        IsDiscCheckRemoved = true;
                        CanRemoveDiscCheck = true;

                        RCFCore.Logger?.LogInformationSource($"The game has been modified to remove the disc checker");
                    }
                    else if (result == null)
                    {
                        CanRemoveDiscCheck = false;

                        RCFCore.Logger?.LogInformationSource($"The game disc checker status could not be read");
                    }
                }
                else
                {
                    CanRemoveDiscCheck = false;

                    RCFCore.Logger?.LogInformationSource($"The game file was not found");
                }
            }
            else
            {
                RCFCore.Logger?.LogTraceSource($"The disc checker can not be removed for this game");
            }

            // If the primary config file does not exist, create a new one
            if (!CommonPaths.UbiIniPath1.FileExists)
            {
                try
                {
                    // Create the file
                    RCFRCP.File.CreateFile(CommonPaths.UbiIniPath1);

                    RCFCore.Logger?.LogInformationSource($"A new ubi.ini file has been created under {CommonPaths.UbiIniPath1}");
                }
                catch (Exception ex)
                {
                    ex.HandleError("Creating ubi.ini file");

                    await RCFUI.MessageUI.DisplayExceptionMessageAsync(ex, String.Format(Resources.Config_InvalidUbiIni, CommonPaths.UbiIniPath1.Parent));

                    throw;
                }
            }

            // If the secondary config file does not exist, attempt to create a new one
            if (!CommonPaths.UbiIniPath2.FileExists)
            {
                try
                {
                    // Create the file
                    RCFRCP.File.CreateFile(CommonPaths.UbiIniPath2);

                    RCFCore.Logger?.LogInformationSource($"A new ubi.ini file has been created under {CommonPaths.UbiIniPath2}");
                }
                catch (Exception ex)
                {
                    ex.HandleError("Creating ubi.ini file");
                }
            }
        }

        /// <summary>
        /// Saving
        /// </summary>
        /// <returns>The task</returns>
        protected override Task OnSaveAsync()
        {
            // Attempt to copy data to secondary file
            if (CommonPaths.UbiIniPath2.FileExists)
            {
                try
                {
                    // Get the current data
                    var sectionData = ConfigData.GetSectionData();

                    // Load the file data
                    var secondaryDataHandler = new DuplicateSectionUbiIniHandler(CommonPaths.UbiIniPath2, ConfigData.SectionKey);

                    // Duplicate the data
                    secondaryDataHandler.Duplicate(sectionData);

                    // Save the file
                    secondaryDataHandler.Save();
                }
                catch (Exception ex)
                {
                    ex.HandleError($"Saving {Game} ubi.ini secondary data");
                }
            }

            if (CanModifyGame)
            {
                try
                {
                    // Get the current dinput type
                    var dt = GetCurrentDinput();
                    var path = GetDinputPath();

                    RCFCore.Logger?.LogInformationSource($"The dinput type has been retrieved as {dt}");

                    if (ControllerSupport)
                    {
                        if (dt != DinputType.Controller)
                        {
                            if (dt != DinputType.None)
                                // Attempt to delete existing dinput file
                                RCFRCP.File.DeleteFile(path);

                            // Write controller patch
                            File.WriteAllBytes(path, Files.dinput8_controller);
                        }
                    }
                    else if (dt == DinputType.Controller)
                    {
                        // Attempt to delete existing dinput file
                        RCFRCP.File.DeleteFile(path);
                    }

                }
                catch (Exception ex)
                {
                    ex.HandleError($"Saving {Game} dinput hack data");
                    throw;
                }

                if (CanRemoveDiscCheck)
                {
                    try
                    {
                        Patcher = new GamePatcher(Game.GetInstallDir() + Game.GetGameInfo().DefaultFileName, PatchGameCheckOriginalBytes, PatchGameCheckPatchedBytes, PatchGameCheckOffset);

                        var result = Patcher.GetIsOriginal();

                        if (result == true && IsDiscCheckRemoved)
                            Patcher.PatchFile(false);

                        else if (result == false && !IsDiscCheckRemoved)
                            Patcher.PatchFile(true);
                    }
                    catch (Exception ex)
                    {
                        ex.HandleError($"Saving {Game} disc check modification");
                        throw;
                    }
                }
            }

            return Task.CompletedTask;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the current dinput file used
        /// </summary>
        /// <returns>The current dinput file used</returns>
        private DinputType GetCurrentDinput()
        {
            var path = GetDinputPath();

            if (!path.FileExists)
                return DinputType.None;

            try
            {
                var size = path.GetSize();

                if (size == new ByteSize(118272))
                    return DinputType.Controller;

                // If the size equals that of the Rayman 2 dinput file, delete it
                // as the Rayman 2 dinput file was accidentally used prior to version 4.1.2
                if (size == new ByteSize(66560))
                {
                    RCFRCP.File.DeleteFile(path);
                    return DinputType.None;
                }

                return DinputType.Unknown;
            }
            catch (Exception ex)
            {
                ex.HandleError($"Getting {Game} dinput file size");
                return DinputType.Unknown;
            }
        }

        /// <summary>
        /// Gets the current dinput.dll path
        /// </summary>
        /// <returns>The path</returns>
        private FileSystemPath GetDinputPath()
        {
            return Game.GetInstallDir(false) + "dinput8.dll";
        }

        #endregion

        #region Private Enum

        /// <summary>
        /// The available types of the dinput8.dll file
        /// </summary>
        private enum DinputType
        {
            /// <summary>
            /// No file found
            /// </summary>
            None,

            /// <summary>
            /// Controller fix
            /// </summary>
            Controller,

            /// <summary>
            /// Unknown
            /// </summary>
            Unknown
        }

        #endregion
    }
}