import { Component, HostListener, ViewChild } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { BehaviorSubject, Subscription } from 'rxjs';

import { AlertService } from '../alert/alert.service';
import { ApplicationService } from '../common/services/application.service';
import { AppStateService } from '../common/services/appState.service';
import { FormEditorService } from '../common/services/formEditor.service';
import { LabelsService } from '../common/services/labels.service';
import { IMetadataProperty } from '../common/objects/common';

@Component({
  selector: 'app-modeling-component',
  styleUrls: ['./modeling.component.css'],
  templateUrl: './modeling.component.html'
  
})
export class ModelingComponent {
      @ViewChild("appSidePanel") appSidePanel: any;
      @ViewChild("appSubSidePanel") appSubSidePanel: any;

      public isCreateModel: boolean = false;
      public isCompanyPage: boolean = false;
      public isWareHousePage: boolean = false;
      public isCurrencyPage: boolean = false;
      public isPriceListPage: boolean = false;
      public isPriceListBasePage: boolean = false;
      public isProductPage: boolean = false;
      
      public instanceName:string = 'Product';
      public sidenavToggle;
      //public namesParoduct: any = undefined;
      public isGridShow:boolean = false;
      public gridProductData = undefined;
      public gridMetadaType = "ProductGrid";
      public isLoadingResults = false;
      public currentModelName : string;
      public currentModelInstanceName : string;
      public modelingIntems : any = [{ Name:"Company", DisplayName: "Company"},
      { Name:"Currency", DisplayName: "Currency"},
      { Name:"PriceListBase", DisplayName: "Price List Base"},
      { Name:"PriceList", DisplayName: "Price List"},
      { Name:"WareHouse", DisplayName: "WareHouse"},
      { Name:"Product", DisplayName: "Product"}];

      public companyIntems : any = [];
      public productTypes : any = [];
      public priceListValuesData:any = undefined;
      private routerEvents$:Subscription;
      private selectedSidePaneModelingValueSubscription$:Subscription;
      private selectedSideSubPanelValueSubscription$:Subscription;
      private refreshListOfPtopsofSubPanelSubscription$:Subscription;

      public properties;
      public properties2;
      // Product
      public isNewProduct: boolean = false;
      public isEditParamProduct: boolean = false;

      get currentModelNameGet(){
        if(this.currentModelName)
          return "/ " +  this.currentModelName;
          
        return this.currentModelName;
      }

      get currentModelInstanceNameGet(){
        if(this.currentModelInstanceName)
          return "/ " +  this.currentModelInstanceName;
          
        return this.currentModelInstanceName;
      }

      // @HostListener('document:keydown.enter')
      // onDocumentKeydownEnter() {
      //   this.saveModel()
      // }  
      
      constructor(private _alertService : AlertService,
                  private _applicationService : ApplicationService, 
                  private _formEditorService : FormEditorService,
                  private alertService:AlertService,
                  private _appStateService:AppStateService,
                  private router: Router,
                  public _labelsService: LabelsService){
      this.isCreateModel = false;
      }
                         
