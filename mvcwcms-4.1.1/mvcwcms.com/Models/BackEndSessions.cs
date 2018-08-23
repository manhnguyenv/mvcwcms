using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Net;
using System.Web.Routing;
using System.Resources;
using System.Globalization;
using System.Collections;
using System.Configuration;

namespace MVCwCMS.Models
{
    public static class BackEndSessions
    {
        public static string CurrentMenu
        {
            get
            {
                if (HttpContext.Current.Session["BackEnd_CurrentMenu"] != null)
                    return HttpContext.Current.Session["BackEnd_CurrentMenu"] as string;
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["BackEnd_CurrentMenu"] = value;
            }
        }

        public static User CurrentUser
        {
            get
            {
                if (HttpContext.Current.Session["BackEnd_CurrentUser"] != null)
                    return HttpContext.Current.Session["BackEnd_CurrentUser"] as User;
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["BackEnd_CurrentUser"] = value;
            }
        }
    }
}