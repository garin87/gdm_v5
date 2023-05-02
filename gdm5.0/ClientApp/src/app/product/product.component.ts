import { AfterContentChecked, ChangeDetectorRef, Component, EventEmitter, Output } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { NavigationEnd, NavigationStart, Router } from '@angular/router';
import { BehaviorSubject, of, Subject } from 'rxjs';

import { AlertService } from '../alert/alert.service';
import { IgetProductTypeInstancesRequest, IPaginationAction, ProductTypeInstancesRequest, valueUpdatedData } from '../common/objects/common';
import { ApplicationService } from '../common/services/application.service';
import { AppStateService } from '../common/services/appState.service';
import { FormEditorService } from '../common/services/formEditor.service';

@Component({
  selector: 'app-product-component',
  styleUrls: ['./product.component.css'],
  templateUrl: './product.component.html'
  
})
export class ProductComponent {

      public isAddProduct: boolean = false;
      public isNewProduct: boolean = false;
      public isEditParamProduct: boolean = false;
      public instanceName:string = 'Product';
      public sidenavToggle;
      public namesParoduct: any = undefined;
      public isGridShow:boolean = false;
      public gridProductData = undefined;
      public gridMetadaType = "ProductGrid";
      public isLoadingResults = false;
      public currentProductName :string;
     //public namesParoduct = new BehaviorSubject<string[]>(undefined);

      constructor(private _alertService : AlertService,
                  private _applicationService : ApplicationService, 
                  private _formEditorService : FormEditorService,
                  private alertService:AlertService,
                  private _appStateService:AppStateService,
                  private router: Router,
                  private changeDetector: ChangeDetectorRef,){}
                         
      onToggleSidenav(event) {
         console.log("---- first event ============= onToggleSidenav");
         console.log(event);
         this._appStateService.detectClickOnPanel.next(true);
         this.sidenavToggle = event;
         if(!this.namesParoduct){
             this._applicationService.getProductTypes().subscribe(result => {
                console.log("-------- getProductTypes");
                console.log(result);

               this.namesParoduct = result;
               
               // this.namesParoduct.next(result);
             })
         }
      }
      ngAfterViewInit() {
        this.changeDetector.detectChanges();
      }
      ngAfterContentChecked() : void {
        this.changeDetector.detectChanges();
      }
      
      ngOnInit(){
        this._appStateService.optionValue = undefined;
        this._appStateService.detectClickOnPanel =  new BehaviorSubject<boolean>(false); 
        this._appStateService.selectedSidePanelValue.subscribe((nameProduct:string) => {
            console.log("------------- selectedSidePanelValue ");
            console.log(nameProduct);
            if(!nameProduct) return; 
            this._appStateService.selectedInstancePanel = nameProduct;
            this.currentProductName = nameProduct;
            this.isLoadingResults = true;
            this._appStateService.changedGridOption = new BehaviorSubject<any>(undefined);
            this.getProductData(nameProduct, 1, 10);
            
        }); 

        this.router.events.subscribe((event) => {
          console.log("--------------------------------   this.router.events");
          if (event instanceof NavigationEnd) {
              this.dispose();
              this._formEditorService.resetValueProperties();
           //   this._appStateService.detectClickOnPanel.next(true);
          }
        });
        
        this._appStateService.changedPageGrid.subscribe((data:IPaginationAction) =>{
            if(data){
              console.log("--------------- pag gri product data ");
              console.log(data);
              if(data.gridName == "ProductGrid"){
                this.getProductData(this.currentProductName, 1 , data.pageSize);
              }
            }
        });
        
        this._appStateService.refreshAddProductSection.subscribe(isRefresh =>{
          if(isRefresh){
            this.isAddProduct = false;
            setTimeout(() => {             
                this.activePanel("isAddProduct");
            }, 0);
          }
        })
      }
      
      getProductData(nameProduct, pageNumber, pageSize){
        const getProductTypeInstancesRequest : IgetProductTypeInstancesRequest = new ProductTypeInstancesRequest();
        getProductTypeInstancesRequest.PageFilter = {
          pageNumber: pageNumber,
          pageSize: pageSize
        };
        getProductTypeInstancesRequest.NameProductType = nameProduct;

        this._applicationService.getProductTypeInstances2(getProductTypeInstancesRequest).subscribe(response => {
          console.log("------------------------ getProductData --------- getProductTypeIntances");
          console.log(response);
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

            console.log("----  error getProductTypeIntances");
            console.log(err);
        });
      }

