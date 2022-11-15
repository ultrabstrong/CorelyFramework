$(document).on('change', '.yesno_visible_toggle', function () {
    var selectedval = $(this).val();
    var togglevisible = $(this).attr('visibletogglefor');
    if (selectedval == '2') {
        $(this).parent().parent().next('#' + togglevisible).show();
        $(this).parent().next('#' + togglevisible).show();
    }
    else {
        $(this).parent().parent().next('#' + togglevisible).hide();
        $(this).parent().next('#' + togglevisible).hide();
    }
});