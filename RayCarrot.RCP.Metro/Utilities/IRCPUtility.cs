﻿using System;
using System.Windows;

namespace RayCarrot.RCP.Metro
{
    //TODO: Finish this

    /*
     
        A utility in the Rayman Control Panel is defined by the RayCarrot.RCP.Metro.IRCPUtility interface in the API. The utility, deriving from
        the interface, must correctly implement each member. All properties are read-only and should not be cached during construction. Instead
        whenever the property is retrieved it should re-calculate the value as it may change depending on the current culture or available game.
        
        A utility is primarily made for a single game. To make it available for several games it needs to have several classes deriving from the
        same class, each with a different game which is retrieved from the Game property. These classes should not use the same files as they may
        run simultaneously.

        The UIElement should be a native WPF UIElement. No fixed size or ScrollViewer should be used. The content should be put in a StackPanel
        where the individual items wrap to avoid overflow.

        The ID for the utility should not be more than 99 characters. Ideally it should be a GUID, although it is not required as long as it is unique.

        The utility has to reference the Rayman Control Panel API. Referencing the Carrot Framework is not recommended due to the API handling the
        important framework services, such as logging and dependency injection. The utility will not load if it references the Rayman Control Panel directly.
        
         
         */

    /// <summary>
    /// Interface for a RCP utility
    /// </summary>
    public interface IRCPUtility
    {
        /// <summary>
        /// The utility ID
        /// </summary>
        string ID { get; }

        /// <summary>
        /// The header for the utility. This property is retrieved again when the current culture is changed.
        /// </summary>
        string DisplayHeader { get; }

        /// <summary>
        /// The utility information text (optional). This property is retrieved again when the current culture is changed.
        /// </summary>
        string InfoText { get; }

        /// <summary>
        /// The utility warning text (optional). This property is retrieved again when the current culture is changed.
        /// </summary>
        string WarningText { get; }

        /// <summary>
        /// The utility UI content
        /// </summary>
        UIElement UIContent { get; }

        /// <summary>
        /// Indicates if the utility requires administration privileges
        /// </summary>
        bool RequiresAdmin { get; }

        /// <summary>
        /// Indicates if the utility is available to the user
        /// </summary>
        bool IsAvailable { get; }

        /// <summary>
        /// The developers of the utility
        /// </summary>
        string Developers { get; }

        /// <summary>
        /// Any additional developers to credit for the utility
        /// </summary>
        string AdditionalDevelopers { get; }

        /// <summary>
        /// The game which the utility was made for
        /// </summary>
        Games Game { get; }

        /// <summary>
        /// Retrieves a list of applied utilities from this utility
        /// </summary>
        /// <returns>The applied utilities</returns>
        string[] GetAppliedUtilities();
    }

    public class R2TranslationUtility : IRCPUtility
    {
        /// <summary>
        /// The utility ID
        /// </summary>
        public string ID => "82622dc8-8212-40b3-b6d5-ba84678d017b";

        /// <summary>
        /// The header for the utility. This property is retrieved again when the current culture is changed.
        /// </summary>
        public string DisplayHeader => Resources.R2U_TranslationsHeader;

        /// <summary>
        /// The utility information text (optional). This property is retrieved again when the current culture is changed.
        /// </summary>
        public string InfoText => Resources.R2U_TranslationsInfo;

        /// <summary>
        /// The utility warning text (optional). This property is retrieved again when the current culture is changed.
        /// </summary>
        public string WarningText => null;

        /// <summary>
        /// The utility UI content
        /// </summary>
        public UIElement UIContent { get; }

        /// <summary>
        /// Indicates if the utility requires administration privileges
        /// </summary>
        public bool RequiresAdmin { get; }

        /// <summary>
        /// Indicates if the utility is available to the user
        /// </summary>
        public bool IsAvailable { get; }

        /// <summary>
        /// The developers of the utility
        /// </summary>
        public string Developers => "RayCarrot";

        /// <summary>
        /// Any additional developers to credit for the utility
        /// </summary>
        public string AdditionalDevelopers => "PluMGMK, Haruka Tavares, MixerX";

        /// <summary>
        /// The game which the utility was made for
        /// </summary>
        public Games Game => Games.Rayman2;

        /// <summary>
        /// Retrieves a list of applied utilities from this utility
        /// </summary>
        /// <returns>The applied utilities</returns>
        public string[] GetAppliedUtilities()
        {
            throw new NotImplementedException();
        }
    }
}