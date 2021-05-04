namespace CryptoSaver.Core.SettingsProviders
{
    /// <summary>
    /// Stores Settings in current user (HKCU) registry
    /// </summary>
    public class RegCurrentUserHelper : RegSettingsHelper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegCurrentUserHelper"/> class.
        /// </summary>
        public RegCurrentUserHelper()
        {
            BaseKey = "HKEY_CURRENT_USER";
        }
    }
}