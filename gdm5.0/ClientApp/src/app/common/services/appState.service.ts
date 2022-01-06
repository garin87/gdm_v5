import { Injectable } from "@angular/core";
import { BehaviorSubject } from "rxjs";


@Injectable()
export class appStateService {

    public detectClickOnPanel = new BehaviorSubject<any>(undefined); 
    public selectedSidePanelValue = new BehaviorSubject<any>(undefined); 
    public changedPageGrid = new BehaviorSubject<any>(undefined);
    constructor() {}

}