import { Component, EventEmitter, Output } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { NavigationEnd, Router } from '@angular/router';
import { BehaviorSubject, of, Subject } from 'rxjs';

import { AlertService } from '../alert/alert.service';
import { IgetProductTypeInstancesRequest, IPaginationAction, ProductTypeInstancesRequest, valueUpdatedData } from '../common/objects/common';
import { ApplicationService } from '../common/services/application.service';
import { AppStateService } from '../common/services/appState.service';
import { FormEditorService } from '../common/services/formEditor.service';

@Component({
  selector: 'app-modeling-component',
  styleUrls: ['./modeling.component.css'],
  templateUrl: './modeling.component.html'
  
})
export class ModelingComponent {

      public isCreateModel: boolean = false;
      public isCompanyPage: boolean = false;
      public isWareHousePage: boolean = false;
      public isCurrencyPage: boolean = false;
      public isEditParamProduct: boolean = false;
      public instanceName:string = 'Product';
      public sidenavToggle;
      public namesParoduct: any = undefined;
      public isGridShow:boolean = false;
      public gridProductData = undefined;
      public gridMetadaType = "ProductGrid";
      public isLoadingResults = false;
      public currentModelName : string;
      public currentModelInstanceName : string;
      public modelingIntems : any = ["Company","Currency", "PriceList", "WareHouse"];
      public companyIntems : any = [];
    
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
      constructor(private _alertService : AlertService,
                  private _applicationService : ApplicationService, 
                  private _formEditorService : FormEditorService,
                  private alertService:AlertService,
                  private _appStateService:AppStateService,
                  private router: Router){

      


      this.isCreateModel = false;
      }
                         
      ngOnInit(){
        this._appStateService.detectClickOnPanel.next(true);
        this._appStateService.selectedSidePaneModelingValue.subscribe((nameModel:string) => {
            console.log("------------- selectedSidePanel nameModel");
            console.log(nameModel);
            if(!nameModel) return; 
            this._appStateService.selectedInstancePanel = nameModel;
            this.currentModelName = nameModel.toLocaleLowerCase();
            this.currentModelInstanceName = undefined; 
            this.isLoadingResults = true;
            this.isCreateModel = false;
            this.isCompanyPage = false;
            this.isWareHousePage = false;
            this.isCurrencyPage = false;
            
            this._appStateService.instanceOfProduct = undefined;
            this._appStateService.initRightActionPanel = false;
            this.companyIntems = [];
            if(nameModel == "Company"){
              this.getCompanies();
            }
            if(nameModel == "WareHouse"){
              this.getWareHouses();
            }
            if(nameModel == "Currency"){
              this.getCurrencies();
            }

            
            this._appStateService.detectClickOnSubPanel.next(true);
        }); 

        this._appStateService.selectedSideSubPanelValue.subscribe( (data: any) =>{
          if(!data)return;
          this.isCreateModel = false;
          this.isCompanyPage = false;
          this.isWareHousePage = false;
          this.isCurrencyPage = false;
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

        });

        this._appStateService.refreshListOfPtopsofSubPanel.subscribe(data=>{
          this.getLastItems(data)
        });

        this.router.events.subscribe((event) => {
          console.log("--------------------------------   this.router.events");
          if (event instanceof NavigationEnd) {
            // this.currentModelName = "";
            // this.currentModelInstanceName = "";
          }
        })
      }

  
      getCustomerByNameCompany(data){
        this._applicationService.getCustomerByNameCompany(data?.element).subscribe( data =>{
          this._appStateService.instanceOfProduct = data;
          this.isCompanyPage = true;
          this.isWareHousePage = false;
          this.isCurrencyPage = false;
          this._appStateService.initRightActionPanel = true;
        })
      }

      getWarehouseByName(data){
        this._applicationService.getWareHouseByName(data?.element).subscribe( data =>{
          this._appStateService.instanceOfProduct = data;
          this.isWareHousePage = true;
          this.isCompanyPage = false;
          this.isCurrencyPage = false;
          this._appStateService.initRightActionPanel = true;
        })
      }

