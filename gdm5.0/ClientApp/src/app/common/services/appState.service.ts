import { Injectable } from "@angular/core";
import { BehaviorSubject } from "rxjs";
import { ProductTypeInstancesRequest } from "../objects/common";
import { CurrenciesService } from "./currencies.service";


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
    private Cur_OfficialRate_USD_V:number = undefined;
    private Cur_OfficialRate_EUR_V:number = undefined;
   
    get Cur_OfficialRate_USD(){
        return this.Cur_OfficialRate_USD_V;
    };
    
    set Cur_OfficialRate_USD(value){
        this.Cur_OfficialRate_USD_V = value;
    }

    get Cur_OfficialRate_EUR(){
        return this.Cur_OfficialRate_EUR_V;
    };
    
    set Cur_OfficialRate_EUR(value){
        this.Cur_OfficialRate_EUR_V = value;
    }

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

    constructor() {

    }

}