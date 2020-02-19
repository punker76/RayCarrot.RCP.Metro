﻿using RayCarrot.IO;
using RayCarrot.Rayman.UbiArt;

namespace RayCarrot.RCP.Metro
{
    /// <summary>
    /// View model for the Rayman Fiesta Run localization converter utility
    /// </summary>
    public class RFRLocalizationConverterUtilityViewModel : BaseUbiArtLocalizationConverterUtilityViewModel<SerializableDictionary<int, SerializableDictionary<int, SerializablePair<string, string>>>>
    {
        /// <summary>
        /// The default localization directory for the game, if available
        /// </summary>
        protected override FileSystemPath? DefaultLocalizationDirectory => Games.RaymanFiestaRun.GetInstallDir() + "resources" + "localisation";

        /// <summary>
        /// The localization file extension
        /// </summary>
        protected override FileExtension LocalizationFileExtension => new FileExtension(".loc");

        /// <summary>
        /// Deserializes the localization file
        /// </summary>
        /// <param name="file">The localization file</param>
        /// <returns>The data</returns>
        protected override SerializableDictionary<int, SerializableDictionary<int, SerializablePair<string, string>>> Deserialize(FileSystemPath file)
        {
            return FiestaRunLocalizationData.GetSerializer().Deserialize(file).Strings;
        }

        /// <summary>
        /// Serializes the data to the localization file
        /// </summary>
        /// <param name="file">The localization file</param>
        /// <param name="data">The data</param>
        protected override void Serialize(FileSystemPath file, SerializableDictionary<int, SerializableDictionary<int, SerializablePair<string, string>>> data)
        {
            // Get the serializer
            var serializer = FiestaRunLocalizationData.GetSerializer();

            // Read the current data to get the remaining bytes
            var currentData = serializer.Deserialize(file);

            // Replace the string data
            currentData.Strings = data;

            // Serialize the data
            serializer.Serialize(file, currentData);
        }
    }
}