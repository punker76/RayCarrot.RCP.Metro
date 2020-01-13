﻿using System;

namespace RayCarrot.RCP.Metro
{
    /// <summary>
    /// View model for an enum selection
    /// </summary>
    /// <typeparam name="T">The type of enum to select</typeparam>
    public class EnumSelectionViewModel<T> : BaseRCPViewModel
        where T : Enum
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="defaultValue">The default selected value</param>
        /// <param name="availableValues">The available values to select</param>
        public EnumSelectionViewModel(T defaultValue, T[] availableValues)
        {
            AvailableValues = availableValues;
            SelectedValue = defaultValue;
        }

        /// <summary>
        /// The available values to select
        /// </summary>
        public T[] AvailableValues { get; }

        /// <summary>
        /// The currently selected value
        /// </summary>
        public T SelectedValue { get; set; }
    }
}