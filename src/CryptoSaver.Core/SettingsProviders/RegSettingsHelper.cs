using System.Configuration;
using CryptoSaver.Core.Internals;
using Microsoft.Win32;

namespace CryptoSaver.Core.SettingsProviders
{
    /// <summary>
    ///  Stores Settings in local machine (HKLM) registry
    /// </summary>
    public class RegSettingsHelper : SettingsProviderEx.ISettingsHelper
    {
        #region Static Fields and Constants

        /// <summary>
        ///     The user root
        /// </summary>
        internal static string BaseKey = "HKEY_LOCAL_MACHINE";

        #endregion

        #region Properties & Indexers

        /// <summary>
        ///     Gets or sets the key path.
        /// </summary>
        /// <value>The key path.</value>
        private static string KeyPath
        {
            get { return BaseKey + "\\" + SubKey; }
        }

        /// <summary>
        ///     Gets or sets the sub key.
        /// </summary>
        /// <value>The sub key.</value>
        private static string SubKey
        {
            get { return "Software\\" + AssemblyInfo.CompanyName() + "\\" + AssemblyInfo.ProductName() + "\\Settings"; }
        }

        #endregion

        #region ISettingsHelper Members

        /// <summary>
        ///     Gets the specified settings property.
        /// </summary>
        /// <param name="settingsProperty">The settings property.</param>
        /// <param name="settingType">Type of the setting.</param>
        /// <returns>System.Configuration.SettingsPropertyValue.</returns>
        public SettingsPropertyValue Get(SettingsProperty settingsProperty, SettingsProviderEx.SettingType settingType)
        {
            return new SettingsPropertyValue(settingsProperty)
            {
                SerializedValue =
                    Registry.GetValue(BaseKey + "\\" + SubKey, settingsProperty.Name, settingsProperty.DefaultValue)
            };
        }

        /// <summary>
        ///     Sets the specified settings property value.
        /// </summary>
        /// <param name="settingsPropertyValue">The settings property value.</param>
        /// <param name="settingType">Type of the setting.</param>
        public void Set(SettingsPropertyValue settingsPropertyValue, SettingsProviderEx.SettingType settingType)
        {
            Registry.SetValue(KeyPath, settingsPropertyValue.Property.Name, settingsPropertyValue.SerializedValue);
        }

        /// <summary>
        ///     Settings exists.
        /// </summary>
        /// <returns>System.Boolean.</returns>
        public bool SettingsExists()
        {
            return true;
        }

        #endregion
    }
}