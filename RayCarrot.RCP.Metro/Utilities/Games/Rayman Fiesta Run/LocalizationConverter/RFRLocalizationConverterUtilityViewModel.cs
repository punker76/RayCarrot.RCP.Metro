﻿using RayCarrot.IO;
using RayCarrot.Rayman;

namespace RayCarrot.RCP.Metro
{
    /// <summary>
    /// View model for the Rayman Fiesta Run localization converter utility
    /// </summary>
    public class RFRLocalizationConverterUtilityViewModel : BaseUbiArtLocalizationConverterUtilityViewModel<BinarySerializableDictionary<int, BinarySerializableDictionary<int, BinarySerializablePair<string, string>>>>
    {
        /// <summary>
        /// The default localization directory for the game, if available
        /// </summary>
        protected override FileSystemPath? DefaultLocalizationDirectory => Games.RaymanFiestaRun.GetInstallDir() + "resources" + "localisation";

        /// <summary>
        /// The localization file extension
        /// </summary>
        protected override string LocalizationFileExtension => ".loc";

        /// <summary>
        /// Deserializes the localization file
        /// </summary>
        /// <param name="file">The localization file</param>
        /// <returns>The data</returns>
        protected override BinarySerializableDictionary<int, BinarySerializableDictionary<int, BinarySerializablePair<string, string>>> Deserialize(FileSystemPath file)
        {
            return new FiestaRunLocalizationSerializer().Deserialize(file).Strings;
        }

        /// <summary>
        /// Serializes the data to the localization file
        /// </summary>
        /// <param name="file">The localization file</param>
        /// <param name="data">The data</param>
        protected override void Serialize(FileSystemPath file, BinarySerializableDictionary<int, BinarySerializableDictionary<int, BinarySerializablePair<string, string>>> data)
        {
            // Get the serializer
            var serializer = new FiestaRunLocalizationSerializer();

            // Read the current data to get the remaining bytes
            var currentData = serializer.Deserialize(file);

            // Replace the string data
            currentData.Strings = data;

            // Serialize the data
            serializer.Serialize(file, currentData);
        }
    }
}