      ngOnInit(){
        this.properties = new BehaviorSubject<IMetadataProperty[]>(undefined);
        this.properties2 = new BehaviorSubject<IMetadataProperty[]>(undefined);
        this._appStateService.detectClickOnPanel.next(true);
        this.selectedSidePaneModelingValueSubscription$ = this._appStateService.selectedSidePaneModelingValue
        .subscribe((nameModel:string) => {
            if(!nameModel) return; 
            this._appStateService.selectedInstancePanel = nameModel;
            this.currentModelName = nameModel.toLocaleLowerCase();
            this.currentModelInstanceName = undefined; 
           // this.isLoadingResults = true;
            this.isCreateModel = false;
            this.isCompanyPage = false;
            this.isWareHousePage = false;
            this.isCurrencyPage = false;
            this.isPriceListPage = false;
            this.isPriceListBasePage = false;
            this.isProductPage = false;
            
            this._appStateService.instanceOfProduct = undefined;
            this._appStateService.initRightActionPanel = false;
            this.companyIntems = [];

            switch (nameModel) {
              case 'Company':
                this.getCompanies();
                break;
              case 'WareHouse':
                this.getWareHouses();
                break;
              case 'Currency':
                this.getCurrencies();
                break;
              case 'PriceListBase':
                this.getPriceList();
                break;
              case 'PriceList':
                this.getPriceList();
                break;
              case 'Product':{
              //   this.getProductTypes();
                 if(this.appSubSidePanel?.drawer?.opened){
                    this.appSubSidePanel.drawer.toggle();
                 }
                 this.isProductPage = true;
                 this.activePanel("isAddNewProduct");
                 return;
              } break;                
            }
            this._appStateService.detectClickOnSubPanel.next({selectMolenigInstance: true});
        }); 

        this.selectedSideSubPanelValueSubscription$ =  this._appStateService.selectedSideSubPanelValue.subscribe( (data: any) =>{
          if(!data || !data?.element)return;

          this.isCreateModel = false;   
          this.resetModelingObjects();
          
          this.currentModelInstanceName = data?.element;
          if(this.currentModelName == "company"){
            this.getCustomerByNameCompany(data);
          };
          if(this.currentModelName == "warehouse"){
            this.getWarehouseByName(data);
          };
          if(this.currentModelName == "currency"){
            this.getCurrencyByName(data);
          };
          if(this.currentModelName == "pricelist"){
            this.getPriceListWithValuesByName(data);
          };
          if(this.currentModelName == "pricelistbase"){
            this.getPriceListByName(data);
          };
        });

        this.refreshListOfPtopsofSubPanelSubscription$ =  this._appStateService.refreshListOfPtopsofSubPanel.subscribe(data=>{
          this.getLastItems(data)
        });

       this.routerEvents$ = this.router.events.subscribe((event) => {
          if (event instanceof NavigationEnd) {
          }
        })
      }
      
      onToggleSidenav(event) {
        this._appStateService.detectClickOnPanel.next(true);
      }

      onToggleSidenavSub(event) {
        this._appStateService.detectClickOnSubPanel.next(true);
      }

      getCustomerByNameCompany(data){
        this.isLoadingResults = true;
        this._applicationService.getCustomerByNameCompany(data?.element).subscribe( data =>{
          this.isLoadingResults = false;
          this._appStateService.instanceOfProduct = data;
       
          this.resetModelingObjects();
          this.isCompanyPage = true;
          this._appStateService.initRightActionPanel = true;
        },
        err => {
            this.isLoadingResults = false;
            this.alertService.error(err?.message || err?.error?.message);
            console.log(err);
        });
      }

      getWarehouseByName(data){
        this.isLoadingResults = true;
        this._applicationService.getWareHouseByName(data?.element).subscribe( data =>{
          this.isLoadingResults = false;
          this._appStateService.instanceOfProduct = data;
         
          this.resetModelingObjects();
          this.isWareHousePage = true;
          this._appStateService.initRightActionPanel = true;
        },
        err => {
            this.isLoadingResults = false;
            this.alertService.error(err?.message || err?.error?.message);
            console.log(err);
        });
      }

      getCurrencyByName(data){
        this.isLoadingResults = true;
        this._applicationService.getCurrencyByName(data?.element).subscribe( data =>{
          this.isLoadingResults = false;
          this._appStateService.instanceOfProduct = data;
          this.resetModelingObjects();
          this.isCurrencyPage = true;
          this._appStateService.initRightActionPanel = true;
        },
        err => {
            this.isLoadingResults = false;
            this.alertService.error(err?.message || err?.error?.message);
            console.log(err);
        });
      }
      

      getPriceListByName(data){
        this.isLoadingResults = true;
        this._applicationService.getPriceListByName(data?.element)
                                .subscribe( data =>{
          this.isLoadingResults = false;                        
          this._appStateService.instanceOfProduct = data;
          this.resetModelingObjects();
          this.isPriceListBasePage = true;
          this._appStateService.initRightActionPanel = true;
        },
        err => {
            this.isLoadingResults = false;
            this.alertService.error(err?.error?.message || err?.message);
            console.log(err);
        });
      }

      getPriceListWithValuesByName(data){
        this.isLoadingResults = true;
        this._applicationService.getPriceListWithValuesByName(data?.element)
                                .subscribe( data =>{
          this.isLoadingResults = false;                        
          this._appStateService.instanceOfProduct = data;
          this.resetModelingObjects();
          this.isPriceListPage = true;
          this._appStateService.initRightActionPanel = true;
          this.priceListValuesData = data?.priceListValue;
        },
        err => {
            this.isLoadingResults = false;
            this.alertService.error(err?.error?.message || err?.message);
            console.log(err);
        });
      }

