// Write your Javascript code.
$(function () {
    var $table = $('table');
    $links = $table.find('a.rowLink');

    $(window).resize(function () {
        $links.width($table.width());
    });

    $(window).trigger('resize');
});