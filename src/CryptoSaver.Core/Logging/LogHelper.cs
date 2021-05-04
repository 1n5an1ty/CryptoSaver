#region Header

// Developer(s) : rgleonard
// Modified : 10-27-2015 11:16 AM
// Created : 10-27-2015 7:28 AM
// Solution : ScreenSaver.Helper
// Project : ScreenSaver.Helper
// File : LogHelper.cs

#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace CryptoSaver.Core.Logging
{
    /// <summary>
    /// Class LogHelper provides log4net helper methods 
    /// </summary>
    public static class LogHelper
    {
        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="LogHelper" /> class.
        /// </summary>
        static LogHelper()
        {
            Root = ((Hierarchy)LogManager.GetRepository()).Root;

            var lvl = Level.Warn;

            if (Debugger.IsAttached)
            {
                lvl = Level.All;
                Root.AddAppender(GetConsoleAppender());
            }
            AddRollingFileAppender(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\" +
                             Internals.AssemblyInfo.ProductName() + "\\" + Internals.AssemblyInfo.ProductName() +
                             "_log.xml"), lvl);
            Root.Repository.Configured = true;
        }

        #endregion

        #region Static Fields and Constants

        /// <summary>
        /// The root logger object
        /// </summary>
        private static readonly Logger Root;

        /// <summary>
        /// The properties dictionary
        /// </summary>
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> Props =
                    new ConcurrentDictionary<Type, PropertyInfo[]>();

        #endregion

        /// <summary>
        /// Logs the specified sender class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="senderClass">The sender class.</param>
        /// <returns>log4net.ILog.</returns>
        public static ILog Log<T>(this T senderClass) where T : class
        {
            return LogManager.GetLogger(senderClass.GetType());
        }

        /// <summary>
        /// Logs the specified sender class with its public properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="senderClass">The sender class.</param>
        /// <returns>log4net.ILog.</returns>
        public static ILog LogP<T>(this T senderClass) where T : class
        {
            ThreadContext.Properties.Clear();
            foreach (
                var propertyInfo in GetPropertyInfos(senderClass.GetType()).Where(propertyInfo => propertyInfo.CanRead))
            {
                ThreadContext.Properties[propertyInfo.Name] = propertyInfo.GetValue(senderClass);
            }
            return LogManager.GetLogger(senderClass.GetType());
        }

        /// <summary>
        /// Logs the specified sender class with specific public property names
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="senderClass">The sender class.</param>
        /// <param name="properties">The properties.</param>
        /// <returns>log4net.ILog.</returns>
        public static ILog LogP<T>(this T senderClass, IEnumerable<string> properties) where T : class
        {
            ThreadContext.Properties.Clear();
            foreach (
                var propertyInfo in
                    GetPropertyInfos(senderClass.GetType())
                        .Where(propertyInfo => propertyInfo.CanRead && properties.Contains(propertyInfo.Name)))
            {
                ThreadContext.Properties[propertyInfo.Name] = propertyInfo.GetValue(senderClass);
            }
            return LogManager.GetLogger(senderClass.GetType());
        }

        /// <summary>
        /// Logs the specified sender class with specific public properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="senderClass">The sender class.</param>
        /// <param name="properties">The properties.</param>
        /// <returns>log4net.ILog.</returns>
        public static ILog LogP<T>(this T senderClass, IEnumerable<PropertyInfo> properties) where T : class
        {
            ThreadContext.Properties.Clear();
            foreach (
                var propertyInfo in
                    GetPropertyInfos(senderClass.GetType())
                        .Where(propertyInfo => propertyInfo.CanRead && properties.Contains(propertyInfo)))
            {
                ThreadContext.Properties[propertyInfo.Name] = propertyInfo.GetValue(senderClass);
            }
            return LogManager.GetLogger(senderClass.GetType());
        }

        /// <summary>
        /// Adds the specified appender.
        /// </summary>
        /// <param name="appender">The appender.</param>
        public static void AddAppender(IAppender appender)
        {
            Root.AddAppender(appender);
        }

        /// <summary>
        /// Adds the rolling file appender.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="threshold">The threshold.</param>
        /// <returns>log4net.Appender.RollingFileAppender.</returns>
        public static RollingFileAppender AddRollingFileAppender(string fileName, Level threshold)
        {
            var appender = GetRollingFileAppender(fileName, threshold);
            Root.AddAppender(appender);
            return appender;
        }

        /// <summary>
        /// Gets the console appender.
        /// </summary>
        /// <returns>log4net.Appender.ConsoleAppender.</returns>
        private static ConsoleAppender GetConsoleAppender()
        {
            var lAppender = new ConsoleAppender
            {
                Name = "Console",
                Layout = new
                    PatternLayout("LOG  >>  %message %n"),
                Threshold = Level.All
            };

            lAppender.ActivateOptions();
            return lAppender;
        }

        /// <summary>
        /// Gets the rolling file appender.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="threshold">The threshold.</param>
        /// <returns>log4net.Appender.RollingFileAppender.</returns>
        private static RollingFileAppender GetRollingFileAppender(string fileName, Level threshold)
        {
            var rolling = new RollingFileAppender
            {
                PreserveLogFileNameExtension = true,
                File = fileName,
                AppendToFile = true,
                DatePattern = "yyyy-MM-dd",
                MaxSizeRollBackups = 3,
                RollingStyle = RollingFileAppender.RollingMode.Date,
                Layout = new XmlLayout(true),
                Threshold = threshold
            };

            rolling.ActivateOptions();
            return rolling;
        }

        /// <summary>
        /// Gets the property infos.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.Collections.Generic.IEnumerable&lt;System.Reflection.PropertyInfo&gt;.</returns>
        private static IEnumerable<PropertyInfo> GetPropertyInfos(Type type)
        {
            if (Props.ContainsKey(type))
                return Props[type];

            return Props.AddOrUpdate(type,
                type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                (key, existingvalue) =>
                    type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
        }
    }
}