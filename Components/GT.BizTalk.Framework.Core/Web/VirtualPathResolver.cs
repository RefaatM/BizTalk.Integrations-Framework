using System;
using System.Web;

namespace GT.BizTalk.Framework.Core.Web
{
    /// <summary>
    /// Provides utility methods for common virtual path resolution.
    /// </summary>
    public static class VirtualPathResolver
    {
        #region Methods

        #region ResolveUrl

        /// <summary>
        /// Returns a site relative HTTP path from a partial path starting out with a ~.
        /// Same syntax that ASP.Net internally supports but this method can be used
        /// outside of the Page framework.
        ///
        /// Works like Control.ResolveUrl including support for ~ syntax
        /// but returns an absolute URL.
        /// </summary>
        /// <param name="originalUrl">Any Url including those starting with ~</param>
        /// <returns>relative url</returns>
        public static string ResolveUrl(string originalUrl)
        {
            if (string.IsNullOrEmpty(originalUrl))
                return originalUrl;

            // absolute path - just return
            if (VirtualPathResolver.IsAbsoluteUrl(originalUrl))
                return originalUrl;

            // we don't start with the '~' -> we don't process the Url
            if (!originalUrl.StartsWith("~"))
                return originalUrl;

            // fix up path for ~ root app dir directory
            // VirtualPathUtility blows up if there is a
            // query string, so we have to account for this.
            int queryStringStartIndex = originalUrl.IndexOf('?');
            if (queryStringStartIndex != -1)
            {
                string queryString = originalUrl.Substring(queryStringStartIndex);
                string baseUrl = originalUrl.Substring(0, queryStringStartIndex);

                return string.Concat(
                    VirtualPathUtility.ToAbsolute(baseUrl),
                    queryString);
            }
            else
            {
                return VirtualPathUtility.ToAbsolute(originalUrl);
            }
        }

        /// <summary>
        /// This method returns a fully qualified absolute server Url which includes
        /// the protocol, server, port in addition to the server relative Url.
        ///
        /// Works like Control.ResolveUrl including support for ~ syntax
        /// but returns an absolute URL.
        /// </summary>
        /// <param name="ServerUrl">Any Url, either App relative or fully qualified</param>
        /// <param name="forceHttps">if true forces the url to use https</param>
        /// <returns></returns>
        public static string ResolveServerUrl(string serverUrl, bool forceHttps)
        {
            if (string.IsNullOrEmpty(serverUrl))
                return serverUrl;

            // is it already an absolute Url?
            if (VirtualPathResolver.IsAbsoluteUrl(serverUrl))
                return serverUrl;

            string newServerUrl = VirtualPathResolver.ResolveUrl(serverUrl);
            Uri result = new Uri(HttpContext.Current.Request.Url, newServerUrl);

            if (!forceHttps)
                return result.ToString();
            else
                return VirtualPathResolver.ForceUriToHttps(result).ToString();
        }

        /// <summary>
        /// This method returns a fully qualified absolute server Url which includes
        /// the protocol, server, port in addition to the server relative Url.
        ///
        /// Works like Page.ResolveUrl, but adds these to the beginning.
        /// This method is useful for generating Urls for AJAX methods
        /// </summary>
        /// <param name="ServerUrl">Any Url, either App relative or fully qualified</param>
        /// <returns></returns>
        public static string ResolveServerUrl(string serverUrl)
        {
            return VirtualPathResolver.ResolveServerUrl(serverUrl, false);
        }

        #endregion ResolveUrl

        #region MapPath

        /// <summary>
        /// Returns a physical path that corresponds to the specified virtual path on the Web server.
        /// </summary>
        /// <param name="path">The virtual path of the Web server.</param>
        /// <returns>The physical file path that corresponds to path.</returns>
        public static string MapPath(string path)
        {
            if (string.IsNullOrEmpty(path) == true)
                return path;

            // check if the path is virtual
            if (path.StartsWith("~/") == true)
            {
                if (HttpContext.Current != null)
                    return HttpContext.Current.Server.MapPath(path);
                else
                    return path.Replace("~/", VirtualPathResolver.GetApplicationPath());
            }
            else
            {
                // check if the path is absolute
                if (System.IO.Path.IsPathRooted(path) == true || VirtualPathResolver.IsAbsoluteUrl(path) == true)
                {
                    return path;
                }
                else
                {
                    if (HttpContext.Current != null)
                    {
                        try
                        {
                            return HttpContext.Current.Server.MapPath(path);
                        }
                        catch (HttpException)
                        {
                            // this could happen if the path was outside the web application;
                            // in this case continue below to try mapping it as a standard
                            // path
                        }
                    }

                    // if we are here is either because there is no HttpContext (not a web application)
                    // or because the path could not be mapped (e.g.: path was outside the web application)
                    string root = VirtualPathResolver.GetApplicationPath();
                    return System.IO.Path.GetFullPath(System.IO.Path.Combine(root, path));
                }
            }
        }

        #endregion MapPath

        #region ApplicationPath

        /// <summary>
        /// Gets the physical drive path of the application directory for the application
        /// hosted in the current application domain.
        /// </summary>
        /// <returns>The physical drive path of the application directory for the application
        /// hosted in the current application domain.</returns>
        public static string GetApplicationPath()
        {
            // check if this is a web application to know how we will get the app path
            if (HttpContext.Current != null)
                return HttpRuntime.AppDomainAppPath;
            else
                return AppDomain.CurrentDomain.BaseDirectory;
        }

        #endregion ApplicationPath

        #region Utility Methods

        /// <summary>
        /// Checks if the specified url is absolute.
        /// </summary>
        /// <param name="url">Url to check.</param>
        /// <returns><b>true</b> if the url is absolute; otherwise <b>false</b>.</true></returns>
        private static bool IsAbsoluteUrl(string url)
        {
            // absolute path - just return
            int indexOfSlashes = url.IndexOf("://");
            int indexOfQuestionMarks = url.IndexOf("?");
            if (indexOfSlashes > -1 &&
                (indexOfQuestionMarks < 0 || (indexOfQuestionMarks > -1 && indexOfQuestionMarks > indexOfSlashes)))
                return true;
            return false;
        }

        /// <summary>
        /// Cleans the specified path as needed.
        /// </summary>
        /// <param name="path">The path to clean.</param>
        /// <returns>A string containing the cleaned path value.</returns>
        public static string CleanPath(string path)
        {
            // Strip any trailing slash from the path.
            if (path.EndsWith("/"))
                return path.Substring(0, path.Length - 1);
            else
                return path;
        }

        #endregion Utility Methods

        #endregion Methods

        #region Private Helpers

        /// <summary>
        /// Forces the Uri to use https
        /// </summary>
        private static Uri ForceUriToHttps(Uri uri)
        {
            // ** Re-write Url using builder.
            UriBuilder builder = new UriBuilder(uri);
            builder.Scheme = Uri.UriSchemeHttps;
            builder.Port = 443;
            return builder.Uri;
        }

        #endregion Private Helpers
    }
}