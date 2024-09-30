mergeInto(LibraryManager.library, {

    IsUserAgentMobile: function () {
        return /iPhone|iPad|iPod|Android|Windows Phone|webOS/i.test(navigator.userAgent);
    },

});