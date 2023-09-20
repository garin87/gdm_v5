import { Injectable } from "@angular/core";
import { BehaviorSubject } from "rxjs";
import { ProductTypeInstancesRequest } from "../objects/common";


@Injectable()
export class AppStateService {
   
    public detectClickOnPanel = new BehaviorSubject<boolean>(false); 
    public detectClickOnSubPanel = new BehaviorSubject<any>(undefined); 
    //SidePanel
    public selectedSidePanelValue = new BehaviorSubject<any>(undefined); 
    public selectedSideSubPanelValue = new BehaviorSubject<any>(undefined); 
    public selectedSideSubPanelValueName = new BehaviorSubject<any>(undefined); 
    public selectedSidePaneModelingValue = new BehaviorSubject<any>(undefined); 
    public selectedRowGrid = new BehaviorSubject<any>(undefined); 
    public changedPageGrid = new BehaviorSubject<any>(undefined);
    public changedGridOption = new BehaviorSubject<any>(undefined);
    public refreshGridData = new BehaviorSubject<boolean>(false);
    public refreshGridPainData = new BehaviorSubject<boolean>(false);
    public refreshSubPanelContentData = new BehaviorSubject<boolean>(false);
    public refreshListOfPtopsofSubPanel = new BehaviorSubject<any>(undefined);
    public refreshPainGrid = new BehaviorSubject<boolean>(false);
    public refreshOrderGrid = new BehaviorSubject<boolean>(false);

    public optionValue = undefined; 
    public initRightActionPanel = false; 
    public gridFilterData = {};
    public instanceOfProduct = undefined;
    
    // order
    public resetOrderTable = new BehaviorSubject<any>(undefined);
    public cartProductCount = new BehaviorSubject<number>(0);

    public selectedInstancePanel: string;

    //add instance product 
    public refreshAddProductSection = new BehaviorSubject<boolean>(false);
    public selectedProductName = undefined;
    
    // Currency 
    public Cur_OfficialRate_USD:number = undefined;
    public Cur_OfficialRate_EUR:number = undefined;
    public currencyNBRB:Array<any> = undefined;
    
    // Grid spinner
    public LoadingGridResults = new BehaviorSubject<boolean>(false);
    // Grid Option
    public getProductTypeInstancesRequest = new ProductTypeInstancesRequest();

    // Right panel
    public isActiveRightActionPanel = new BehaviorSubject<boolean>(false);

    // Filter Parameters 
    public listFilterParameters:Array<any> = [];
    public filter_TileFilterParameters:Array<any> = [];
    public filter_clickByTileFilter = new BehaviorSubject<any>(undefined);
    public filter_initCreateFilterTileComponent = new BehaviorSubject<any>(undefined);

    constructor() {}

}