$(document).ready(function () {
    $('#datepicker').datepicker({
        format: 'dd/mm/yyyy',
        language: 'it',
        autoclose: true
    }).on('changeDate', function (e) {
        const dateParts = e.format().split('/');
        const isoDate = `${dateParts[2]}-${dateParts[1]}-${dateParts[0]}`;
        $('#ExpireDate').val(isoDate);
    });
});
