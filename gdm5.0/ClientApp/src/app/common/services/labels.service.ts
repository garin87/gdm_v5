import { Injectable } from "@angular/core";
import { IGdmLabels } from "../objects/common";
import { gdmLabelsENG } from "../objects/gdmLabelsENG";
import { gdmLabelsRU } from "../objects/gdmLabelsRU";

@Injectable()
export class LabelsService {
    
    private allLabels:IGdmLabels;
    get labels():IGdmLabels { return this.allLabels};

    constructor() {
        this.getLabels("RU");
    }

    getLabels(lang){
       if(lang == "ENG"){
         this.allLabels = new gdmLabelsENG();
       }else if(lang == "RU"){
         this.allLabels = new gdmLabelsRU();
       }
    }
    
}