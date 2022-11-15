function showModal(headertext, bodytext) {
    $('#noticeModalLabel').text(headertext);
    $('.modal-body').text(bodytext);
    $('#noticeModal').modal('show');
}