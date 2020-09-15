
class ControlPanelSettings {
    constructor(
        sortType = 0,
        sortField = "Id",
        pageRowsCount = 10,
        currentPage = 1,
        searchText = "",
        filterField = "") {
            this.SortType = sortType;
            this.SortField = sortField;
            this.PageRowsCount = pageRowsCount;
            this.CurrentPage = currentPage;
            this.SearchText = searchText;
            this.FilterField = filterField;
    }
};

const ControllerName = document.getElementById("ControllerName").value;
const SettingsName = ControllerName + "Settings";
var CurrentSettings = new ControlPanelSettings();

$(document).ready(() => {
    const localCookie = getCookie(SettingsName);
    if (localCookie !== undefined && localCookie) {
        deserializeSettings();
    }
    else {
        serializeSettings();
    }

    sortFieldBindEventOnClick();
    searchBindEventOnClick();
    paginationBindEventOnClick();
});

function getCookie(name) {
    //console.log("Function 'getCookie' is started.");
    let matches = document.cookie.match(new RegExp(
        "(?:^|; )" + name.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, '\\$1') + "=([^;]*)"
    ));
    //if (matches != undefined && matches) {
    //    console.log(`Geting cookie for '${name}' is success made.`);
    //}

    return matches ? decodeURIComponent(matches[1]) : undefined;
}

function setCookie(name, value, options = {}) {
    //console.log("Function 'setCookie' is started.");
    options = {
        path: '/',
        secure: true,
        sameSite: "Lax"
    };

    if (options.expires instanceof Date) {
        options.expires = options.expires.toUTCString();
    }

    let updatedCookie = encodeURIComponent(name) + "=" + encodeURIComponent(value);

    for (let optionKey in options) {
        updatedCookie += "; " + optionKey;
        let optionValue = options[optionKey];
        if (optionValue !== true) {
            updatedCookie += "=" + optionValue;
        }
    }

    document.cookie = updatedCookie;
    //if (updatedCookie != undefined && updatedCookie) {
    //    console.log(`Seting cookie for '${name}=${value}' is success made.`);
    //}
}

function sortFieldBindEventOnClick(){
    //console.log("Method 'sortFieldBindEventOnClick' was run...");
    const arr = document.getElementsByClassName("sort-field");
    for (var idx in arr) {
        var el = arr[idx];
        el.onclick = function(event){
            var sortField = event.target.attributes.value.value;
            CurrentSettings.SortField = sortField;
            if (CurrentSettings.SortType === 0) { CurrentSettings.SortType = 1; }
            else { CurrentSettings.SortType = 0; }
            serializeSettings();
        };
    };
    //console.log("Method 'sortFieldBindEventOnClick' success is made...");
}

function paginationBindEventOnClick(){
    //console.log("Method 'paginationBindEventOnClick' was run...");
    const arr = document.getElementsByClassName("page-control-panel");
    for (var idx in arr) {
        var el = arr[idx];
        el.onclick = function(event) {
            var currPage = event.target.attributes.value.value;
            CurrentSettings.CurrentPage = currPage;
            serializeSettings();y
        }
    };
    //console.log("Method 'paginationBindEventOnClick' success is made...");
}

function searchBindEventOnClick() {
    //console.log("Method 'searchBindEventOnClick' was run...");
    var btn = document.getElementById("StartSearch");
    btn.onclick = function() {
        var inputText = document.getElementById("SearchText");
        var searchText = inputText.value;
        CurrentSettings.SearchText = searchText;
        serializeSettings();
    };
    //console.log("Method 'searchBindEventOnClick' success is made...");
}

function deserializeSettings()
{
    //console.log("Method 'deserializeSettings' was run...");
    const settings = JSON.parse(getCookie(SettingsName));
    CurrentSettings = Object.setPrototypeOf(settings, ControlPanelSettings.prototype);
    //console.log("Method 'deserializeSettings' success is made...");
}

function serializeSettings() {
    //console.log("Method 'serializeSettings' was run...");
    setCookie(SettingsName, JSON.stringify(CurrentSettings));
    //console.log("Method 'serializeSettings' success is made...");
}
