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
    public static class FrontEndSessions
    {
        public static Subscription CurrentSubscription
        {
            get
            {
                if (HttpContext.Current.Session["FrontEnd_CurrentSubscription"] != null)
                    return HttpContext.Current.Session["FrontEnd_CurrentSubscription"] as Subscription;
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["FrontEnd_CurrentSubscription"] = value;
            }
        }
    }
}