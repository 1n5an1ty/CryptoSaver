namespace CryptoSaver.Core.Internals
{
    /// <summary>
    /// Enum Application ShutdownCodes
    /// </summary>
    public enum ShutdownCodes
    {
        /// <summary>
        /// The unknown argument
        /// </summary>
        UnknownArgument = 101,
        /// <summary>
        /// No configuration window identified
        /// </summary>
        NoConfigWindow = 102,
        /// <summary>
        /// Restarting elevated
        /// </summary>
        RestartingElevated = 107,
        /// <summary>
        /// Cool down mode, used in combination with auto restart to ward against error loops
        /// </summary>
        CoolDown = 110,
        /// <summary>
        /// Installation failure
        /// </summary>
        InstallationFailure = 123,
        /// <summary>
        /// Unknown error
        /// </summary>
        UnknownError = 666
    }
}
