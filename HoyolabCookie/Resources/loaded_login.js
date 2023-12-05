_loginCT = setInterval(() => {
    if (!location.href.includes("hoyolab")) {
        clearInterval(_loginCT);
        return
    }
    let e = document.querySelector(".login-box-side_bottom__btn");
    e && !document.querySelector("#hyv-account-sdk-frame") && (e.click(), clearInterval(_loginCT), window.changePage());
}, 100);
_closeNeed = setInterval(() => {
    if (!location.href.includes("hoyolab")) {
        clearInterval(_closeNeed);
        return;
    }
    let e = document.querySelector("div.mhy-interest-selector__header > div > button");
    e && (e.click(), clearInterval(_closeNeed));
}, 100);