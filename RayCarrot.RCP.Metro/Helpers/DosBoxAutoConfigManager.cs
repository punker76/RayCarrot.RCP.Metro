﻿using System;
using System.IO;
using System.Net;
using System.Text;
using RayCarrot.CarrotFramework;

namespace RayCarrot.RCP.Metro
{
    /// <summary>
    /// Manager for auto DosBox config file
    /// </summary>
    public class DosBoxAutoConfigManager
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="path">The config file path</param>
        public DosBoxAutoConfigManager(FileSystemPath path)
        {
            FilePath = path;
            FirstLines = $"[ipx]{Environment.NewLine}# ipx -- Enable ipx over UDP/IP emulation.{Environment.NewLine}ipx=false{Environment.NewLine}{Environment.NewLine}[autoexec]{Environment.NewLine}@echo off";
        }

        #region Private Properties

        /// <summary>
        /// The first lines of the config file
        /// </summary>
        private string FirstLines { get; }

        /// <summary>
        /// The config file path
        /// </summary>
        private FileSystemPath FilePath { get; }

        /// <summary>
        /// The line separator for the custom commands
        /// </summary>
        private const string CustomCommandsSeparator = "#Custom commands:";

        /// <summary>
        /// The start of a config file
        /// </summary>
        private const string ConfigLineStart = "CONFIG -set ";

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the configuration file if it does not already exist with a valid configuration
        /// </summary>
        public void Create()
        {
            RCF.Logger.LogTraceSource($"Recreating DosBox config file {FilePath}");

            // Check if the file exists and is valid
            if (FilePath.FileExists && File.ReadAllText(FilePath).StartsWith(FirstLines))
            {
                RCF.Logger.LogTraceSource($"The DosBox config file is valid and no further action is needed");
                return;
            }

            Directory.CreateDirectory(FilePath.Parent);

            // Create the file with default content
            File.WriteAllText(FilePath, FirstLines);

            RCF.Logger.LogInformationSource($"The DosBox config file was recreated");
        }

        /// <summary>
        /// Reads the file and returns the data
        /// </summary>
        /// <returns>The file data</returns>
        public DosBoxAutoConfigData ReadFile()
        {
            var content = File.ReadAllText(FilePath);

            // Remove the first lines
            content = content.Substring(FirstLines.Length);

            // Split into lines
            var lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            // Create the output
            var output = new DosBoxAutoConfigData();

            bool customCommands = false;

            // Check each line
            foreach (var line in lines)
            {
                // Flag when the custom commands are reached
                if (!customCommands && line == CustomCommandsSeparator)
                {
                    customCommands = true;
                    continue;
                }

                // Ignore if it's empty
                if (line.IsNullOrWhiteSpace())
                    continue;

                if (customCommands)
                {
                    output.CustomLines.Add(line);
                }
                else
                {
                    // Ignore if it's not a valid config line
                    if (!line.StartsWith(ConfigLineStart))
                        continue;

                    // Get the values
                    var values = line.Substring(ConfigLineStart.Length).Trim('\"').Split('=');

                    // Make sure we have two values
                    if (values.Length != 2)
                        continue;

                    // Add the values
                    output.Commands.Add(values[0], values[1]);
                }
            }

            return output;
        }

        /// <summary>
        /// Writes the specified data to the file
        /// </summary>
        /// <param name="data">The data to write</param>
        public void WriteFile(DosBoxAutoConfigData data)
        {
            StringBuilder stringBuilder = new StringBuilder();

            // Add the first lines
            stringBuilder.AppendLine(FirstLines);

            // Add the commands
            foreach (var command in data.Commands)
                stringBuilder.AppendLine($"{ConfigLineStart}\"{command.Key}={command.Value}\"");

            // Add the custom commands separator
            stringBuilder.AppendLine(CustomCommandsSeparator);

            // Add the custom commands
            foreach (string customLine in data.CustomLines)
                stringBuilder.AppendLine(customLine);

            // Write the text to the file
            File.WriteAllText(FilePath, stringBuilder.ToString());
        }

        #endregion
    }
}