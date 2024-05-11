import { ChangeDetectorRef, Component, ElementRef, HostListener, ViewChild } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { BehaviorSubject, Subscription } from 'rxjs';
import { AlertService } from '../alert/alert.service';
import { IgetProductTypeInstancesRequest, IPaginationAction, ProductTypeInstancesRequest } from '../common/objects/common';
import { ApplicationService } from '../common/services/application.service';
import { AppStateService } from '../common/services/appState.service';
import { FormEditorService } from '../common/services/formEditor.service';
import { LabelsService } from '../common/services/labels.service';
import { CurrenciesService } from '../common/services/currencies.service';

@Component({
  selector: 'app-product-component',
  styleUrls: ['./product.component.css'],
  templateUrl: './product.component.html'
  
})
export class ProductComponent {
      
      @ViewChild("appSidePanel") appSidePanel: any;

      public isAddProduct: boolean = false;
      public isNewProduct: boolean = false;
      public isEditParamProduct: boolean = false;
      public isReportProduct: boolean = false;
      public instanceName:string = 'Product';
      public sidenavToggle;
      public namesParoduct;
      public isGridShow:boolean = false;
      public gridProductData;
      public gridMetadaType = "ProductGrid";
      public isLoadingResults = false;
      public currentProductName :string;
      public gridProductReportData;
      private routerEvents$:Subscription;
      private selectedSidePanelValueSubscription$:Subscription;
      private changedPageGridSubscription$:Subscription;
      private refreshAddProductSectionSubscription$:Subscription;
      private isActiveRightActionPanelSubscription$:Subscription;
      


      constructor(private _alertService : AlertService,
                  private _applicationService : ApplicationService, 
                  private _formEditorService : FormEditorService,
                  private alertService:AlertService,
                  private _appStateService:AppStateService,
                  private router: Router,
                  private changeDetector: ChangeDetectorRef,
                  public _labelsService: LabelsService,
                  public _currenciesService:CurrenciesService){}
                     
      onToggleSidenav(event) {
        
         this._appStateService.detectClickOnPanel.next(true);
         this.sidenavToggle = event;
         if(!this.namesParoduct){
             this._applicationService.getProductTypes().subscribe(result => {
                let producTypes = result.map(item =>{
                    return {Name: item, DisplayName: item}
                });
                this.namesParoduct = producTypes;
             })
         }
      }

      ngAfterViewInit() {
        this._appStateService.selectedSidePanelValue.next("Шток хромированный");
        this.changeDetector.detectChanges();
      }
      ngAfterContentChecked() : void {
        this.changeDetector.detectChanges();
      }
      
      ngOnInit(){
        this._appStateService.optionValue = undefined;
        this._appStateService.detectClickOnPanel =  new BehaviorSubject<boolean>(false); 
        
        this.selectedSidePanelValueSubscription$ = this._appStateService.selectedSidePanelValue.subscribe((nameProduct:string) => {
            if(!nameProduct) return; 

            this._appStateService.listFilterParameters = [];
            this._appStateService.getProductTypeInstancesRequest = new ProductTypeInstancesRequest();
            this._appStateService.changedGridOption.next(undefined);
            this._appStateService.selectedInstancePanel = nameProduct;
            this.currentProductName = nameProduct;
            this.isLoadingResults = true;
            this.getProductData(nameProduct, 1, 10);
        }); 
        
        this.changedPageGridSubscription$ = this._appStateService.changedPageGrid.subscribe((data:IPaginationAction) =>{
            if(data){
              if(data.gridName == "ProductGrid"){
                this.getProductData(this.currentProductName, 1 , data.pageSize);
              }
            }
        });
        
        this.refreshAddProductSectionSubscription$ = this._appStateService.refreshAddProductSection.subscribe(isRefresh =>{
          if(isRefresh){
            this.isAddProduct = false;
            setTimeout(() => {             
                this.activePanel("isAddProduct");
            }, 0);
          }
        });

        this.isActiveRightActionPanelSubscription$ = this._appStateService.isActiveRightActionPanel.subscribe(isActive =>{
           this._appStateService.initRightActionPanel = isActive;
        });

        this.routerEvents$ = this.router.events.subscribe((event) => {
          console.log("--------------------------------   this.router.events");
          if (event instanceof NavigationEnd) {
              this.dispose();
            //  this._formEditorService.resetValueProperties();
          }
        });

      }
      
      getProductData(nameProduct, pageNumber, pageSize){
        const getProductTypeInstancesRequest : IgetProductTypeInstancesRequest = new ProductTypeInstancesRequest();
        getProductTypeInstancesRequest.PageFilter = {
          pageNumber: pageNumber,
          pageSize: pageSize
        };
        getProductTypeInstancesRequest.NameProductType = nameProduct;
        this.isLoadingResults = true;
        this._applicationService.getProductTypeInstances2(getProductTypeInstancesRequest).subscribe(response => {
          this.isLoadingResults = false;
          this.gridProductData = response;
          this.isGridShow = false;
          setTimeout(() => {             
            this.activePanel("isGridShow");
          }, 0);         
        },
        err => {
            this.isLoadingResults = false;
            this.alertService.error(err.error.message);
            console.log(err);
        });
      }