      activePanel(namePanel){
          if(namePanel == "createModel" && this.currentModelName){
            this.isCreateModel = true;
            this.isCompanyPage = false;
            this.isWareHousePage = false;
            this.isCurrencyPage = false;
            this.isPriceListPage = false;
            this.isPriceListBasePage = false;
            this._appStateService.selectedSideSubPanelValueName.next(undefined);
          } else if(namePanel == "isAddNewProduct"){
            this.isNewProduct = true;
            this.isEditParamProduct = false;
            this.isGridShow = false;
            this._appStateService.selectedProductName = undefined;
          }else if(namePanel == "isEditParamProduct"){
            this.isEditParamProduct = true;
            this.isNewProduct = false;
            this.isGridShow = false;
            this._appStateService.selectedProductName = undefined;
          }

          this._formEditorService.resetValueProperties();
          this.dispose();
      }

      saveModel(){
        const id = this.currentModelName + "controls-id";
        if(!this._formEditorService.isRequiredValue(id)){
          this._alertService.warning("Пожалуйста, заполните обязательные поля");
          return;
        } 
        if(this.currentModelName == "company"){
          this.saveCompany();
        }else if(this.currentModelName == "warehouse"){
          this.saveWareHouse();
        }else if(this.currentModelName == "currency"){
          this.saveCurrency();
        }else if(this.currentModelName == "pricelist"){
          this.savePriceList();
        }else if(this.currentModelName == "pricelistbase"){
          this.savePriceList();
        }

        
      }

      saveWareHouse(){
        const wareHouseBody = this.formModelInstanceToSave();
        this.isLoadingResults = true;
        this._applicationService.addWareHouse(wareHouseBody)
            .subscribe(response => {
                this.isLoadingResults = false;
                this.alertService.success(response.message);
                this._formEditorService.resetValueProperties();
                this.isCreateModel = false;
                this._appStateService.selectedSidePaneModelingValue.next("WareHouse");
            },
            err => {
              this.isLoadingResults = false;
                this.alertService.error(err?.error?.message || err?.message);
                console.log("----  error addCompany");
                console.log(err);
            });
      }

      saveCompany(){ 
        const  companyBody = this.formModelInstanceToSave();
     
        this.isLoadingResults = true;
        this._applicationService.addCompany(companyBody)
            .subscribe(response => {
                this.isLoadingResults = false;
                this.alertService.success(response.message);
                //this.isAddProduct = false;
                this._formEditorService.resetValueProperties();
                this.getCompanies();
                this.isCreateModel = false;
                this._appStateService.selectedSidePaneModelingValue.next("Company");
            },
            err => {
                this.isLoadingResults = false;
                this.alertService.error(err?.error?.message || err?.message);
                console.log(err);
            });

      }

      saveCurrency(){ 
        const  companyBody = this.formModelInstanceToSave();
        this.isLoadingResults = true;
        this._applicationService.addСurrency(companyBody)
            .subscribe(response => {
                this.alertService.success(response.message);
                this.isLoadingResults = false;
                this._formEditorService.resetValueProperties();
                this.isCreateModel = false;
                this._appStateService.selectedSidePaneModelingValue.next("Currency");
            },
            err => {
                this.isLoadingResults = false;
                this.alertService.error(err?.message || err?.error?.message);
                console.log("----  error saveCurrency");
                console.log(err);
            });

      }

      savePriceList(){ 
        const priceListBody = this.formModelInstanceToSave();
        this.isLoadingResults = true;
        this._applicationService.addPriceList(priceListBody)
            .subscribe(response => {
                this.isLoadingResults = false;
                this.alertService.success(response.message);
                this._formEditorService.resetValueProperties();
                this.isCreateModel = false;
                if(this.currentModelName == "pricelist"){
                  this._appStateService.selectedSidePaneModelingValue.next("PriceList");
                }
                if(this.currentModelName == "pricelistbase"){
                  this._appStateService.selectedSidePaneModelingValue.next("PriceListBase");
                }
            },
            err => {
                this.isLoadingResults = false;
                this.alertService.error(err?.message || err?.error?.message);
                console.log(err);
            });

      }

