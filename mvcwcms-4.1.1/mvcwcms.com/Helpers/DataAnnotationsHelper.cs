using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace MVCwCMS.Helpers
{
    //IMPORTANT: In the Global.asax we need to register the custom attributes with the relative validation providers

    public sealed class DataAnnotationsDisplay : DisplayNameAttribute
    {
        private string Name { get; set; }

        public DataAnnotationsDisplay(string name)
        {
            this.Name = name;
        }

        public override string DisplayName
        {
            get
            {
                if (this.Name.IsNotEmptyOrWhiteSpace())
                    //return Resources.Strings.ResourceManager.GetString(this.Name);
                    return this.Name.GetStringFromResource();
                else
                    return null;
                
                //return base.DisplayName;
            }
        }
    }

    public sealed class DataAnnotationsDomainName : RegularExpressionAttribute
    {
        public DataAnnotationsDomainName()
            : base(@"^(?!www\.)([a-zA-Z0-9]([a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?\.)+[a-zA-Z]{2,6}$")
        {
            base.ErrorMessageResourceType = typeof(Resources.Strings);
            base.ErrorMessageResourceName = "DataAnnotationsRegularExpression";
        }
    }

    public sealed class DataAnnotationsEmailAddress : DataTypeAttribute, IClientValidatable
    {
        private static Regex _regex = new Regex(@"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

        public DataAnnotationsEmailAddress()
            : base(DataType.EmailAddress)
        {
            base.ErrorMessage = null;
            base.ErrorMessageResourceType = typeof(Resources.Strings);
            base.ErrorMessageResourceName = "DataAnnotationsEmailAddress";
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ValidationType = "email",
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName())
            };
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            string valueAsString = value as string;
            return valueAsString != null && _regex.Match(valueAsString).Length > 0;
        }
    }

    public sealed class DataAnnotationsRange : RangeAttribute
    {
        public DataAnnotationsRange(int from, int to)
            : base(from, to)
        {
            base.ErrorMessageResourceType = typeof(Resources.Strings);
            base.ErrorMessageResourceName = "DataAnnotationsRange";
        }
    }

    public sealed class DataAnnotationsNotEqualTo : RegularExpressionAttribute
    {
        public DataAnnotationsNotEqualTo(string value)
            : base(@"^(?!" + value + "$).*$")
        {
            base.ErrorMessageResourceType = typeof(Resources.Strings);
            base.ErrorMessageResourceName = "DataAnnotationsRegularExpression";
        }
    }

    public sealed class DataAnnotationsOnlyDecimal : RegularExpressionAttribute
    {
        public DataAnnotationsOnlyDecimal()
            : base(@"^\d+\.?\d{0,2}$")
        {
            base.ErrorMessageResourceType = typeof(Resources.Strings);
            base.ErrorMessageResourceName = "DataAnnotationsOnlyDecimal";
        }
    }

    public sealed class DataAnnotationsOnlyIntegers : RegularExpressionAttribute
    {
        public DataAnnotationsOnlyIntegers()
            : base(@"^-?[0-9]*$")
        {
            base.ErrorMessageResourceType = typeof(Resources.Strings);
            base.ErrorMessageResourceName = "DataAnnotationsOnlyIntegers";
        }
    }

    public sealed class DataAnnotationsOnlyLetters : RegularExpressionAttribute
    {
        public DataAnnotationsOnlyLetters()
            : base(@"^[a-zA-Z]*$")
        {
            base.ErrorMessageResourceType = typeof(Resources.Strings);
            base.ErrorMessageResourceName = "DataAnnotationsOnlyLetters";
        }
    }

    public sealed class DataAnnotationsOnlyLettersNumbers : RegularExpressionAttribute
    {
        public DataAnnotationsOnlyLettersNumbers()
            : base(@"^[a-zA-Z0-9]*$")
        {
            base.ErrorMessageResourceType = typeof(Resources.Strings);
            base.ErrorMessageResourceName = "DataAnnotationsOnlyLettersNumbers";
        }
    }

    public sealed class DataAnnotationsOnlyNonAccentedLettersNumbersDashes : RegularExpressionAttribute
    {
        public DataAnnotationsOnlyNonAccentedLettersNumbersDashes()
            : base(@"^[a-zA-Z0-9-]*$")
        {
            base.ErrorMessageResourceType = typeof(Resources.Strings);
            base.ErrorMessageResourceName = "DataAnnotationsOnlyNonAccentedLettersNumbersDashes";
        }
    }

    public sealed class DataAnnotationsOnlyNonAccentedLettersNumbersDashesSpaces : RegularExpressionAttribute
    {
        public DataAnnotationsOnlyNonAccentedLettersNumbersDashesSpaces()
            : base(@"^[a-zA-Z0-9 -]*$")
        {
            base.ErrorMessageResourceType = typeof(Resources.Strings);
            base.ErrorMessageResourceName = "DataAnnotationsOnlyNonAccentedLettersNumbersDashesSpaces";
        }
    }

    public sealed class DataAnnotationsOnlyNumbers : RegularExpressionAttribute
    {
        public DataAnnotationsOnlyNumbers()
            : base(@"^[0-9]*$")
        {
            base.ErrorMessageResourceType = typeof(Resources.Strings);
            base.ErrorMessageResourceName = "DataAnnotationsOnlyNumbers";
        }
    }

    public sealed class DataAnnotationsOnlySafeCharacters : RegularExpressionAttribute
    {
        public DataAnnotationsOnlySafeCharacters()
            : base(@"^[a-zA-Z0-9 '!@#$*()_+=,.:;""?&/%-]*$")
        {
            base.ErrorMessageResourceType = typeof(Resources.Strings);
            base.ErrorMessageResourceName = "DataAnnotationsOnlySafeCharacters";
        }
    }

    public sealed class DataAnnotationsRequired : RequiredAttribute
    {
        public DataAnnotationsRequired(string resourceName = "")
        {
            this.ErrorMessageResourceType = typeof(Resources.Strings);
            if (resourceName.IsNotEmptyOrWhiteSpace())
            {
                this.ErrorMessageResourceName = resourceName;
            }
            else
            {
                this.ErrorMessageResourceName = "DataAnnotationsRequired";
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class DataAnnotationsRequiredAtLeastOneAttribute : ValidationAttribute//, IClientValidatable //Not possible to implement: http://stackoverflow.com/questions/5803314/client-side-validation-for-custom-validationattribute-with-attributetargets-clas
    {
        private string[] PropertyList { get; set; }

        public DataAnnotationsRequiredAtLeastOneAttribute(params string[] propertyList)
        {
            this.PropertyList = propertyList;
        }

        public override object TypeId
        {
            get
            {
                return this;
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo propertyInfo;
            foreach (string propertyName in PropertyList)
            {
                propertyInfo = validationContext.ObjectInstance.GetType().GetProperty(propertyName);
                if (propertyInfo != null && propertyInfo.GetValue(validationContext.ObjectInstance, null).IsNotNull())
                {
                    return ValidationResult.Success;
                }
            }
            //return new ValidationResult(Resources.Strings.AtLeastOneField + " " + PropertyList.Select(i => i = Resources.Strings.ResourceManager.GetString(i)).ToCSV(',').Replace(",", ", "));
            return new ValidationResult(Resources.Strings.AtLeastOneField + " " + PropertyList.Select(i => i = i.GetStringFromResource()).ToCSV(',').Replace(",", ", "));
        }
    }

    public enum ConditionType
    {
        IsEqualTo,
        IsNotEqualTo
    }
    public sealed class DataAnnotationsRequiredIfAttribute : ValidationAttribute, IClientValidatable
    {
        private RequiredAttribute requiredAttribute = new RequiredAttribute();
        private string PropertyName { get; set; }
        private ConditionType ConditionType { get; set; }
        private object ConditionValue { get; set; }

        public DataAnnotationsRequiredIfAttribute(string propertyName, ConditionType conditionType, object conditionValue)
        {
            this.PropertyName = propertyName;
            this.ConditionType = conditionType;
            this.ConditionValue = conditionValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectInstance.GetType().GetProperty(PropertyName);
            if (property != null)
            {
                var propertyValue = property.GetValue(validationContext.ObjectInstance, null);
                bool isConditionTypeValid = false;
                switch (ConditionType)
                {
                    case ConditionType.IsEqualTo:
                        if (propertyValue.IsNotNull())
                        {
                            isConditionTypeValid = propertyValue.Equals(ConditionValue);
                        }
                        else if (propertyValue.IsNull() && ConditionValue.IsNull())
                        {
                            isConditionTypeValid = true;
                        }
                        else if (propertyValue.IsNull() && ConditionValue.IsNotNull())
                        {
                            isConditionTypeValid = false;
                        }
                        break;
                    case ConditionType.IsNotEqualTo:
                        if (propertyValue.IsNotNull())
                        {
                            isConditionTypeValid = !propertyValue.Equals(ConditionValue);
                        }
                        else if (propertyValue.IsNull() && ConditionValue.IsNull())
                        {
                            isConditionTypeValid = false;
                        }
                        else if (propertyValue.IsNull() && ConditionValue.IsNotNull())
                        {
                            isConditionTypeValid = true;
                        }
                        break;
                }
                if (isConditionTypeValid)
                {
                    if (!requiredAttribute.IsValid(value))
                        return new ValidationResult(requiredAttribute.FormatErrorMessage(validationContext.DisplayName));
                }

            }
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ModelClientValidationRule modelClientValidationRule = new ModelClientValidationRule()
            {
                ErrorMessage = requiredAttribute.FormatErrorMessage(metadata.DisplayName),
                ValidationType = "requiredifattribute"
            };
            modelClientValidationRule.ValidationParameters.Add("propertyname", PropertyName);
            modelClientValidationRule.ValidationParameters.Add("conditiontype", ConditionType);
            modelClientValidationRule.ValidationParameters.Add("conditionvalue", ConditionValue);
            yield return modelClientValidationRule;
        }
    }

    public sealed class DataAnnotationsStringLengthMax : StringLengthAttribute
    {
        public DataAnnotationsStringLengthMax(int maximumLength)
            : base(maximumLength)
        {
            base.ErrorMessageResourceType = typeof(Resources.Strings);
            base.ErrorMessageResourceName = "DataAnnotationsStringLengthMax";
        }
    }

    public sealed class DataAnnotationsStringLengthMin : MinLengthAttribute
    {
        public DataAnnotationsStringLengthMin(int length)
            : base(length)
        {
            base.ErrorMessageResourceType = typeof(Resources.Strings);
            base.ErrorMessageResourceName = "DataAnnotationsStringLengthMin";
        }
    }

    public sealed class DataAnnotationsStringLengthRange : StringLengthAttribute
    {
        public DataAnnotationsStringLengthRange(int minimumLength, int maximumLength)
            : base(maximumLength)
        {
            base.ErrorMessageResourceType = typeof(Resources.Strings);
            base.ErrorMessageResourceName = "DataAnnotationsStringLengthRange";
            base.MinimumLength = minimumLength;
        }
    }

    public sealed class DataAnnotationsURL : RegularExpressionAttribute
    {
        public DataAnnotationsURL()
            : base(@"^(http|https)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?/\\\+&amp;%\$#\=~])*[a-zA-Z0-9\-\._/\\\+&amp;%\$#\=~]$")
        {
            base.ErrorMessageResourceType = typeof(Resources.Strings);
            base.ErrorMessageResourceName = "DataAnnotationsRegularExpression";
        }
    }

    public class NoDuplicateModules
    {
        public static ValidationResult Validate(string value, ValidationContext vc)
        {
            string error = string.Empty;
            List<ControllerAction> caList = ControllerHelper.GetAllControllersActions();
            foreach (ControllerAction ca in caList)
            {
                if (value.Split(new string[] { ca.ControllerActionID }, System.StringSplitOptions.None).Length > 2)
                {
                    error += ca.ControllerActionID + ", ";
                }
            }
            error = error.TrimEnd(' ').TrimEnd(',');
            if (error.IsEmptyOrWhiteSpace())
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(Resources.Strings.ModulesInsertedMoreThanOnce + " " + error);
            }
        }
    }
}
