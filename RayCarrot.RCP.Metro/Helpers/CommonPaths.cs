﻿using System;
using System.IO;
using RayCarrot.CarrotFramework;
using RayCarrot.Windows.Registry;

namespace RayCarrot.RCP.Metro
{
    /// <summary>
    /// Common paths used in the Rayman Control Panel
    /// </summary>
    public static class CommonPaths
    {
        /// <summary>
        /// The base user data directory
        /// </summary>
        public static FileSystemPath UserDataBaseDir => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Rayman Control Panel", "RCP_Metro");

        /// <summary>
        /// The <see cref="AppUserData"/> file path
        /// </summary>
        public static FileSystemPath AppUserDataPath => UserDataBaseDir + "appuserdata.json";

        /// <summary>
        /// The log file path
        /// </summary>
        public static FileSystemPath LogFile => UserDataBaseDir + "Temp\\Log.txt";

        /// <summary>
        /// The path for temporary files in this application
        /// </summary>
        public static FileSystemPath TempPath => Path.Combine(Path.GetTempPath(), "RCP_Metro");

        /// <summary>
        /// The common path to the ubi.ini file
        /// </summary>
        public static FileSystemPath UbiIniPath1 => @"C:\Windows\Ubisoft\ubi.ini";

        /// <summary>
        /// The second common path to the ubi.ini file
        /// </summary>
        public static FileSystemPath UbiIniPath2 => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VirtualStore\\Windows\\Ubisoft\\Ubi.ini");

        /// <summary>
        /// The registry base key
        /// </summary>
        public const string RegistryBaseKey = RCFRegistryPaths.BasePath + @"\RCP_Metro";

        /// <summary>
        /// The license accepted value name
        /// </summary>
        public static string RegistryLicenseValue => "LicenseAccepted";

        /// <summary>
        /// The Rayman Raving Rabbids registry key path
        /// </summary>
        public static string RavingRabbidsRegistryKey = @"HKEY_CURRENT_USER\Software\Ubisoft\Rayman4\{05D2C1BC-A857-4493-9BDA-C7707CACB937}";

            /*
               RegistrynGlide = @"HKEY_CURRENT_USER\Software\Zeus Software\nGlide";
               RegistryRO = @"HKEY_CURRENT_USER\Software\Ubisoft\RaymanOrigins";
               RegistryRL = @"HKEY_CURRENT_USER\Software\Ubisoft\Rayman Legends";
             */
    }
}