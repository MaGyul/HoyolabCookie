async function changePage() {
    let content = document.querySelector('.root-page-container__content');
    content.removeChild(document.querySelector('.root-page-container__right'));
    document.querySelector('.root-page-container').style.minWidth = '0';
    let header = document.querySelector(".mhy-page-header-content > #mhy-switch-tab > ul");
    for (let ele of header.children) ele.remove();
    header.querySelector('.mhy-switch-tab__label > span').textContent = await i18n.get('cookie');
    header.querySelector("li").style.cursor = "default";
    header.style.cursor = "default";
    let article_list = document.querySelector('.mhy-article-list');
    let home = article_list.parentElement;
    home.removeChild(article_list);
    article_list = document.createElement('div');
    article_list.className = "mhy-article-list";
    article_list.innerHTML =
        `{al_html}`
            .replaceAll('{cookie_ready}', await i18n.get('cookie_ready'))
            .replace("{cookie_copy}", await i18n.get('cookie_copy'))
            .replace("{copied}", await i18n.get('copied'));
    home.appendChild(article_list);
    header_tab = document.querySelector("#header > div > div > div.header__left > div.header-tab-wrapper");
    header_main = document.querySelector("#header > div > div > div.header__main");
    header_right = document.querySelector("#header > div > div > div.header__right");
    header_main.parentElement.removeChild(header_main);
    header_right.parentElement.removeChild(header_right);
    header_tab.parentElement.removeChild(header_tab);
    document.querySelector("#header > div > div > div > div").onclick = () => location.reload();
}
function cc() {
    navigator.clipboard.writeText(copydata);
    document.querySelector('#copybtn-tooltip').style.opacity = '1';
    window.lastId = setTimeout(() => {
        document.querySelector('#copybtn-tooltip').style.opacity = '0';
    }, 1500);
}