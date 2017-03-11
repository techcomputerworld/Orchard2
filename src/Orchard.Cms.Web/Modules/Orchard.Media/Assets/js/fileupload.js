$(function () {
    $('#fileupload').fileupload({
        dataType: 'json',
        url: $('#uploadFiles').val(),
        formData: function() {
            return [{name: 'path', value: navigationApp.selectedFolder.path}]
        },
        done: function (e, data) {
            $.each(data.result.files, function (index, file) {
                filesApp.mediaItems.push(file.model)
            });
            $('#progress .progress-bar').css(
                'width',
                0 + '%'
            );
        }
    });
});