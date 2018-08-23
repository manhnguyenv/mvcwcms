using MVCwCMS.Helpers;

namespace MVCwCMS.ViewModels
{
    public class BackEndInstallation
    {
        [DataAnnotationsDisplay("AdminLanguageCode")]
        [DataAnnotationsRequired]
        public string AdminLanguageCode { get; set; }

        public bool IsChangeAdminLanguageCode { get; set; }

        [DataAnnotationsDisplay("SqlServerName")]
        [DataAnnotationsRequired]
        public string SqlServerName { get; set; }

        [DataAnnotationsDisplay("DatabaseName")]
        [DataAnnotationsRequired]
        public string DatabaseName { get; set; }

        [DataAnnotationsDisplay("CreateDbIfDoesNotExist")]
        public bool CreateDbIfDoesNotExist { get; set; }

        [DataAnnotationsDisplay("IgnoreDbExistsWarning")]
        public bool IgnoreDbExistsWarning { get; set; }

        [DataAnnotationsDisplay("ResetDbIfDoesExist")]
        public bool ResetDbIfDoesExist { get; set; }

        [DataAnnotationsDisplay("SqlAuthenticationType")]
        [DataAnnotationsRequired]
        public string SqlAuthenticationType { get; set; }

        [DataAnnotationsDisplay("CurrentWindowsUser")]
        public string CurrentWindowsUser { get; set; }

        [DataAnnotationsDisplay("SqlUsername")]
        [DataAnnotationsRequiredIf("SqlAuthenticationType", ConditionType.IsNotEqualTo, "IntegratedWindowsAuthentication")]
        public string SqlUsername { get; set; }

        [DataAnnotationsDisplay("SqlPassword")]
        [DataAnnotationsRequiredIf("SqlAuthenticationType", ConditionType.IsNotEqualTo, "IntegratedWindowsAuthentication")]
        [DataAnnotationsStringLengthRange(6, 255)]
        public string SqlPassword { get; set; }
    }
}