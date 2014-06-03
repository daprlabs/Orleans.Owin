// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Orleans OWIN helper middleware.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Orleans.OwinMiddleware
{
    using System;
    using System.IO;
    using System.Reflection;

    using Orleans.Host.Azure.Client;

    using Owin;

    /// <summary>
    /// The orleans middleware.
    /// </summary>
    public static class OrleansMiddleware
    {
        /// <summary>
        /// The name of the client configuration file.
        /// </summary>
        private const string ClientConfigurationFileName = "ClientConfiguration.xml";

        /// <summary>
        /// The initialization lock.
        /// </summary>
        private static readonly object InitializationLock = new object();

        /// <summary>
        /// Configures Orleans for use with Azure.
        /// </summary>
        /// <remarks>
        /// This should appear earlier in the pipeline than any usage of Orleans.
        /// </remarks>
        /// <param name="app">
        /// The app being configured.
        /// </param>
        /// <param name="clientConfiguration">
        /// The client configuration.
        /// </param>
        public static void ConfigureOrleans(this IAppBuilder app, ClientConfiguration clientConfiguration)
        {
            app.Use(
                async (context, next) =>
                {
                    if (!OrleansAzureClient.IsInitialized)
                    {
                        lock (InitializationLock)
                        {
                            if (!OrleansAzureClient.IsInitialized)
                            {
                                OrleansAzureClient.Initialize(clientConfiguration);
                            }
                        }
                    }

                    await next.Invoke().ConfigureAwait(false);
                });
        }

        /// <summary>
        /// Configures Orleans for use with Azure.
        /// </summary>
        /// <remarks>This should appear earlier in the pipeline than any usage of Orleans.</remarks>
        /// <param name="app">The app being configured.</param>
        /// <param name="clientConfigurationFile">The client configuration file.</param>
        public static void ConfigureOrleans(this IAppBuilder app, FileInfo clientConfigurationFile)
        {
            app.Use(
                async (context, next) =>
                {
                    if (!OrleansAzureClient.IsInitialized)
                    {
                        lock (InitializationLock)
                        {
                            if (!OrleansAzureClient.IsInitialized)
                            {
                                OrleansAzureClient.Initialize(clientConfigurationFile);
                            }
                        }
                    }

                    await next.Invoke().ConfigureAwait(false);
                });
        }

        /// <summary>
        /// Configures Orleans for use with Azure.
        /// </summary>
        /// <remarks>This should appear earlier in the pipeline than any usage of Orleans.</remarks>
        /// <param name="app">The app being configured.</param>
        public static void ConfigureOrleans(this IAppBuilder app)
        {
            var path = GetAssemblyPath();
            var clientConfigFile = new FileInfo(Path.Combine(path, ClientConfigurationFileName));
            if (!clientConfigFile.Exists)
            {
                throw new FileNotFoundException(
                    string.Format("Cannot find Orleans client config file for initialization at {0}", clientConfigFile.FullName),
                    clientConfigFile.FullName);
            }
            
            app.ConfigureOrleans(clientConfigFile);
        }

        /// <summary>
        /// Returns the path of the executing assembly.
        /// </summary>
        /// <returns>The path of the executing assembly.</returns>
        private static string GetAssemblyPath()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var codeBaseUri = new UriBuilder(codeBase);
            var pathString = Uri.UnescapeDataString(codeBaseUri.Path);
            return Path.GetDirectoryName(pathString);
        }
    }
}