      getCurrencyByName(data){
        this._applicationService.getCurrencyByName(data?.element).subscribe( data =>{
          this._appStateService.instanceOfProduct = data;
          this.isWareHousePage = false;
          this.isCompanyPage = false;
          this.isCurrencyPage = true;
          this._appStateService.initRightActionPanel = true;
        })
      }

      activePanel(namePanel){
          if(namePanel == "createModel" && this.currentModelName){
            this.isCreateModel = true;
            this.isCompanyPage = false;
            this.isWareHousePage = false;
            this.isCurrencyPage = false;
            this._appStateService.selectedSideSubPanelValueName.next(undefined);
          } 
          this._formEditorService.resetValueProperties();
          this.dispose();
      }

      saveModel(){
        const id = this.currentModelName + "controls-id";
        if(!this._formEditorService.isRequiredValue(id)){
          this._alertService.warning("Please fill out all required fields");
          return;
        } 
        if(this.currentModelName == "company"){
          this.saveCompany();
        }else if(this.currentModelName == "warehouse"){
          this.saveWareHouse();
        }else if(this.currentModelName == "currency"){
          this.saveCurrency();
        }

        
      }

      saveWareHouse(){
        const wareHouseBody = this.formModelInstanceToSave();
        console.log(wareHouseBody);
        this._applicationService.addWareHouse(wareHouseBody)
            .subscribe(response => {
                this.alertService.success(response.message);
                console.log("Success new wareHouse has added.");
                console.log(response);
                this._formEditorService.resetValueProperties();
                this.isCreateModel = false;
                this._appStateService.selectedSidePaneModelingValue.next("WareHouse");
            },
            err => {
                this.alertService.error(err.error.message);
                console.log("----  error addCompany");
                console.log(err);
            });
      }

      saveCompany(){ 
        const  companyBody = this.formModelInstanceToSave();
        console.log(companyBody);
        this._applicationService.addCompany(companyBody)
            .subscribe(response => {
                this.alertService.success(response.message);
                console.log("Success  new company has added.");
                console.log(response);
                //this.isAddProduct = false;
                this._formEditorService.resetValueProperties();
                this.getCompanies();
                this.isCreateModel = false;
                this._appStateService.selectedSidePaneModelingValue.next("Company");
            },
            err => {
                this.alertService.error(err.error.message);
                console.log("----  error addCompany");
                console.log(err);
            });

      }

      saveCurrency(){ 
        const  companyBody = this.formModelInstanceToSave();
        console.log(companyBody);
        this._applicationService.addСurrency(companyBody)
            .subscribe(response => {
                this.alertService.success(response.message);
                console.log("Success  new company has added.");
                console.log(response);
                //this.isAddProduct = false;
                this._formEditorService.resetValueProperties();
                this.isCreateModel = false;
                this._appStateService.selectedSidePaneModelingValue.next("Currency");
            },
            err => {
                this.alertService.error(err.error.message);
                console.log("----  error saveCurrency");
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
        this._applicationService.getCompanies().subscribe(companies => {
          if(companies){
             this.companyIntems = companies.map( item => item?.nameCompany);;
          }
        },
        err => {
            this.alertService.error(err.error.message);
            console.log("----  error addCompany");
            console.log(err);
        });
      }

      getWareHouses(){
        this._applicationService.getWareHouses().subscribe(WareHouses => {
          if(WareHouses){
             this.companyIntems = WareHouses.map(item => item?.name);
          }
        },
        err => {
            this.alertService.error(err.error.message);
            console.log("----  error get WareHouses");
            console.log(err);
        });
      }

      getCurrencies(){
        this._applicationService.getCurrencies().subscribe(currency => {
          if(currency){
             this.companyIntems = currency.map(item => item?.currencyName);
          }
        },
        err => {
            this.alertService.error(err.error.message);
            console.log("----  error get currency");
            console.log(err);
        });
      }

      onDestroy(){
        this.dispose();
      }
    
      dispose(){
        this._appStateService.initRightActionPanel = false;
        this._appStateService.instanceOfProduct = undefined;
      }


    private  formModelInstanceToSave(){
        let productData = this._formEditorService.instanceData;
        return JSON.stringify(productData);
    }

}
