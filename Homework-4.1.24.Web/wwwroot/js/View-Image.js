$(() => {
    $(".bi-heart-fill").on("click", function () {
        $.get('/questions/DidLikeQuestion', { id: $("#question-id").val() }, function (obj) {
            if (obj.notPreviouslyLiked) {
                $.post('/questions/likequestion', { id: $("#question-id").val() }, function () {
                    $("#likes-count").text(+$("#likes-count").text() + 1);
                });
            }
        })
    })
})