      getLastItems(getItems){
        if(getItems?.lastItems == "company"){
          this.getCompanies();
          this.isCompanyPage = false;
        }
      }

      getCompanies(){
        this.isLoadingResults = true;
        this._applicationService.getCompaniesNames().subscribe(companies => {
          this.isLoadingResults = false;
          if(companies){  
            this.companyIntems = companies;         
            // this.companyIntems = companies.map( item => item?.nameCompany);
          }
        },
        err => {
            this.isLoadingResults = false;
            this.alertService.error(err?.message || err?.error?.message);
            console.log("----  error addCompany");
            console.log(err);
        });
      }

      getWareHouses(){
        this.isLoadingResults = true;
        this._applicationService.getWareHouses().subscribe(WareHouses => {
          this.isLoadingResults = false;
          if(WareHouses){
             this.companyIntems = WareHouses.map(item => item?.name);
          }
        },
        err => {
            this.isLoadingResults = false;
            this.alertService.error(err?.message || err?.error?.message);
            console.log(err);
        });
      }

      getPriceList(){
        this.isLoadingResults = true;
        this._applicationService.getPriceLists().subscribe(currency => {
          this.isLoadingResults = false;
          if(currency){
             this.companyIntems = currency.map(item => item?.name);
          }
        },
        err => {
          this.isLoadingResults = false;
          this.alertService.error(err?.message || err?.error?.message);
          console.log(err);
        });
      }

      getProductTypes(){
        this.isLoadingResults = true;
        this._applicationService.getProductTypes().subscribe(result => {
          this.isLoadingResults = false;
          // let producTypes = result.map(item =>{
          //     return {Name: item, DisplayName: item}
          // });
          this.companyIntems = result;
        },
        err => {
          this.isLoadingResults = false;
          this.alertService.error(err?.message || err?.error?.message);
          console.log(err);
        })
      }

      getCurrencies(){
        this.isLoadingResults = true;
        this._applicationService.getCurrencies().subscribe(currency => {
          this.isLoadingResults = false;
          if(currency){
             this.companyIntems = currency.map(item => item?.currencyName);
          }
        },
        err => {
            this.isLoadingResults = false;
            this.alertService.error(err?.error?.message);
            console.log(err);
        });
      }

      addPriceValue(){

      }

      savePriceValue(){
        const instanceDataList = this.getInstanceDataListToSave();
        let instanceDataP = instanceDataList['OptionOfPriceListProduct'];
        let instanceDataPList = instanceDataList['pricelistContent'];
       
        instanceDataP.PriceListName = instanceDataPList['name']?.value;

        let body = this.formPriceListValue();
        this.isLoadingResults = true;
        this._applicationService.addPriceListValues(body)
        .subscribe(response => {
              this.isLoadingResults = false;
              this.alertService.success(response.message);
              this._formEditorService.resetValueProperties();
              this.isCreateModel = false;
              let pl = {
                "element": instanceDataP.PriceListName
              }
              this.getPriceListWithValuesByName(pl);
            //  priceListValuesData
              //this._appStateService.refreshGridData.next(true);
        },
        err => {
              this.isLoadingResults = false;
              this.alertService.error(err?.error?.message);
              console.log(err);
        });
      }


      formPriceListValue(){
        const instanceDataList = this.getInstanceDataListToSave();
        let instanceDataP = instanceDataList['OptionOfPriceListProduct'];
        let instanceDataPList = instanceDataList['pricelistContent'];

        const productName = instanceDataP.name?.value == undefined ? "" : instanceDataP.name?.value;
        const parameters = instanceDataP.parameters;


        let parm = [];
        if(instanceDataP.parameters?.filterenddimension){
           instanceDataP.filterenddimension = instanceDataP.parameters?.filterenddimension;
           delete instanceDataP.parameters?.filterenddimension;
        }

        if(instanceDataP.parameters?.filterstartdimension){
          instanceDataP.filterstartdimension = instanceDataP.parameters?.filterstartdimension;
          delete instanceDataP.parameters?.filterstartdimension;
        }
        

        if(instanceDataP?.parameters){
          for(let item in instanceDataP?.parameters){
            parm.push(instanceDataP?.parameters[item]);
          }
          instanceDataP.parameters = parm;
        } 
    
        const currencyName = {
          name: "currencyname",
          navPriority: 1,
          type: "text",
          value: "BYN"
        }

        instanceDataP["currencyname"] = currencyName;

 
        instanceDataP.PriceListName = instanceDataPList["name"]?.value;


        return JSON.stringify(instanceDataP);

      }