      activePanel(namePanel){
          if(namePanel == "isAddProduct"){
            this.isAddProduct = true;
            this.isNewProduct = false;
            this.isEditParamProduct = false;
            this.isGridShow = false;
            this.isReportProduct = false;
          } else if(namePanel == "isAddNewProduct"){
            this.isNewProduct = true;
            this.isAddProduct = false;
            this.isEditParamProduct = false;
            this.isGridShow = false;
            this.isReportProduct = false;
            this._appStateService.selectedProductName = undefined;
          }else if(namePanel == "isEditParamProduct"){
            this.isEditParamProduct = true;
            this.isNewProduct = false;
            this.isAddProduct = false;
            this.isGridShow = false;
            this.isReportProduct = false;
            this._appStateService.selectedProductName = undefined;
          }
          else if(namePanel == "isGridShow"){
            this.isGridShow = true;
            this.isNewProduct = false;
            this.isAddProduct = false;
            this.isEditParamProduct = false;
            this.isReportProduct = false;
            this._appStateService.selectedProductName = undefined;
          }
          else if(namePanel == "isReportProduct"){
            this.isGridShow = false;
            this.isNewProduct = false;
            this.isAddProduct = false;
            this.isEditParamProduct = false;
            this.isReportProduct = true;
            this._appStateService.selectedProductName = undefined;
          }

          
          this._formEditorService.resetValueProperties(); 
          this.dispose();
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

      saveEditedProduct(){
        const parameters = this._formEditorService.instanceData?.parameters;
        if(!parameters){
          this._alertService.warning("You did not change and add any parameters");
          return;
        }

        console.log("----------- saveEditedProduct");
        console.log(this._formEditorService.instanceData);

        const message = this.formMessageForEditedProduct("saveEditedProduct");
        const title = "Are you sure you want to save the following parameters?";
        this.isLoadingResults = true;
        this._alertService.warningModalObject(message, title).subscribe(result =>{
          console.log(result);
          this.isLoadingResults = false;
          if(result){
             console.log("----------- confirm - ok - parameters");
             const product = this.formProductInstanceToSave();
             console.log(product);

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

      saveInstanceProduct2(){
        const id = "AddInstanceProduct" + "controls-id";
        if(!this._formEditorService.isRequiredValue(id)){
          this._alertService.warning("Please fill out all required fields");
          return;
        } 
       
        const product = this.formProductInstanceToSave();
        this.isLoadingResults = true;
        this._applicationService.addInstanceProduct(product)
            .subscribe(response => {
                this.isLoadingResults = false;
                this.alertService.success(response.message);
                // this.isAddProduct = false;
                // this._formEditorService.resetValueProperties();
                // setTimeout(() => {             
                //     this.activePanel("isAddProduct");
                // }, 0);
                      
            },
            err => {
              this.isLoadingResults = false;
                this.alertService.error(err?.error?.message || err?.message);
                console.log(err);
            });
      }

      formProductInstanceToSave(){
        let parm = [];
        let productData = Object.assign({}, this._formEditorService.instanceData);
        
        const primeCostBYN = this._formEditorService.instanceData["primecost"]?.value;
        const primeCostEUR = this._formEditorService.instanceData["primecosteur"]?.value;
        const primeCostUSD = this._formEditorService.instanceData["primecostusd"]?.value;
        const customCurrencyRateEUR = this._formEditorService.instanceData["currencyrateeur"]?.value;
       
        if(!primeCostBYN && !primeCostEUR && !primeCostUSD){
            this.alertService.warning("Введите себестоимость продукта")
        }

        if(!this._appStateService.Cur_OfficialRate_EUR || 
           !this._appStateService.Cur_OfficialRate_USD){
           this._currenciesService.getLastRate();
        }
        
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

      loadProductReport(){
        const id = "ReportCommanProduct" + "controls-id";
        if(!this._formEditorService.isRequiredValue(id)){
          this._alertService.warning("Please fill out all required fields");
          return;
        } 

        this.isLoadingResults = true;
        const reportFilterRequest = this.formFilterForReport();
        this._applicationService.loadProductReport(reportFilterRequest)
          .subscribe(response => {
            this.isLoadingResults = false;
            this.isGridShow = false;
            this.gridProductReportData = response;
         //   this.gridOrderData = false;
        //    this.gridOrderReportData = response;
          },
          err => {
              this.isLoadingResults = false;
              this.alertService.error(err?.error?.message || err?.error);
              console.log("----  error loadOrederReport");
              console.log(err);
          });
      }

      formFilterForReport(){
          let orderReportData = Object.assign({}, this._formEditorService.instanceData);
          const parameters = this._formEditorService.instanceData?.parameters;
          let param = [];
          if(orderReportData?.parameters){
            for(let item in orderReportData?.parameters){
              param.push(orderReportData?.parameters[item]);
            }
            orderReportData.parameters = param;
          } 
          
          return orderReportData;
      }
      ngOnDestroy(){

        this.dispose();
        this._formEditorService.listCreatedField = [];

        this.routerEvents$.unsubscribe();
        this.selectedSidePanelValueSubscription$.unsubscribe();
        this.changedPageGridSubscription$.unsubscribe();
        this.refreshAddProductSectionSubscription$.unsubscribe();
        this.isActiveRightActionPanelSubscription$.unsubscribe();
      }
      
      dispose(){
        this._appStateService.initRightActionPanel = false;
        this._appStateService.instanceOfProduct = undefined;
      }
}