      activePanel(namePanel){

          if(namePanel == "isAddProduct"){
            this.isAddProduct = true;
            this.isNewProduct = false;
            this.isEditParamProduct = false;
            this.isGridShow = false;
          } else if(namePanel == "isAddNewProduct"){
            this.isNewProduct = true;
            this.isAddProduct = false;
            this.isEditParamProduct = false;
            this.isGridShow = false;
            this._appStateService.selectedProductName = undefined;
          }else if(namePanel == "isEditParamProduct"){
            this.isEditParamProduct = true;
            this.isNewProduct = false;
            this.isAddProduct = false;
            this.isGridShow = false;
            this._appStateService.selectedProductName = undefined;
          }
          else if(namePanel == "isGridShow"){
            this.isGridShow = true;
            this.isNewProduct = false;
            this.isAddProduct = false;
            this.isEditParamProduct = false;
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
        this._alertService.warningModalObject(message, title).subscribe(result =>{
          console.log(result);
          if(result){
             console.log("----------- confirm - ok - parameters");
             const product = this.formProductInstanceToSave();
             console.log(product);

             this._applicationService.updateProductParameters(product)
             .subscribe(response => {
                 this.alertService.success(response.message);
                 console.log("Successful save parameters");
                 console.log(response);
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
                 this.alertService.error(err.error.message);
                 console.log("----  error save parameters");
                 console.log(err);
             });
          }
    
        });
      }
      
      saveProduct(){
        // const id = this.instanceName + "controls-id";
        // if(!this._formEditorService.isRequiredValue(id)){
        //   this._alertService.warning("Please fill out all required fields");
        //   return;
        // } 
       
        console.log("----------- saveProduct");
        console.log(this._formEditorService.instanceData);
        
        const message = this.formMessageForProduct();
        const title = "Are you sure you want to add the following product?";
        this._alertService.warningModalObject(message, title).subscribe(result =>{
          console.log("----------- confirm - ok - saveProduct");
          console.log(result);
          if(result){
              console.log(this._formEditorService.instanceData);
              const product = this.formProductInstanceToSave();
              console.log(product);
              this._applicationService.addNewProduct(product)
                  .subscribe(response => {
                      this.alertService.success(response.message);
                      console.log("Successful addNewProduct");
                      console.log(response);
                      this.isNewProduct = false;
                      this._formEditorService.resetValueProperties();
                      setTimeout(() => {             
                          this.activePanel("isAddNewProduct");
                      }, 0);
                            
                  },
                  err => {
                      this.alertService.error(err.error.message);
                      console.log("----  error addNewProduct");
                      console.log(err);
                  });
          }
    
        });
      }

      // saveInstanceProduct(){
      //   const id = "AddInstanceProduct" + "controls-id";
      //   if(!this._formEditorService.isRequiredValue(id)){
      //     this._alertService.warning("Please fill out all required fields");
      //     return;
      //   } 
       
      //   console.log("----------- saveProduct");
      //   console.log(this._formEditorService.instanceData);
        
      //   const message = this.formMessageForProduct();
      //   const title = "Do you want to save the product?";
      //   this._alertService.warningModalObject(message, title).subscribe(result =>{
      //     console.log("----------- confirm - ok - saveInstanceProduct");
      //     console.log(result);
      //     if(result){
      //         console.log(this._formEditorService.instanceData);
      //         const product = this.formProductInstanceToSave();
      //         console.log(product);
      //         this._applicationService.addInstanceProduct(product)
      //             .subscribe(response => {
      //                 this.alertService.success(response.message);
      //                 console.log("Successful addInstanceProduct");
      //                 console.log(response);
      //                 this.isAddProduct = false;
      //                 this._formEditorService.resetValueProperties();
      //                 setTimeout(() => {             
      //                     this.activePanel("isAddProduct");
      //                 }, 0);
                            
      //             },
      //             err => {
      //                 this.alertService.error(err.error.message);
      //                 console.log("----  error addInstanceProduct");
      //                 console.log(err);
      //             });
      //     }
    
      //   });
      // }

      saveInstanceProduct2(){
        const id = "AddInstanceProduct" + "controls-id";
        if(!this._formEditorService.isRequiredValue(id)){
          this._alertService.warning("Please fill out all required fields");
          return;
        } 
       
        console.log("----------- saveProduct");
        console.log(this._formEditorService.instanceData);
        const product = this.formProductInstanceToSave();
        console.log(product);
        this._applicationService.addInstanceProduct(product)
            .subscribe(response => {
                this.alertService.success(response.message);
                console.log("Successful addInstanceProduct");
                console.log(response);
                // this.isAddProduct = false;
                // this._formEditorService.resetValueProperties();
                // setTimeout(() => {             
                //     this.activePanel("isAddProduct");
                // }, 0);
                      
            },
            err => {
                this.alertService.error(err?.error?.message || err?.message);
                console.log("----  error addInstanceProduct");
                console.log(err);
            });
      }

      formProductInstanceToSave(){
        let parm = [];
        let productData = this._formEditorService.instanceData;
        
        const primeCostBYN = this._formEditorService.instanceData["primecost"]?.value;
        if(this._appStateService.Cur_OfficialRate_EUR && this._appStateService.Cur_OfficialRate_USD){
          const primeCostEUR = {
            name: "primecosteur",
            navPriority: 1,
            type: "text",
            value: (primeCostBYN / this._appStateService.Cur_OfficialRate_EUR).toFixed(2) + ""
          }
          this._formEditorService.instanceData["primecosteur"] = primeCostEUR;
          const primeCostUSD = {
            name: "primecostusd",
            navPriority: 1,
            type: "text",
            value: (primeCostBYN / this._appStateService.Cur_OfficialRate_USD).toFixed(2) + ""
          }
          this._formEditorService.instanceData["primecostusd"] = primeCostUSD;
        }
      
        if(this._formEditorService.instanceData?.parameters){
          for(let item in this._formEditorService.instanceData?.parameters){
            parm.push(this._formEditorService.instanceData?.parameters[item]);
          }
          productData.parameters = parm;
        } 
    
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

      onDestroy(){
        this.dispose();
      }
      
      dispose(){
        this._appStateService.initRightActionPanel = false;
        this._appStateService.instanceOfProduct = undefined;
      }
}