      // Product

      // Edit Product
      saveEditedProduct(){
        const parameters = this._formEditorService.instanceData?.parameters;
        if(!parameters){
          this._alertService.warning("You did not change and add any parameters");
          return;
        }
        const message = this.formMessageForEditedProduct("saveEditedProduct");
        const title = "Are you sure you want to save the following parameters?";
        this.isLoadingResults = true;
        this._alertService.warningModalObject(message, title).subscribe(result =>{
          this.isLoadingResults = false;
          if(result){
             const product = this.formProductInstanceToSave();
             this._applicationService.updateProductParameters(product)
             .subscribe(response => {
                 this.alertService.success(response.message);
                 // refresh
                 this.isEditParamProduct = false;
                 this._formEditorService.resetValueProperties();
                 setTimeout(() => {             
                    this.activePanel("isEditParamProduct");
                 }, 0);
                 const productName = this._formEditorService.instanceData?.name?.value == undefined ? 
                            "" : this._formEditorService.instanceData?.name?.value;
                //new valueUpdatedData(this._property.name, convertedValue, this.typeName);
              //   this._formEditorService.valueUpdated.next( new valueUpdatedData(productName, "", ""))
             },
             err => {
                 this.isLoadingResults = false;
                 this.alertService.error(err.error.message);
                 console.log("----  error save parameters");
                 console.log(err);
             });
          }
    
        });
      }

      formMessageForEditedProduct(typeAction:string){
        const productName = this._formEditorService.instanceData?.name?.value == undefined ? 
        "" : this._formEditorService.instanceData?.name?.value;
        const parameters = this._formEditorService.instanceData?.parameters;

        console.log(productName);

        let message = [{
          "name":"Product name - ",
          "value": productName,
          "additionalInfo":"",
          "pri":""
        }]
        
        if(typeAction == "saveProduct"){
          const quantity = this._formEditorService.instanceData?.quantity?.value;
          message.push(
            {
              "name":"Product quantity - ",
              "value": quantity,
              "additionalInfo":"",
              "pri":""
            }
          );
       
        }
        for(let parm in parameters){
          let newName = parameters[parm].newName != "" ? "the name was changed to - " + parameters[parm].newName: "";
          let deletedLabel =  parameters[parm]?.isDeleted ? " - the parameter was DELETED":""    
          
          message.push({
            "name":"Parameter name - ",
            "value": parameters[parm].name,
            "additionalInfo": newName + deletedLabel,
            "pri": "  priority - " + parameters[parm].navPriority
          });
        }

        return message;
      }

      formProductInstanceToSave(){
        let parm = [];
        let productData = Object.assign({}, this._formEditorService.instanceData);
        
        const primeCostBYN = this._formEditorService.instanceData["primecost"]?.value;
        const primeCostEUR = this._formEditorService.instanceData["primecosteur"]?.value;
        const primeCostUSD = this._formEditorService.instanceData["primecostusd"]?.value;
        const customCurrencyRateEUR = this._formEditorService.instanceData["currencyrateeur"]?.value;
       
       
        if(primeCostBYN && this._appStateService.Cur_OfficialRate_EUR && 
          this._appStateService.Cur_OfficialRate_USD){
          
          const primeCostEUR = {
            name: "primecosteur",
            navPriority: 1,
            type: "text",
            value: (primeCostBYN / this._appStateService.Cur_OfficialRate_EUR).toFixed(2) + ""
          }
          productData["primecosteur"] = primeCostEUR;
          const primeCostUSD = {
            name: "primecostusd",
            navPriority: 1,
            type: "text",
            value: (primeCostBYN / this._appStateService.Cur_OfficialRate_USD).toFixed(2) + ""
          }
          productData["primecostusd"] = primeCostUSD;
        }

        if(!primeCostBYN && primeCostEUR){
         
          const primecost = {
            name: "primecost",
            navPriority: 1,
            type: "text",
            value: (primeCostEUR * this._appStateService.Cur_OfficialRate_EUR).toFixed(2) + ""
          }

          productData["primecost"] = primecost;

          const primeCostUSD = {
            name: "primecostusd",
            navPriority: 1,
            type: "text",
            value: ((primeCostEUR * this._appStateService.Cur_OfficialRate_EUR) / this._appStateService.Cur_OfficialRate_USD).toFixed(2) + ""
          }
          productData["primecostusd"] = primeCostUSD;

        }


        if(!primeCostBYN && primeCostUSD){
         
          const primecost = {
            name: "primecost",
            navPriority: 1,
            type: "text",
            value: (primeCostUSD * this._appStateService.Cur_OfficialRate_USD).toFixed(2) + ""
          }
          productData["primecost"] = primecost;

          const primeCostEUR = {
            name: "primecosteur",
            navPriority: 1,
            type: "text",
            value: ((primeCostUSD * this._appStateService.Cur_OfficialRate_USD) / this._appStateService.Cur_OfficialRate_EUR).toFixed(2) + ""
          }
          productData["primecosteur"] = primeCostEUR;
        }
        
        if(customCurrencyRateEUR){
          const primeCostEUR = {
            name: "primecosteur",
            navPriority: 1,
            type: "text",
            value: (primeCostBYN / customCurrencyRateEUR).toFixed(2) + ""
          }
          productData["primecosteur"] = primeCostEUR;
        }
        
        if(productData?.parameters){
          for(let item in productData?.parameters){
            parm.push(productData?.parameters[item]);
          }
          productData.parameters = parm;
        } 
    
        const currencyName = {
          name: "currencyname",
          navPriority: 1,
          type: "text",
          value: "BYN"
        }

        productData["currencyname"] = currencyName;

        return JSON.stringify(productData);
      }

