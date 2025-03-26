//function AdjustHeight() {
//    // Select all three elements
//    let card_1_height = $(".card-1-height").outerHeight();
//    let card_2_height = $(".card-2-height").outerHeight();
//    let card_3_height = $(".card-3-height").outerHeight();

//    console.log("Card 1 Height:", card_1_height);
//    console.log("Card 2 Height:", card_2_height);
//    console.log("Card 3 Height:", card_3_height);

//    //select all three elements that will change it's height
//    let card_1_adjustHeight = $(".card-1-adjustHeight");
//    let card_2_adjustHeight = $(".card-2-adjustHeight");
//    let card_3_adjustHeight = $(".card-3-adjustHeight");

//    // Get the max height
//    let maxHeight = Math.max(card_1_height, card_2_height, card_3_height);

//    if (maxHeight > card_1_height) {
//        card_1_adjustHeight.css("height", (maxHeight - card_1_height) + "px");
//    }

//    if (maxHeight > card_2_height) {
//        card_2_adjustHeight.css("height", (maxHeight - card_2_height) + "px");
//    }

//    if (maxHeight > card_3_height) {
//        card_3_adjustHeight.css("height", (maxHeight - card_3_height) + "px");
//    }
//}