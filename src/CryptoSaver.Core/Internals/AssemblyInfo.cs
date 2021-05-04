using System.Linq;
using System.Reflection;

namespace CryptoSaver.Core.Internals
{
    /// <summary>
    /// Class AssemblyInfo gets information about the entry assembly.
    /// </summary>
    public static class AssemblyInfo
    {
        static AssemblyInfo()
        {
            EntryAssembly = Assembly.GetEntryAssembly();
        }

        /// <summary>
        /// The this
        /// </summary>
        internal static readonly Assembly EntryAssembly;

        /// <summary>
        /// Gets the name of the product.
        /// </summary>
        /// <value>The name of the product.</value>
        public static string ProductName(Assembly assembly = null)
        {
            var workAssembly = assembly ?? EntryAssembly;

            var atts = workAssembly.GetCustomAttributes(typeof(AssemblyProductAttribute));
            return ((AssemblyProductAttribute)atts.First()).Product;
        }

        /// <summary>
        /// Gets the name of the company.
        /// </summary>
        /// <value>The name of the company.</value>
        public static string CompanyName(Assembly assembly = null)
        {
            var workAssembly = assembly ?? EntryAssembly;

            var atts = workAssembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute));
            var company = ((AssemblyCompanyAttribute)atts.First()).Company;
            return string.IsNullOrEmpty(company) ? ProductName() : company;
        }
    }
}