      // New product

      saveProduct(){
        const message = this.formMessageForProduct();
        const title = "Are you sure you want to add the following product?";
        this.isLoadingResults = true;
        this._alertService.warningModalObject(message, title).subscribe(result =>{
          if(result){
              const product = this.formProductInstanceToSave();
              console.log(product);
              this._applicationService.addNewProduct(product)
                  .subscribe(response => {
                    this.isLoadingResults = false;
                      this.alertService.success(response.message);
                      this.isNewProduct = false;
                      this._formEditorService.resetValueProperties();
                      setTimeout(() => {             
                          this.activePanel("isAddNewProduct");
                      }, 0);
                            
                  },
                  err => {
                      this.isLoadingResults = false;
                      this.alertService.error(err.error.message);
                      console.log("----  error addNewProduct");
                      console.log(err);
                  });
          }
    
        });
      }

      formMessageForProduct(){
        const productName = this._formEditorService.instanceData?.name?.value == undefined ? 
                            "" : this._formEditorService.instanceData?.name?.value;
        const quantity = this._formEditorService.instanceData?.quantity?.value;
        const parameters = this._formEditorService.instanceData?.parameters;
        let message = [{
          "name":"Product name - ",
          "value":productName,
          "pri":""
        },
        {
          "name":"Product quantity - ",
          "value":quantity,
          "pri":""
        }
       ]

        for(let parm in parameters){
          message.push({
            "name":"Parameter name - ",
            "value": parameters[parm].name,
            "pri": " priority - " + parameters[parm].navPriority
          });
        }
        return message;
      }

      ngOnDestroy(){
        this.dispose();
        this._formEditorService.listCreatedField = [];
        this.routerEvents$.unsubscribe();
        this.selectedSidePaneModelingValueSubscription$.unsubscribe();
        this.selectedSideSubPanelValueSubscription$.unsubscribe();
        this.refreshListOfPtopsofSubPanelSubscription$.unsubscribe();
      }
    
      dispose(){
        this._appStateService.initRightActionPanel = false;
        this._appStateService.instanceOfProduct = undefined;
        this.currentModelInstanceName = undefined;
       // this._appStateService.selectedInstancePanel = undefined;
        //this.currentModelName = undefined;
      }

      private resetModelingObjects(){
          this.isWareHousePage = false;
          this.isCompanyPage = false;
          this.isCurrencyPage = false;
          this.isPriceListPage = false;
          this.isPriceListBasePage = false;
          this.isProductPage = false;
          
      }
      
      private  formModelInstanceToSave(){
          let productData = Object.assign({}, this._formEditorService.instanceData);
          return JSON.stringify(productData);
      }

      private  getInstanceDataListToSave(){
        return Object.assign({}, this._formEditorService.instanceDataList);;
      }

}
