using MVCwCMS.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace MVCwCMS.Models
{
    public class Device
    {
        public string AolVersion { get; set; }
        public string BrowserBits { get; set; }
        public string BrowserDescription { get; set; }
        public string BrowserMaker { get; set; }
        public string BrowserModus { get; set; }
        public string BrowserType { get; set; }
        public string BrowserVersion { get; set; }
        public string Comment { get; set; }
        public string CssVersion { get; set; }
        public string DeviceBrandName { get; set; }
        public string DeviceCodeName { get; set; }
        public string DeviceMaker { get; set; }
        public string DeviceName { get; set; }
        public string DevicePointingMethod { get; set; }
        public string DeviceType { get; set; }
        public bool IsActiveXControlsCapable { get; set; }
        public bool IsAlpha { get; set; }
        public bool IsBackgroundSoundsCapable { get; set; }
        public bool IsBeta { get; set; }
        public bool IsCookiesCapable { get; set; }
        public bool IsCrawler { get; set; }
        public bool IsFramesCapable { get; set; }
        public bool IsIFramesCapable { get; set; }
        public bool IsJavaAppletsCapable { get; set; }
        public bool IsJavaScriptCapable { get; set; }
        public bool IsMobileDevice { get; set; }
        public bool IsSyndicationReaderCapable { get; set; }
        public bool IsTablesCapable { get; set; }
        public bool IsTablet { get; set; }
        public bool IsVBScriptCapable { get; set; }
        public bool IsWin16 { get; set; }
        public bool IsWin32 { get; set; }
        public bool IsWin64 { get; set; }
        public string MajorVer { get; set; }
        public string MinorVer { get; set; }
        public string Parent { get; set; }
        public string Platform { get; set; }
        public string PlatformBits { get; set; }
        public string PlatformDescription { get; set; }
        public string PlatformMaker { get; set; }
        public string PlatformVersion { get; set; }
        public string RenderingEngineDescription { get; set; }
        public string RenderingEngineMaker { get; set; }
        public string RenderingEngineName { get; set; }
        public string RenderingEngineVersion { get; set; }
    }

    public class Devices
    {
        private static object ThisLock = new object();

        private BrowsCapProvider _AllItems;

        private BrowsCapProvider GetAllItems(bool force = false)
        {
            HttpContext context = HttpContext.Current;

            if (force || context.Cache["BrowsCapProvider"].IsNull()) //Double check locking
            {
                lock (ThisLock)
                {
                    if (force || context.Cache["BrowsCapProvider"].IsNull()) //Double check locking
                    {
                        context.Cache.Insert("BrowsCapProvider", new BrowsCapProvider(HttpContext.Current.Server.MapPath("~/App_Data/BrowsCapProvider.ini")));
                    }
                }
            }

            return context.Cache["BrowsCapProvider"] as BrowsCapProvider;
        }

        public Devices()
        {
            _AllItems = GetAllItems();
        }

        public Device GetDeviceByUserAgent(string userAgent)
        {
            Device result = new Device();

            if (_AllItems.IsNotNull())
            {
                NameValueCollection nameValueCollection = _AllItems.GetValues(userAgent);

                if (nameValueCollection.IsNotNull())
                {
                    result.AolVersion = nameValueCollection["Aol_Version"];
                    result.BrowserBits = nameValueCollection["Browser_Bits"];
                    result.BrowserDescription = nameValueCollection["Browser"];
                    result.BrowserMaker = nameValueCollection["Browser_Maker"];
                    result.BrowserModus = nameValueCollection["Browser_Modus"];
                    result.BrowserType = nameValueCollection["Browser_Type"];
                    result.BrowserVersion = nameValueCollection["Version"];
                    result.Comment = nameValueCollection["Comment"];
                    result.CssVersion = nameValueCollection["CssVersion"];
                    result.DeviceBrandName = nameValueCollection["Device_Brand_Name"];
                    result.DeviceCodeName = nameValueCollection["Device_Code_Name"];
                    result.DeviceMaker = nameValueCollection["Device_Maker"];
                    result.DeviceName = nameValueCollection["Device_Name"];
                    result.DevicePointingMethod = nameValueCollection["Device_Pointing_Method"];
                    result.DeviceType = nameValueCollection["Device_Type"];
                    result.IsActiveXControlsCapable = nameValueCollection["ActiveXControls"].ConvertTo<bool>(false, true);
                    result.IsAlpha = nameValueCollection["Alpha"].ConvertTo<bool>(false, true);
                    result.IsBackgroundSoundsCapable = nameValueCollection["BackgroundSounds"].ConvertTo<bool>(false, true);
                    result.IsBeta = nameValueCollection["Beta"].ConvertTo<bool>(false, true);
                    result.IsCookiesCapable = nameValueCollection["Cookies"].ConvertTo<bool>(false, true);
                    result.IsCrawler = nameValueCollection["Crawler"].ConvertTo<bool>(false, true);
                    result.IsFramesCapable = nameValueCollection["Frames"].ConvertTo<bool>(false, true);
                    result.IsIFramesCapable = nameValueCollection["IFrames"].ConvertTo<bool>(false, true);
                    result.IsJavaAppletsCapable = nameValueCollection["JavaApplets"].ConvertTo<bool>(false, true);
                    result.IsJavaScriptCapable = nameValueCollection["JavaScript"].ConvertTo<bool>(false, true);
                    result.IsMobileDevice = nameValueCollection["IsMobileDevice"].ConvertTo<bool>(false, true);
                    result.IsSyndicationReaderCapable = nameValueCollection["IsSyndicationReader"].ConvertTo<bool>(false, true);
                    result.IsTablesCapable = nameValueCollection["Tables"].ConvertTo<bool>(false, true);
                    result.IsTablet = nameValueCollection["IsTablet"].ConvertTo<bool>(false, true);
                    result.IsVBScriptCapable = nameValueCollection["VBScript"].ConvertTo<bool>(false, true);
                    result.IsWin16 = nameValueCollection["Win16"].ConvertTo<bool>(false, true);
                    result.IsWin32 = nameValueCollection["Win32"].ConvertTo<bool>(false, true);
                    result.IsWin64 = nameValueCollection["Win64"].ConvertTo<bool>(false, true);
                    result.MajorVer = nameValueCollection["MajorVer"];
                    result.MinorVer = nameValueCollection["MinorVer"];
                    result.Parent = nameValueCollection["Parent"];
                    result.Platform = nameValueCollection["Platform"];
                    result.PlatformBits = nameValueCollection["Platform_Bits"];
                    result.PlatformDescription = nameValueCollection["Platform_Description"];
                    result.PlatformMaker = nameValueCollection["Platform_Maker"];
                    result.PlatformVersion = nameValueCollection["Platform_Version"];
                    result.RenderingEngineDescription = nameValueCollection["Rendering_Engine_Description"];
                    result.RenderingEngineMaker = nameValueCollection["Rendering_Engine_Maker"];
                    result.RenderingEngineName = nameValueCollection["Rendering_Engine_Name"];
                    result.RenderingEngineVersion = nameValueCollection["Rendering_Engine_Version"];
                }
            }

            return result;
        }
    }
}