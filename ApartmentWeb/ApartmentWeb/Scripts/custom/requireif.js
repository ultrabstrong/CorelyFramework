// Add RequireIfEnum definition
$.validator.unobtrusive.adapters.add('requireifenum', ['checkifname', 'checkifvalue'], function (options) {
    options.rules['requireifenum'] = options.params;
    options.messages['requireifenum'] = options.message;
});
// Add RangeIfEnum definition
$.validator.unobtrusive.adapters.add('rangeifenum', ['minvalue', 'maxvalue', 'checkifname', 'checkifvalue'], function (options) {
    options.rules['rangeifenum'] = options.params;
    options.messages['rangeifenum'] = options.message;
});

$(document).ready(function () {

    // RequireIfEnum vlaidation handler
    $.validator.addMethod('requireifenum', function (value, element, parameters) {
        // Get the check if value
        var checkifvalue = parameters.checkifvalue;
        checkifvalue = (checkifvalue == null ? '' : checkifvalue).toString();
        // Get relative element name for child models
        var childinstancename = element.id.substring(0, element.id.indexOf('_') + 1);
        // Get input vlaue
        var actualvalue = $("#" + childinstancename + parameters.checkifname).val();
        // If input value is check if value
        if ($.trim(checkifvalue).toLowerCase() === $.trim(actualvalue).toLocaleLowerCase()) {
            // Validate element
            var isValid = $.validator.methods.required.call(this, value, element, parameters);
            return isValid;
        }
        // If input value is not check if value
        return true;
    });
    // RangeIfEnum vlaidation handler
    $.validator.addMethod('rangeifenum', function (value, element, parameters) {
        // Get the check if value
        var checkifvalue = parameters.checkifvalue;
        checkifvalue = (checkifvalue == null ? '' : checkifvalue).toString();
        // Get relative element name for child models
        var childinstancename = element.id.substring(0, element.id.indexOf('_') + 1);
        // Get input vlaue
        var actualvalue = $("#" + childinstancename + parameters.checkifname).val();
        // If input value is check if value
        if ($.trim(checkifvalue).toLowerCase() === $.trim(actualvalue).toLocaleLowerCase()) {
            // Validate element
            //var isValid = $.validator.methods.range.call(this, value, element, parameters);
            var isValid = true;
            var valdec = parseFloat(value);
            // Validate minimum
            if (parameters.minvalue) {
                var mindec = parseFloat(parameters.minvalue);
                if (valdec < mindec) {
                    isValid = false;
                }
            }
            // Validate maxiumum
            if (parameters.maxvalue) {
                var maxdec = parseFloat(parameters.maxvalue);
                if (valdec > maxdec) {
                    isValid = false;
                }
            }
            return isValid;
        }
        // If input value is not check if value
        return true;
    });
});