﻿using System.Collections.ObjectModel;
using System.Globalization;
using RayCarrot.CarrotFramework;

namespace RayCarrot.RCP.Metro
{
    /// <summary>
    /// Helper class for application languages
    /// </summary>
    public static class AppLanguages
    {
        #region Constructor

        static AppLanguages()
        {
            Languages = new ObservableCollection<CultureInfo>();

            DefaultCulture = new CultureInfo("en-US");

            RefreshLanguages(false);
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Refreshes the list of available languages
        /// </summary>
        /// <param name="includeIncomplete">Indicates if languages with incomplete translations should be included</param>
        public static void RefreshLanguages(bool includeIncomplete)
        {
            Languages.Clear();

            Languages.AddRange(new CultureInfo[]
            {
                DefaultCulture
            });

            if (includeIncomplete)
            {
                Languages.AddRange(new CultureInfo[]
                {
                    // Swedish
                    new CultureInfo("sv-SE"),

                    // German
                    new CultureInfo("de-DE"),

                    // Polish
                    new CultureInfo("pl-PL"),

                    // Portuguese
                    new CultureInfo("pt-PT"), 

                    // Serbian
                    new CultureInfo("sr-Cyrl"), 

                    // Dutch
                    new CultureInfo("nl-NL"),

                    // Spanish
                    new CultureInfo("es-PR"), 
                });
            }
        }

        #endregion

        #region Public Static Properties

        /// <summary>
        /// The default culture
        /// </summary>
        public static CultureInfo DefaultCulture { get; }

        /// <summary>
        /// The available languages
        /// </summary>
        public static ObservableCollection<CultureInfo> Languages { get; }

        /// <summary>
        /// The path to the resource file
        /// </summary>
        public static string ResourcePath => "RayCarrot.RCP.Metro.Localization.Resources";

        #endregion
    }
}