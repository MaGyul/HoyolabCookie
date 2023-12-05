_loginCT = setInterval(() => {
    let e = document.querySelector('div[aria-label="dialog"]');
    let l = document.querySelector('button[aria-label="close"]');
    if (e) {
        e.removeChild(l);
        let a = document.createElement('style');
        a.textContent = 'div[aria-label="dialog"] {border-radius: 0px !important;}';
        document.body.appendChild(a);
        document.querySelector('.el-overlay-dialog').style.overflow = 'hidden';
        clearInterval(_loginCT);
    }
}, 100);