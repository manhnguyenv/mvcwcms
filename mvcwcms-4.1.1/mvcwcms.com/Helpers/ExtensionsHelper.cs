using MVCwCMS;
using MVCwCMS.Models;
using MVCwCMS.ModuleConnectors;
using MVCwCMS.Resources;
using MVCwCMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace MVCwCMS.Helpers
{
    public static class ExtensionsHelper
    {
        private static string BASE36 = "0123456789" + "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static string ToBase36(this long input)
        {
            string r = string.Empty;
            int targetBase = BASE36.Length;
            do
            {
                r = string.Format("{0}{1}",
                    BASE36[(int)(input % targetBase)],
                    r);
                input /= targetBase;
            } while (input > 0);

            return r;
        }
        public static long FromBase36(this string input)
        {
            int srcBase = BASE36.Length;
            long id = 0;
            string r = input.Reverse();

            for (int i = 0; i < r.Length; i++)
            {
                int charIndex = BASE36.IndexOf(r[i]);
                id += charIndex * (long)Math.Pow(srcBase, i);
            }

            return id;
        }

        public static string Canonical(this HtmlHelper html, string path = null)
        {
            if (path.IsEmptyOrWhiteSpace())
            {
                Uri rawUrl = html.ViewContext.RequestContext.HttpContext.Request.Url;
                path = string.Format("{0}://{1}{2}", rawUrl.Scheme, rawUrl.Host, rawUrl.AbsolutePath);
            }

            path = path.ToLower();

            if (path.Count(c => c == '/') > 3)
            {
                path = path.TrimEnd('/');
            }

            if (path.EndsWith("/index"))
            {
                path = path.Substring(0, path.Length - 6);
            }

            return path;
        }

        public static List<SelectListItem> GetCmsModules(bool includeContentModule)
        {
            List<SelectListItem> result = new List<SelectListItem>();

            //Adds all the modules defined in ModuleConnectors
            //These modules are dynamically built and their behaviour is editable at runtime.
            //E.g. Shared contents or Media Galleries
            foreach (IModuleConnector moduleConnector in ModuleConnectorsHelper.GetModuleConnectors())
            {
                result.AddRange(moduleConnector.GetSelectItemList());
            }

            //Adds the FrontEnd modules
            //These modules are pre-built and their behaviour is not editable at runtime
            //E.g. FrontEndContactController or FrontEndBreadCrumbsController
            SelectListItem sli;
            foreach (ControllerAction ca in ControllerHelper.GetAllControllersActions())
            {
                sli = new SelectListItem();
                sli.Text = ca.ControllerActionName;
                sli.Value = ca.ControllerActionID;
                result.Add(sli);
            }

            //Adds the content module only if requested
            if (includeContentModule)
                result.Insert(0, (new SelectListItem { Text = "Content", Value = "{$Content}" }));

            //Returns the result in alphabetical order
            return result.OrderBy(i => i.Text).ToList();
        }

        public static List<string> GetHtmlCodeParts(string htmlCode)
        {
            List<string> result = new List<string>();
            int cursor_pos = 0, start_index, end_index, length;

            end_index = htmlCode.IndexOf("{$", cursor_pos);
            if (end_index == -1)
                end_index = htmlCode.Length;
            length = end_index;
            if (htmlCode.Substring(cursor_pos, length).IsNotEmptyOrWhiteSpace())
                result.Add(htmlCode.Substring(cursor_pos, length));
            cursor_pos = end_index;

            do
            {
                start_index = htmlCode.IndexOf("{$", cursor_pos);
                if (start_index != -1)
                {
                    end_index = htmlCode.IndexOf("}", start_index);
                    length = end_index - start_index + 1;
                    if (htmlCode.Substring(start_index, length).IsNotEmptyOrWhiteSpace())
                        result.Add(htmlCode.Substring(start_index, length));
                    cursor_pos = end_index;

                    end_index = htmlCode.IndexOf("{$", cursor_pos);
                    if (end_index == -1)
                        end_index = htmlCode.Length;
                    length = end_index - cursor_pos - 1;
                    if (htmlCode.Substring(cursor_pos + 1, length).IsNotEmptyOrWhiteSpace())
                        result.Add(htmlCode.Substring(cursor_pos + 1, length));
                    cursor_pos = end_index;
                }
            } while (start_index != -1);

            return result;
        }

        /// <summary>
        /// Converts a DataTable to a List&lt;TSource&gt;
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static List<TSource> ToList<TSource>(this DataTable dataTable, bool replaceResourceKeys)
        {
            EnumerableRowCollection<DataRow> EnumerableRowCollectionDataRowObj = dataTable.AsEnumerable();
            List<TSource> ListResult = new List<TSource>();
            if (EnumerableRowCollectionDataRowObj != null)
            {
                string dtValueOriginal;
                TSource ResultTemp;
                foreach (DataRow dr in EnumerableRowCollectionDataRowObj)
                {
                    ResultTemp = Activator.CreateInstance<TSource>();
                    foreach (var prop in ResultTemp.GetType().GetProperties())
                    {
                        if (dr.Table.Columns.Contains(prop.Name))
                        {
                            var dtValue = dr[prop.Name] is DBNull ? (prop.PropertyType == typeof(string) ? string.Empty : null) : dr[prop.Name];

                            if (dtValue.IsNotNull() && dtValue.GetType() == typeof(string))
                            {
                                dtValue = (dtValue as string).Trim();
                            }

                            //To support the multilingual records it is necessary to use {#itemvalue} in the DB records
                            if (dtValue.IsNotNull() && replaceResourceKeys && dtValue.AsString().StartsWith("{#") && dtValue.AsString().EndsWith("}"))
                            {
                                dtValueOriginal = dtValue.AsString();
                                //dtValue = Strings.ResourceManager.GetString(dtValue.AsString().Substring(2, dtValue.AsString().Length - 3));
                                dtValue = dtValue.AsString().Substring(2, dtValue.AsString().Length - 3).GetStringFromResource();

                                //If the Resource file lookup returns empty then display the original key value
                                if (dtValue.AsString().IsEmptyOrWhiteSpace())
                                {
                                    dtValue = dtValueOriginal;
                                }
                            }
                            ResultTemp.SetPropertyValue(prop.Name, dtValue);
                        }
                    }
                    ListResult.Add(ResultTemp);
                }
            }
            return ListResult;
        }

        /// <summary>
        /// Returns the value of the specified string resource key
        /// </summary>
        /// <param name="stringResourceKey">The string resource key</param>
        /// <returns></returns>
        public static string GetStringFromResource(this string stringResourceKey)
        {
            string result = string.Empty;

            //Resource.Strings look up
            result = Strings.ResourceManager.GetString(stringResourceKey);
            if (result.IsEmptyOrWhiteSpace())
            {
                ResourceManager resourceManager;

                //Look up all the other resources
                List<Type> resourceSources = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Namespace == "MVCwCMS.Resources" && t.Name != "Strings").ToList<Type>();
                foreach (Type resourceSource in resourceSources)
                {
                    resourceManager = new ResourceManager(resourceSource);
                    if (resourceManager.IsNotNull())
                    {
                        result = resourceManager.GetString(stringResourceKey);
                        if (result.IsNotEmptyOrWhiteSpace())
                            break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceGlobalTokens(this string str)
        {
            string result = str;

            GlobalConfiguration globalConfiguration = new GlobalConfigurations().GetGlobalConfiguration();

            result = result.ReplaceWith(@"{\$DomainName}", globalConfiguration.DomainName, RegexOptions.IgnoreCase);
            result = result.ReplaceWith(@"{\$SiteName}", globalConfiguration.SiteName, RegexOptions.IgnoreCase);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string StripHtml(this string str)
        {
            return Regex.Replace(str, @"<(.|\n)*?>", string.Empty);
        }

        /// <summary>
        /// Extension method to translate 8bit characters into 7bit characters. E.g. to convert higher bit ascii characters (like, Ü which is extended ascii 154) into U (which is ascii 85).
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToAscii(this string str)
        {
            StringBuilder newStringBuilder = new StringBuilder();
            newStringBuilder.Append(str.Normalize(NormalizationForm.FormKD).Where(x => x < 128).ToArray());
            return newStringBuilder.ToString();
        }

        public static string ToDateString(this DateTime dateTime)
        {
            GlobalConfiguration globalConfiguration = new GlobalConfigurations().GetGlobalConfiguration();

            return dateTime.ToString(globalConfiguration.DateFormat, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts the specified string representation of a date (or a date time) to its DateTime equivalent using the Global Configuration Date Format and Time Format
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime? ToDateTime(this string date)
        {
            DateTime? result = null;

            GlobalConfiguration globalConfiguration = new GlobalConfigurations().GetGlobalConfiguration();

            string format = globalConfiguration.DateFormat;
            if (date.IfEmpty("").Length > 10)
            {
                format = globalConfiguration.DateFormat + " " + globalConfiguration.TimeFormat;
            }

            DateTime tempResult;
            if (DateTime.TryParseExact(date, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out tempResult))
            {
                result = tempResult;
            }

            return result;
        }

        public static string ToDateTimeString(this DateTime dateTime)
        {
            GlobalConfiguration globalConfiguration = new GlobalConfigurations().GetGlobalConfiguration();

            return dateTime.ToString(globalConfiguration.DateFormat + " " + globalConfiguration.TimeFormat, System.Globalization.CultureInfo.InvariantCulture);
        }

        public static string ToDateTimeString(this DateTime? dateTime)
        {
            string result = string.Empty;

            if (dateTime.IsNotNull())
            {
                result = ToDateTimeString((DateTime)dateTime);
            }

            return result;
        }

        public static decimal? ToFormattedDecimal(this decimal value, int decimalDigits = 2)
        {
            //return value.ToString("0.00").ConvertTo<decimal?>();
            return decimal.Round(value, decimalDigits, MidpointRounding.AwayFromZero);
        }

        public static decimal? ToFormattedDecimal(this decimal? value, int decimalDigits = 2)
        {
            decimal? result = null;

            if (value.IsNotNull())
            {
                result = ((decimal)value).ToFormattedDecimal(decimalDigits);
            }

            return result;
        }

        /// <summary>
        /// Converts a domain name into a fully qualified Url.
        /// <para>E.g. mydomainname.com becomes http://www.mydomainname.com</para>
        /// </summary>
        /// <param name="domainName">E.g. mydomainname.com</param>
        /// <returns></returns>
        public static string ToUrl(this string domainName)
        {
            string result;

            GlobalConfiguration globalConfiguration = new GlobalConfigurations().GetGlobalConfiguration();

            UriBuilder uriBuilder = new UriBuilder(globalConfiguration.HostNameLabel + domainName);
            result = uriBuilder.Uri.AbsoluteUri;

            return result;
        }

        public static string ToFriendlyUrlDashedString(this string title)
        {
            if (title == null) return "";

            const int maxlen = 80;
            int len = title.Length;
            bool prevdash = false;
            var sb = new StringBuilder(len);
            char c;

            for (int i = 0; i < len; i++)
            {
                c = title[i];
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    sb.Append(c);
                    prevdash = false;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    // tricky way to convert to lowercase 
                    sb.Append((char)(c | 32));
                    prevdash = false;
                }
                else if (c == ' ' || c == ',' || c == '.' || c == '/' ||
                    c == '\\' || c == '-' || c == '_' || c == '=')
                {
                    if (!prevdash && sb.Length > 0)
                    {
                        sb.Append('-');
                        prevdash = true;
                    }
                }
                else if ((int)c >= 128)
                {
                    int prevlen = sb.Length;
                    sb.Append(RemapInternationalCharToAscii(c));
                    if (prevlen != sb.Length) prevdash = false;
                }
                if (i == maxlen) break;
            }

            if (prevdash)
                return sb.ToString().Substring(0, sb.Length - 1);
            else
                return sb.ToString();
        }

        public static string RemapInternationalCharToAscii(this char c)
        {
            string s = c.ToString().ToLowerInvariant();
            if ("àåáâäãåą".Contains(s))
            {
                return "a";
            }
            else if ("èéêëę".Contains(s))
            {
                return "e";
            }
            else if ("ìíîïı".Contains(s))
            {
                return "i";
            }
            else if ("òóôõöøőð".Contains(s))
            {
                return "o";
            }
            else if ("ùúûüŭů".Contains(s))
            {
                return "u";
            }
            else if ("çćčĉ".Contains(s))
            {
                return "c";
            }
            else if ("żźž".Contains(s))
            {
                return "z";
            }
            else if ("śşšŝ".Contains(s))
            {
                return "s";
            }
            else if ("ñń".Contains(s))
            {
                return "n";
            }
            else if ("ýÿ".Contains(s))
            {
                return "y";
            }
            else if ("ğĝ".Contains(s))
            {
                return "g";
            }
            else if (c == 'ř')
            {
                return "r";
            }
            else if (c == 'ł')
            {
                return "l";
            }
            else if (c == 'đ')
            {
                return "d";
            }
            else if (c == 'ß')
            {
                return "ss";
            }
            else if (c == 'Þ')
            {
                return "th";
            }
            else if (c == 'ĥ')
            {
                return "h";
            }
            else if (c == 'ĵ')
            {
                return "j";
            }
            else
            {
                return "";
            }
        }
    }
}
