using System;
using System.IO;
using System.Reflection;
using CryptoSaver.Core.Logging;

namespace CryptoSaver.Core.Internals
{
    /// <summary>
    /// Class FileName.
    /// </summary>
    internal static class FileName
    {
        private static readonly Type This = typeof(FileName);
        /// <summary>
        /// Renames to executable.
        /// </summary>
        /// <returns>System.String.</returns>
        public static string RenameToExe()
        {
            return ChangeExtension(Assembly.GetEntryAssembly().Location, ".exe");
        }

        /// <summary>
        /// Renames to SCR.
        /// </summary>
        /// <returns>System.String.</returns>
        public static string RenameToScr()
        {
            return ChangeExtension(Assembly.GetEntryAssembly().Location, ".scr");
        }

        /// <summary>
        /// Changes the extension.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="newExtension">The new extension.</param>
        /// <returns>System.String.</returns>
        private static string ChangeExtension(string filePath, string newExtension)
        {
            This.Log().Debug("Changeing " + filePath + " extension to " + newExtension);
            if (Path.GetExtension(filePath) == newExtension) return filePath;

            var newFilePath = Path.ChangeExtension(filePath, newExtension);

            if (File.Exists(newFilePath))
            {
                try
                {
                    File.Delete(newFilePath);
                }
                catch (Exception e)
                {
                    This.Log().Error("Error while changing file extension", e);
                }
            }

            if (!File.Exists(newFilePath))
            {
                File.Move(filePath, newFilePath);
            }
            return newFilePath;
        }
    }
}