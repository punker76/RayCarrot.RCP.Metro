﻿using RayCarrot.IO;
using RayCarrot.Rayman;

namespace RayCarrot.RCP.Metro
{
    /// <summary>
    /// View model for the Rayman Origins localization converter utility
    /// </summary>
    public class ROLocalizationConverterUtilityViewModel : BaseUbiArtLocalizationConverterUtilityViewModel<UbiArtSerializableDictionary<int, UbiArtSerializableDictionary<int, string>>>
    {
        /// <summary>
        /// The default localization directory for the game, if available
        /// </summary>
        protected override FileSystemPath? DefaultLocalizationDirectory => null;

        /// <summary>
        /// The localization file extension
        /// </summary>
        protected override string LocalizationFileExtension => ".loc";

        /// <summary>
        /// Deserializes the localization file
        /// </summary>
        /// <param name="file">The localization file</param>
        /// <returns>The data</returns>
        protected override UbiArtSerializableDictionary<int, UbiArtSerializableDictionary<int, string>> Deserialize(FileSystemPath file)
        {
            return UbiArtLocalizationData.GetSerializer(UbiArtGameMode.RaymanOriginsPC.GetSettings()).Deserialize(file).Strings;
        }

        /// <summary>
        /// Serializes the data to the localization file
        /// </summary>
        /// <param name="file">The localization file</param>
        /// <param name="data">The data</param>
        protected override void Serialize(FileSystemPath file, UbiArtSerializableDictionary<int, UbiArtSerializableDictionary<int, string>> data)
        {
            // Get the serializer
            var serializer = UbiArtLocalizationData.GetSerializer(UbiArtGameMode.RaymanOriginsPC.GetSettings());

            // Read the current data to get the remaining bytes
            var currentData = serializer.Deserialize(file);

            // Replace the string data
            currentData.Strings = data;

            // Serialize the data
            serializer.Serialize(file, currentData);
        }
    }
}