using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCwCMS.Helpers
{
    public class FormHelper
    {
        
        /// <summary>
        /// Returns a SelectList object to use in a DropDownListFor containing the options for the Admin Language Codes
        /// </summary>
        /// <param name="selectedValue"></param>
        /// <returns></returns>
        public static SelectListItem[] GetSelectListForAdminLanguageCodes(SelectedValueForAdminLanguageCodes? selectedValue = null)
        {
            return new[] {
                        new SelectListItem { Value = "EN", Text = "English", Selected = (selectedValue == SelectedValueForAdminLanguageCodes.English) },
                        new SelectListItem { Value = "IT", Text = "Italian", Selected = (selectedValue == SelectedValueForAdminLanguageCodes.Italian) }
                    };
        }
        public enum SelectedValueForAdminLanguageCodes
        {
            English,
            Italian
        }

        /// <summary>
        /// Returns a SelectList object to use in a DropDownListFor containing the options for the Date format
        /// </summary>
        /// <param name="selectedValue"></param>
        /// <returns></returns>
        public static SelectListItem[] GetSelectListForDateFormat(SelectedValueForDateFormat? selectedValue = null)
        {
            return new[] {
                        new SelectListItem { Value = "dd/MM/yyyy", Text = "28/01/2014", Selected = (selectedValue == SelectedValueForDateFormat.DDMMYYYY) },
                        new SelectListItem { Value = "MM/dd/yyyy", Text = "01/28/2014", Selected = (selectedValue == SelectedValueForDateFormat.MMDDYYYY) }
                    };
        }
        public enum SelectedValueForDateFormat
        {
            DDMMYYYY,
            MMDDYYYY
        }

        /// <summary>
        /// Returns a SelectList object to use in a DropDownListFor containing the options for the Host Name Label
        /// </summary>
        /// <param name="selectedValue"></param>
        /// <returns></returns>
        public static SelectListItem[] GetSelectListForHostNameLabel(SelectedValueForHostNameLabel? selectedValue = null)
        {
            return new[] {
                        new SelectListItem { Value = "www.", Text = "www.", Selected = (selectedValue == SelectedValueForHostNameLabel.WWW) }
                    };
        }
        public enum SelectedValueForHostNameLabel
        {
            WWW
        }

        /// <summary>
        /// Returns a SelectList object to use in a DropDownListFor containing the options for the Meta Robots
        /// </summary>
        /// <param name="selectedValue"></param>
        /// <returns></returns>
        public static SelectListItem[] GetSelectListForMetaRobots(SelectedValueForMetaRobots? selectedValue = null)
        {
            return new[] {
                        new SelectListItem { Value = "index, follow", Text = "Index, Follow", Selected = (selectedValue == SelectedValueForMetaRobots.IndexFollow) },
                        new SelectListItem { Value = "noindex, follow", Text = "No index, Follow", Selected = (selectedValue == SelectedValueForMetaRobots.NoIndexFollow) },
                        new SelectListItem { Value = "index, nofollow", Text = "Index, No follow", Selected = (selectedValue == SelectedValueForMetaRobots.IndexNoFollow) },
                        new SelectListItem { Value = "noindex, nofollow", Text = "No index, No follow", Selected = (selectedValue == SelectedValueForMetaRobots.NoIndexNoFollow) }
                    };
        }
        public enum SelectedValueForMetaRobots
        {
            IndexFollow,
            NoIndexFollow,
            IndexNoFollow,
            NoIndexNoFollow
        }

        /// <summary>
        /// Returns a SelectList object to use in a DropDownListFor containing the options for the Target attribute of an action link
        /// </summary>
        /// <param name="selectedValue"></param>
        /// <returns></returns>
        public static SelectListItem[] GetSelectListForTarget(SelectedValueForTarget? selectedValue = null)
        {
            return new[] {
                        new SelectListItem { Value = "_blank", Text = Resources.Strings.NewWindow, Selected = (selectedValue == SelectedValueForTarget.NewWindow) },
                        new SelectListItem { Value = "_self", Text = Resources.Strings.SameWindow, Selected = (selectedValue == SelectedValueForTarget.SameWindow) }
                    };
        }
        public enum SelectedValueForTarget
        {
            NewWindow,
            SameWindow
        }

        /// <summary>
        /// Returns a SelectList object to use in a DropDownListFor containing the options for the Time format
        /// </summary>
        /// <param name="selectedValue"></param>
        /// <returns></returns>
        public static SelectListItem[] GetSelectListForTimeFormat(SelectedValueForTimeFormat? selectedValue = null)
        {
            return new[] {
                        new SelectListItem { Value = "HH:mm", Text = "20:30", Selected = (selectedValue == SelectedValueForTimeFormat.HHmmtt) },
                        new SelectListItem { Value = "hh:mm tt", Text = "08:30 PM", Selected = (selectedValue == SelectedValueForTimeFormat.hhmmtt) }
                    };
        }
        public enum SelectedValueForTimeFormat
        {
            HHmmtt,
            hhmmtt
        }

        /// <summary>
        /// Returns a SelectList object to use in a DropDownListFor containing the options True and False
        /// </summary>
        /// <param name="selectedValue"></param>
        /// <returns></returns>
        public static SelectListItem[] GetSelectListForYesNo(SelectedValueForYesNo? selectedValue = null)
        {
            return new[] {
                        new SelectListItem { Value = "True", Text = Resources.Strings.Yes, Selected = (selectedValue == SelectedValueForYesNo.Yes) },
                        new SelectListItem { Value = "False", Text = Resources.Strings.No, Selected = (selectedValue == SelectedValueForYesNo.No) }
                    };
        }
        public enum SelectedValueForYesNo
        {
            Yes,
            No
        }

        /// <summary>
        /// Returns a SelectList object to use in a DropDownListFor containing the options for CostMethodCode
        /// </summary>
        /// <param name="selectedValue"></param>
        /// <returns></returns>
        public static SelectListItem[] GetSelectListForSqlAuthenticationTypes(SqlAuthenticationType? selectedValue = null)
        {
            return new[] {
                        new SelectListItem { Value = "IntegratedWindowsAuthentication", Text = Resources.Strings.IntegratedWindowsAuthentication, Selected = (selectedValue == SqlAuthenticationType.IntegratedWindowsAuthentication) },
                        new SelectListItem { Value = "SqlServerAccount", Text = Resources.Strings.SqlServerAccount, Selected = (selectedValue == SqlAuthenticationType.SqlServerAccount) }
                    };
        }
        public enum SqlAuthenticationType
        {
            IntegratedWindowsAuthentication,
            SqlServerAccount
            
        }
    }
}