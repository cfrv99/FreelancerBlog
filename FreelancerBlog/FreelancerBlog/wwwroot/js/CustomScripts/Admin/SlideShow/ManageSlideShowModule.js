﻿var manageSlideShowModule = (function ($) {
    'use strict';

    function showSlideShowDetailHandler(e) {

        e.preventDefault();

        var $this = $(this);

        var slideShowBodyTxt = $this.data("slideshow-body");
        var slideShowPicture = $this.data("slideshow-picture");

        $("#slideShowBodyModal").text(slideShowBodyTxt);
        $("#slideShowPicture").attr("src", "/images/slider/" + slideShowPicture);

        $("#SlideShowDetailModal").modal("show");

    }

    function deleteSlideShowHandler(e) {
        e.preventDefault();

        var $this = $(this);
        var url = $this.attr("href");
        var antiForgeryToken = $("input[name='__RequestVerificationToken']").val();

        pNotifyModule.confirm("تایید حذف", "آیا از حذف اسلاید شو مورد نظر اطمینان دارید؟", function () {

            $.ajax({
                type: "POST",
                url: url,
                data: { __RequestVerificationToken: antiForgeryToken },
                dataType: "json",
                success: function (response) {

                    if (response.status === "Deleted") {
                        pNotifyModule.successNotice();
                        $this.closest("tr").fadeOut(2000);
                    }

                    if (response.fileDeleteResult === "Success") {
                        pNotifyModule.successNotice('عملیات موفق', "فایل عکس اسلاید شو مورد نظر با موفقیت حذف شد.");
                    }

                    if (response.status === "NotDeletedSomeProblem") {
                        pNotifyModule.failureNotice();
                    }

                },
                error: function (xhr, status, error) {
                    console.log(xhr.responseText);
                    alert("message : \n" + "An error occurred, for more info check the js console" + "\n status : \n" + status + " \n error : \n" + error);
                }
            });

        });

    }

    var Run = function () {
        $(".DeleteSlideShowButton").on("click", deleteSlideShowHandler);
        $(".slideShowDetail").on("click", showSlideShowDetailHandler);
    };

    return {
        Run: Run
    };

})(jQuery);