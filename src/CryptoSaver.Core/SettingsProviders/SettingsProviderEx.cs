using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Reflection;
using CryptoSaver.Core.Internals;
using CryptoSaver.Core.Logging;

namespace CryptoSaver.Core.SettingsProviders
{
    /// <summary>
    /// Adds an extensible settings provider
    /// </summary>
    public class SettingsProviderEx : SettingsProvider
    {
        #region SettingType enum

        /// <summary>
        ///     Enum SettingType
        /// </summary>
        public enum SettingType
        {
            App,
            User
        }

        #endregion

        #region Properties & Indexers

        /// <summary>
        ///     Gets or sets the name of the currently running application.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String" /> that contains the application's shortened name, which does not contain a full path
        ///     or extension, for example, SimpleAppSettings.
        /// </returns>
        public override string ApplicationName
        {
            get
            {
                return AssemblyInfo.ProductName();
            }
            set { }
        }

        private ISettingsHelper _helper;

        private ISettingsHelper CurrentHelper
        {
            get
            {
                if (_helper != null)
                    return _helper;
                foreach (var type in GetTypesWithHelpAttribute(AssemblyInfo.EntryAssembly))
                {
                    var results = type.GetCustomAttributes(typeof(SettingsHelperAttribute));
                    foreach (var attribute1 in results)
                    {
                        var attribute = (SettingsHelperAttribute)attribute1;
                        _helper = Activator.CreateInstance(attribute.Helper);
                        this.Log().Info("Log helper [" + _helper.GetType().Name + "] found");
                        return _helper;
                    }
                }
                this.Log().Warn("No log helper given using default Reg Current User helper");
                return new RegCurrentUserHelper();
            }
        }
        #endregion

        /// <summary>
        ///     Initializes the provider.
        /// </summary>
        /// <param name="name">The friendly name of the provider.</param>
        /// <param name="config">
        ///     A collection of the name/value pairs representing the provider-specific attributes specified in
        ///     the configuration for this provider.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">The name of the provider is null.</exception>
        /// <exception cref="T:System.ArgumentException">The name of the provider has a length of zero.</exception>
        /// <exception cref="T:System.InvalidOperationException">
        ///     An attempt is made to call
        ///     <see
        ///         cref="M:System.Configuration.Provider.ProviderBase.Initialize(System.String,System.Collections.Specialized.NameValueCollection)" />
        ///     on a provider after the provider has already been initialized.
        /// </exception>
        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(ApplicationName, config);
        }

        /// <summary>
        ///     Returns the collection of settings property values for the specified application instance and settings property
        ///     group.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.Configuration.SettingsPropertyValueCollection" /> containing the values for the specified
        ///     settings property group.
        /// </returns>
        /// <param name="context">A <see cref="T:System.Configuration.SettingsContext" /> describing the current application use.</param>
        /// <param name="collection">
        ///     A <see cref="T:System.Configuration.SettingsPropertyCollection" /> containing the settings
        ///     property group whose values are to be retrieved.
        /// </param>
        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context,
            SettingsPropertyCollection collection)
        {
            var propertyValueCollection = new SettingsPropertyValueCollection();

            if (CurrentHelper.SettingsExists())
                GetValues(collection, propertyValueCollection);
            else
                GetDefaultValues(collection, propertyValueCollection);

            return propertyValueCollection;
        }

        private void GetValues(SettingsPropertyCollection collection, SettingsPropertyValueCollection propertyValueCollection)
        {
            foreach (SettingsProperty property in collection)
            {
                propertyValueCollection.Add(CurrentHelper.Get(property, GetSettingType(property)));
            }
        }

        private void GetDefaultValues(SettingsPropertyCollection collection, SettingsPropertyValueCollection propertyValueCollection)
        {
            foreach (SettingsProperty property in collection)
            {
                propertyValueCollection.Add(new SettingsPropertyValue(property) { SerializedValue = property.DefaultValue });
            }
        }

        /// <summary>
        ///     Sets the values of the specified group of property settings.
        /// </summary>
        /// <param name="context">A <see cref="T:System.Configuration.SettingsContext" /> describing the current application usage.</param>
        /// <param name="collection">
        ///     A <see cref="T:System.Configuration.SettingsPropertyValueCollection" /> representing the group
        ///     of property settings to set.
        /// </param>
        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            foreach (
                var propertyValue in
                    collection.Cast<SettingsPropertyValue>().Where(propertyValue => propertyValue.IsDirty))
            {
                CurrentHelper.Set(propertyValue, GetSettingType(propertyValue.Property));
            }
        }

        private Attribute GetCustomAttributeFromExecutingAssembly(Type attributeType)
        {
            var assembly = AssemblyInfo.EntryAssembly;
            return Attribute.GetCustomAttribute(assembly, attributeType);
        }

        private static IEnumerable<Type> GetTypesWithHelpAttribute(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                var myAtt = type.GetCustomAttributes(typeof(SettingsHelperAttribute), true);

                if (type.GetCustomAttributes(typeof(SettingsHelperAttribute), true).Length > 0)
                    yield return type;
            }
        }

        internal static SettingType GetSettingType(SettingsProperty property)
        {
            return IsAppScoped(property) ? SettingType.App : SettingType.User;
        }

        private static bool IsAppScoped(SettingsProperty settingsProperty)
        {
            return settingsProperty.Attributes[typeof(ApplicationScopedSettingAttribute)] != null;
        }

        private static bool IsUserScoped(SettingsProperty settingsProperty)
        {
            return settingsProperty.Attributes[typeof(UserScopedSettingAttribute)] != null;
        }

        #region Nested type: ISettingsHelper

        /// <summary>
        ///     Interface ISettingsHelper
        /// </summary>
        public interface ISettingsHelper
        {
            SettingsPropertyValue Get(SettingsProperty settingsProperty, SettingType settingType);

            void Set(SettingsPropertyValue settingsPropertyValue, SettingType settingType);
            bool SettingsExists();
        }

        #endregion

        #region Nested type: SettingsProviderExAttribute

        /// <summary>
        ///     Class SettingsProviderExAttribute.
        /// </summary>
        [AttributeUsage(AttributeTargets.Class)]
        public class SettingsHelperAttribute : Attribute
        {
            #region Constructors

            public SettingsHelperAttribute()
            {
                Helper = new RegCurrentUserHelper();
            }

            /// <summary>
            ///     Initializes a new instance of the <see cref="SettingsHelperAttribute" /> class.
            /// </summary>
            /// <param name="helper">The helper.</param>
            public SettingsHelperAttribute(Type helper)
            {
                if (typeof(ISettingsHelper).IsAssignableFrom(helper))
                {
                    Helper = helper;
                }
            }

            #endregion

            #region Properties & Indexers

            /// <summary>
            ///     Gets the helper.
            /// </summary>
            /// <value>The helper.</value>
            public dynamic Helper { get; private set; }

            #endregion
        }

        #endregion
    }
}
