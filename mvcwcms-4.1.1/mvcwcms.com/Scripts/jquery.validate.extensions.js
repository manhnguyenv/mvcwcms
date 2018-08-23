jQuery(document).ready(function ($) {
    //////////////////////////////////////
    // Custom jquery validation methods //
    //////////////////////////////////////
    jQuery.validator.addMethod('requiredif', function (value, element, params) {
        var propertyName = element.attributes["data-val-requiredifattribute-propertyname"].value;

        var propertyValue;
        if ($("#" + propertyName).is(":checkbox")) {
            propertyValue = $("#" + propertyName).prop("checked");
        } else {
            propertyValue = $("#" + propertyName).val();
        }
        propertyValue = propertyValue.toString().toLowerCase();

        var conditionType = element.attributes["data-val-requiredifattribute-conditiontype"].value;

        var conditionValue = element.attributes["data-val-requiredifattribute-conditionvalue"].value.toLowerCase();

        var isConditionTypeValid = false;

        switch (conditionType)
        {
            case "IsEqualTo":
                if (propertyValue !== "")
                {
                    isConditionTypeValid = (propertyValue === conditionValue);
                }
                else if (propertyValue === "" && conditionValue === "")
                {
                    isConditionTypeValid = true;
                }
                else if (propertyValue === "" && conditionValue !== "")
                {
                    isConditionTypeValid = false;
                }
                break;
            case "IsNotEqualTo":
                if (propertyValue !== "")
                {
                    isConditionTypeValid = (propertyValue !== conditionValue);
                }
                else if (propertyValue === "" && conditionValue === "")
                {
                    isConditionTypeValid = false;
                }
                else if (propertyValue === "" && conditionValue !== "")
                {
                    isConditionTypeValid = true;
                }
                break;
        }
        if (isConditionTypeValid)
        {
            if (value === "")
                return false;
        }
        return true
    }, '');

    /////////////////////////////////
    // Custom unobtrusive adapters //
    /////////////////////////////////
    jQuery.validator.unobtrusive.adapters.add('requiredifattribute', {}, function (options) {
        options.rules['requiredif'] = true;
        options.messages['requiredif'] = options.message;
    });
});