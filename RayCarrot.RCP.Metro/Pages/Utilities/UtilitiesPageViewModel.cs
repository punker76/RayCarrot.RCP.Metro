﻿namespace RayCarrot.RCP.Metro
{
    /// <summary>
    /// View model for the utilities page
    /// </summary>
    public class UtilitiesPageViewModel : BaseRCPViewModel
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public UtilitiesPageViewModel()
        {
            // Create view models
            ArchiveExplorerViewModels = new UtilityViewModel[]
            {
                new UtilityViewModel(new IPKArchiveExplorerUtility()),
                new UtilityViewModel(new CNTArchiveExplorerUtility()),
            };
            ConverterViewModels = new UtilityViewModel[]
            {
                new UtilityViewModel(new GFConverterUtility()),
                new UtilityViewModel(new R3SaveConverterUtility()),
                new UtilityViewModel(new LOCConverterUtility()),
                new UtilityViewModel(new RJRSaveConverterUtility()),
            };
            OtherViewModels = new UtilityViewModel[]
            {
                new UtilityViewModel(new R1MapViewerUtility()),
                new UtilityViewModel(new SyncTextureInfoUtility()),
            };
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// View models for the Archive Explorer utilities
        /// </summary>
        public UtilityViewModel[] ArchiveExplorerViewModels { get; }

        /// <summary>
        /// View models for the converter utilities
        /// </summary>
        public UtilityViewModel[] ConverterViewModels { get; }

        /// <summary>
        /// View models for the other utilities
        /// </summary>
        public UtilityViewModel[] OtherViewModels { get; }

        #endregion
    }
}