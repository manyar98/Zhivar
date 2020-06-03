using OMF.Common.ExceptionManagement;
using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Common.Log;
using System;
using System.Configuration;
using System.Diagnostics;

namespace OMF.Common.Web.Configuration
{
    internal class OMFFrameworkWebSection : ConfigurationSection
    {
        private static OMFFrameworkWebSection current;

        [ConfigurationProperty("ImageCacheSettings")]
        public ImageCachingCollection ImageCacheSettings
        {
            get
            {
                return (ImageCachingCollection)this[nameof(ImageCacheSettings)];
            }
        }

        [ConfigurationProperty("HttpCompression")]
        public HttpCompressionSettings HttpCompression
        {
            get
            {
                return (HttpCompressionSettings)this[nameof(HttpCompression)];
            }
        }

        [ConfigurationProperty("ViewStateOptimizer")]
        public ViewStateOptimizerSettings ViewStateOptimizer
        {
            get
            {
                return (ViewStateOptimizerSettings)this[nameof(ViewStateOptimizer)];
            }
        }

        public static OMFFrameworkWebSection Current
        {
            get
            {
                return OMFFrameworkWebSection.current;
            }
        }

        static OMFFrameworkWebSection()
        {
            try
            {
                OMFFrameworkWebSection.current = ConfigurationManager.GetSection("OMF.Framework.Web") as OMFFrameworkWebSection;
            }
            catch (Exception ex)
            {
                EventLogHelper.Write(ExceptionManager.GetExceptionMessageWithDebugInfo((Exception)new OMFException("OMF.Framework.Web configuration exception", ex)), EventLogEntryType.Error);
            }
        }

        private OMFFrameworkWebSection()
        {
        }
    }
}
