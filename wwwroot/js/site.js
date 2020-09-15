var currCntrlName = null;

$(document).ready(() => {
    bindToAccessRightsOnClick();
});

function bindToAccessRightsOnClick() {
    console.log("Method 'bindToAccessRightsOnClick' sucessfully ran");
    var cntrlSlct = document.getElementById("Controller_Name");
    currCntrlName = cntrlSlct.selectedOptions[0].value;
    cntrlSlct.onchange = function ()
    {
        const textToReplace = "hidden-";
        var slctCntrlName = cntrlSlct.selectedOptions[0].value;
        var currActDiv = document.getElementById(`_${currCntrlName}`);
        var slctActDiv = document.getElementById(`_${slctCntrlName}`);

        currActDiv.style.display = "none";
        slctActDiv.style.display = "block";

        var currInps = currActDiv.querySelectorAll("input");
        var slctInps = slctActDiv.querySelectorAll("input");

        currInps.forEach((el) => {
            el.id = textToReplace + el.id;
            el.name = textToReplace + el.name;
        });
        slctInps.forEach((el) => {
            el.id = el.id.replace(textToReplace, "");
            el.name = el.name.replace(textToReplace, "");
        });

        currCntrlName = slctCntrlName;
    }
    console.log("Method 'bindToAccessRightsOnClick' sucessfully made");
}