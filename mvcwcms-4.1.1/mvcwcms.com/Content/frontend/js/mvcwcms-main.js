jQuery(document).ready(function ($) {

    $(".go-back").click(function () {
        history.back();
        return false;
    });

    //Attaches a tooltip message to the invalid fields. It requires a CSS setting for .field-validation-error
    $(".form-group").on("focus", ".input-validation-error", function (event) {
        var errElement = $(this).next().find(":last-child");
        $(this).tooltip("destroy");
        $(this).tooltip({
            title: errElement.html(),
            container: "body"
        });
    });

    $(".redirect").click(function () {
        window.location.href = $(this).attr("data-redirect-url");
        return false;
    });

    $(".date-mask").inputmask(DateFormat); //If the inputmask fails most probably is because the jquery.inputmask.date.extensions.js file is missing the alias for the current DateFormat specified in _LayoutBackEnd.cshtml

    $(".date-picker").datetimepicker({
        language: LanguageCode,
        format: DateFormat.toUpperCase(),
        pickDate: true,
        pickTime: false,
        useCurrent: false
    });
    $(".time-picker").datetimepicker({
        language: LanguageCode,
        format: TimeFormat.replace("tt", "A"),
        pickDate: false,
        pickTime: true,
        useCurrent: false
    });
    $(".datetime-picker").datetimepicker({
        language: LanguageCode,
        format: DateFormat.toUpperCase() + " " + TimeFormat.replace("tt", "A"),
        pickDate: true,
        pickTime: true,
        useCurrent: false
    });

    $(".submit-confirm").click(function () {
        if (confirm(ToConfirmSubmitPressOK)) {
            $(".validation-summary-errors").removeClass("alert-success").addClass("alert-danger");
            $(this).closest("form").submit();
        }
        return false;
    });

    $.goup({
        trigger: 200
    });

    $(".footable").footable({
        breakpoints: {
            phone: 768,
            tablet: 1024
        }
    });
    $(".footable-expand-all").click(function () {
        $(".footable").trigger("footable_expand_all");
    });
    $(".footable-collapse-all").click(function () {
        $(".footable").trigger("footable_collapse_all");
    });

});