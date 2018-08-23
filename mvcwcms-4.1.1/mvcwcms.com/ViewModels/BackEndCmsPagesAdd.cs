using MVCwCMS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCwCMS.ViewModels
{
    public class BackEndCmsPagesAdd
    {
        [DataAnnotationsDisplay("ParentPage")]
        public int? PageParentId { get; set; }

        [DataAnnotationsDisplay("PageName")]
        [DataAnnotationsOnlyNonAccentedLettersNumbersDashesSpaces]
        [DataAnnotationsRequired]
        [DataAnnotationsStringLengthMax(255)]
        public string PageName { get; set; }

        [DataAnnotationsDisplay("PageTemplate")]
        public int? PageTemplateId { get; set; }

        [DataAnnotationsDisplay("Segment")]
        [DataAnnotationsOnlyNonAccentedLettersNumbersDashes]
        [DataAnnotationsRequiredIf("PageTemplateId", ConditionType.IsNotEqualTo, null)]
        [DataAnnotationsStringLengthMax(255)]
        public string Segment { get; set; }

        [DataAnnotationsDisplay("Url")]
        [DataAnnotationsRequiredIf("PageTemplateId", ConditionType.IsEqualTo, null)]
        [Url]
        [DataAnnotationsStringLengthMax(1000)]
        public string Url { get; set; }

        [DataAnnotationsDisplay("Target")]
        [DataAnnotationsRequired]
        [DataAnnotationsStringLengthMax(255)]
        public string Target { get; set; }

        [DataAnnotationsDisplay("ShowInMainMenu")]
        public bool ShowInMainMenu { get; set; }

        [DataAnnotationsDisplay("ShowInFooterMenu")]
        public bool ShowInBottomMenu { get; set; }

        [DataAnnotationsDisplay("ShowInSitemap")]
        public bool ShowInSitemap { get; set; }

        [DataAnnotationsDisplay("Active")]
        public bool IsActive { get; set; }

        [DataAnnotationsDisplay("Restricted")]
        public bool IsAccessRestricted { get; set; }

        [DataAnnotationsDisplay("HomePage")]
        public bool IsHomePage { get; set; }
    }
}