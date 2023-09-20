
import { BehaviorSubject, Subject } from 'rxjs';
import { AlertService } from 'src/app/alert/alert.service';
import { ExecuteCommand, IExecuteCommand, valueUpdatedData } from 'src/app/common/objects/common';
import { ApplicationService } from 'src/app/common/services/application.service';
import { AppStateService,  } from 'src/app/common/services/appState.service';
import { CurrenciesService } from 'src/app/common/services/currencies.service';
import { FormEditorService } from 'src/app/common/services/formEditor.service';

export class ActionManager{

  public executeCommands : IExecuteCommand[];
  public isLoadingResults: boolean;

  constructor(private _appStateService:AppStateService,
              private _formEditorService:FormEditorService,
              private _applicationService : ApplicationService,
              private _alertService: AlertService, 
              private _currenciesService: CurrenciesService) { 
  };

  executeCommand(command:ExecuteCommand){

    if(command.command == "Edit"){

      if(command.data?.parentType == "product"){
        this.updateProductInstance();
      }
      if(command.data?.parentType == "company"){
        this.updateCompany();
      }
      if(command.data?.parentType == "warehouse"){
        this.updateWarehouse();
      }

      if(command.data?.parentType == "currency"){
        this.updateCurrency();
      }

    }

    if(command.command == "Delete"){
      if(command.data?.parentType == "product"){
        this.deleteProductInstance();
      }
      if(command.data?.parentType == "company"){
        this.deleteCompanyInstance();
      }
      if(command.data?.parentType == "warehouse"){
        this.deleteWareHouseInstance();
      }
      if(command.data?.parentType == "currency"){
        this.deleteCurrencyInstance();
      }
      if(command.data?.parentType == "cartProduct"){
        this.deleteCartProduct();
      }
      if(command.data?.parentType == "orderProduct"){
        this.deleteOrderProduct();
      }
    }
    if(command.command == "Order"){
      if(command.data?.parentType == "product"){
          
          this.orderProduct();
      }
    }

    if(command.command == "AddToCart"){
      if(command.data?.parentType == "product"){
         
          this.addToCartProduct();
      }
    }
  }  
    
  updateProductInstance(){
    const updatedInstance = this.formProductInstanceToSave();
    this._appStateService.LoadingGridResults.next(true);
    this._applicationService.updateProductInstance(updatedInstance)
    .subscribe(response => {
      this._appStateService.LoadingGridResults.next(false);
       this._alertService.success(response.message);
       this.dispose();
       this._appStateService.refreshGridData.next(true);
     },
     err => {
       this._appStateService.LoadingGridResults.next(false);
       this._alertService.error(err.error.message);
     });
  }

  deleteProductInstance(){
    const productId = this._appStateService.instanceOfProduct?.productId;
    this._appStateService.LoadingGridResults.next(true);
    this._applicationService.deleteProductInstance(productId)
    .subscribe(response => {
       this._appStateService.LoadingGridResults.next(false);
       this._alertService.success(response.message);
       
       this.dispose();
       this._appStateService.refreshGridData.next(true);
     },
     err => {
       this._appStateService.LoadingGridResults.next(false);
       this._alertService.error(err.error.message);
    });
  }

  orderProduct(){
 
    const orderData = this.formOrderProduct();
    this._appStateService.LoadingGridResults.next(true);
    this._applicationService.addOrderProduct(orderData)
    .subscribe(response => {
       this._appStateService.LoadingGridResults.next(false);
       this._alertService.success(response.message);
       this.dispose();
       this._appStateService.refreshGridData.next(true);
       this._appStateService.isActiveRightActionPanel.next(false);
      // this._appStateService.selectedSidePanelValue.next(this._appStateService.selectedInstancePanel);
     },
     err => {
         this._appStateService.LoadingGridResults.next(false);
         this._alertService.error(err.error.message);
     });
  }

  addToCartProduct(){
    const orderData = this.formOrderProduct();
    this._appStateService.LoadingGridResults.next(true);
   
    this._applicationService.addToCartOrder(orderData)
      .subscribe(response => {
        this._alertService.success(response.message);
        this._appStateService.LoadingGridResults.next(false);
        this.dispose();
      // this._appStateService.selectedSidePanelValue.next(this._appStateService.selectedInstancePanel);
        this._appStateService.cartProductCount.next(1);
        this._appStateService.refreshGridData.next(true);
        this._appStateService.isActiveRightActionPanel.next(false);
        // this._appStateService.refreshGridData.next(true);
      },
      err => {
          this._appStateService.LoadingGridResults.next(false);
          this._alertService.error(err.error.message);
    });
  }

  updateCompany(){
    const updatedInstance = this.formCompanyInstanceToSave();
    this._applicationService.updateCompany(updatedInstance)
    .subscribe(response => {
       this._alertService.success(response.message);
       this._appStateService.refreshSubPanelContentData.next(true);
       this.dispose();
     },
     err => {
         this._alertService.error(err.error.message);
     });
  }

  updateCurrency(){
    const updatedInstance = this.forCurrencyInstanceToSave();
    this._applicationService.updateСurrency(updatedInstance)
    .subscribe(response => {
       this._alertService.success(response.message);
       this._appStateService.refreshSubPanelContentData.next(true);
       this.dispose();
     },
     err => {
         this._alertService.error(err.error.message);
     });
  }
  
  updateWarehouse(){
    const updatedInstance = this.forWareHouseInstanceToSave();
    this._applicationService.updateWareHouse(updatedInstance)
    .subscribe(response => {
       this._alertService.success(response.message);
       this._appStateService.refreshSubPanelContentData.next(true);
       this.dispose();
     },
     err => {
         this._alertService.error(err.error.message);
     });
  }

  deleteCompanyInstance(){
    const companyId = this._appStateService.instanceOfProduct?.id;
    this._applicationService.deleteCompany(companyId)
    .subscribe(response => {
       this._alertService.success(response.message);
       this.dispose();
       this._appStateService.refreshListOfPtopsofSubPanel.next({lastItems:"company"});
     },
     err => {
         this._alertService.error(err.error.message);
    });
  }

  deleteCurrencyInstance(){
    const currencyId = this._appStateService.instanceOfProduct?.id;
    this._applicationService.deleteСurrency(currencyId)
    .subscribe(response => {
       this._alertService.success(response.message);
       this.dispose();
       //this._appStateService.refreshListOfPtopsofSubPanel.next({lastItems:"currency"});
     },
     err => {
         this._alertService.error(err.error.message);
    });
  }

  deleteWareHouseInstance(){
    const wareHouseId = this._appStateService.instanceOfProduct?.id;
    this._applicationService.deleteWareHouse(wareHouseId)
    .subscribe(response => {
       this._alertService.success(response.message);
       this.dispose();
       this._appStateService.refreshListOfPtopsofSubPanel.next({lastItems:"warehouse"});
     },
     err => {
         this._alertService.error(err.error.message);
    });
  }

  deleteCartProduct(){
    const idCartProduct = this._appStateService.instanceOfProduct?.productId;
    this._applicationService.deleteCartProduct(idCartProduct)
    .subscribe(response => {
       this._alertService.success(response.message);
       this.dispose();
       this._appStateService.refreshPainGrid.next(true);
       this._appStateService.cartProductCount.next(-1);
     },
     err => {
        
         console.log(err);
         this._alertService.error(err.error.message);
    });
  }

  deleteOrderProduct(){
    const idOrderProduct = this._appStateService.instanceOfProduct?.orderId;
    this._applicationService.deleteOrder(idOrderProduct)
    .subscribe(response => {
       this._alertService.success(response.message);
       this.dispose();
       this._appStateService.refreshOrderGrid.next(true);
     },
     err => {
        
         console.log(err);
         this._alertService.error(err.error.message);
    });
  }

  formProductInstanceToSave(){
    let parm = [];
    let productData = Object.assign({}, this._formEditorService.instanceData);
    if(productData?.parameters){
      for(let item in productData?.parameters){
        parm.push(productData?.parameters[item]);
      }
      productData.parameters = parm;
    } 
    productData.ProductId = this._appStateService.instanceOfProduct?.productId || this._appStateService.instanceOfProduct?.id 
    
    const primeCostBYN = productData["primecost"]?.value;
    const primeCostUSD = productData["primecostusd"]?.value;
    const primeCostEUR = productData["primecosteur"]?.value;
    if(this._appStateService.instanceOfProduct){
      if(this._appStateService.instanceOfProduct?.primeCost !== primeCostBYN){
        if(primeCostBYN){
          this.setPrimeCostUSDandEUR(primeCostBYN, productData);
        }
      }

      if(this._appStateService.instanceOfProduct?.primeCostEUR !== primeCostEUR){
        if(primeCostEUR){
          this.setEURPrimeCost(primeCostEUR, productData);
        }
      }

      if(this._appStateService.instanceOfProduct?.primeCostUSD !== primeCostUSD){
        if(primeCostUSD){
          this.setUSDPrimeCost(primeCostUSD, productData);
        }
      }

    }

    if(productData?.quantity){
       productData.quantity.value = productData?.quantity?.value.toString();
    }
    if(productData?.standartcost){
      productData.standartcost.value = productData?.standartcost?.value.toString();
    }
    if(productData?.primecost){
       productData.primecost.value = productData?.primecost?.value.toString();
    }
    if(productData?.primecostusd){
      productData.primecostusd.value = productData?.primecostusd?.value.toString();
    }
    if(productData?.primecosteur){
      productData.primecosteur.value = productData?.primecosteur?.value.toString();
    }

    return JSON.stringify(productData);
  }

  setPrimeCostUSDandEUR(primeCostBYN, productData){
    if(this._appStateService.Cur_OfficialRate_EUR && this._appStateService.Cur_OfficialRate_USD){
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
 
  }

  setUSDPrimeCost(primeCostUSDinput, productData){
    if(this._appStateService.Cur_OfficialRate_USD){
      const primeCostUSD = {
        name: "primecostusd",
        navPriority: 1,
        type: "text",
        value: primeCostUSDinput
      }

      productData["primecostusd"] = primeCostUSD ;
     
      const primeCostBYN = {
        name: "primecost",
        navPriority: 1,
        type: "text",
        value: (primeCostUSDinput * this._appStateService.Cur_OfficialRate_USD).toFixed(2) + ""
      }

      productData["primecost"] = primeCostBYN;

      const primeCostEUR = {
        name: "primecosteur",
        navPriority: 1,
        type: "text",
        value: (primeCostUSDinput * this._appStateService.Cur_OfficialRate_USD / this._appStateService.Cur_OfficialRate_EUR).toFixed(2) + ""
      }
      productData["primecosteur"] = primeCostEUR;
    }
 
  }

  setEURPrimeCost(primeCostEURinput, productData){
    if(this._appStateService.Cur_OfficialRate_EUR){
      const primeCostEUR = {
        name: "primecosteur",
        navPriority: 1,
        type: "text",
        value: primeCostEURinput
      }

      productData["primecosteur"] = primeCostEUR;
     
      const primeCostBYN = {
        name: "primecost",
        navPriority: 1,
        type: "text",
        value: (primeCostEURinput * this._appStateService.Cur_OfficialRate_EUR).toFixed(2) + ""
      }

      productData["primecost"] = primeCostBYN;
      const primeCostUSD = {
        name: "primecostusd",
        navPriority: 1,
        type: "text",
        value: ((primeCostEURinput * this._appStateService.Cur_OfficialRate_EUR) / this._appStateService.Cur_OfficialRate_USD).toFixed(2) + ""
      }

      productData["primecostusd"] = primeCostUSD;
      
    }
 
  }

  loadCurrencyOnDate(date, curName){
    
    this._currenciesService.loadCurrencyInfoOnDate(date, "USD");

    return this._currenciesService.getCurrencyInfoByAbbreviation(this._appStateService.currencyNBRB, curName);
  }

  formOrderProduct(){
   let orderData = Object.assign({}, this._formEditorService.instanceData);
   orderData.ProductId = this._appStateService.instanceOfProduct?.productId || this._appStateService.instanceOfProduct?.id; 
    
   if(orderData?.quantity){
      orderData.quantity.value = orderData?.quantity?.value.toString();
   }
   if(orderData?.standartcost){
    orderData.standartcost.value = orderData?.standartcost?.value.toString();
   }
   if(orderData?.totalprice){
    orderData.totalprice.value = orderData?.totalprice?.value.toString();
   }
   if(orderData?.quantityorder){
    orderData.quantityorder.value = orderData?.quantityorder?.value.toString();
   }
   if(orderData?.markup){
    orderData.markup.value = orderData?.markup?.value.toString();
   }

    return JSON.stringify(orderData);
  }

  formCompanyInstanceToSave(){
    let companyData = Object.assign({}, this._formEditorService.instanceData);
    companyData.CustomerId = this._appStateService.instanceOfProduct?.id 
    
    return JSON.stringify(companyData);
  }

  forWareHouseInstanceToSave(){
    let warehouseData = Object.assign({}, this._formEditorService.instanceData);
    warehouseData.WareHouseId = this._appStateService.instanceOfProduct?.id 
    
    return JSON.stringify(warehouseData);
  }

  forCurrencyInstanceToSave(){
    let currencyData = Object.assign({}, this._formEditorService.instanceData);
    currencyData.CurrencyId = this._appStateService.instanceOfProduct?.id 
    
    return JSON.stringify(currencyData);
  }

  dispose(){
    this._formEditorService.customProperties = new Subject<any>();
    this._formEditorService.dependentProperties = new Subject<any>();
    this._formEditorService.valueUpdated = new BehaviorSubject<valueUpdatedData>(undefined);
    this._formEditorService.instanceData = {};
    this._formEditorService.listParameters = {};
  }

  //   if(cmd.data){            
  //     if(cmd.data.hasOwnProperty('name')) 
  //         newName = cmd.data['name'];
  //     if(cmd.data.hasOwnProperty('message'))
  //         respMessage = cmd.data['message'];
  // }

  // if (cmd.save) {
  //     if( root.__xml ){
  //         this._alert.warningModal( "Please switch to the Controls view and save again", "Save" );
  //         return;
  //     }
  //     if(root.__nameWasReverted){
  //         delete root.__nameWasReverted;
  //         return;
  //     }
  //     this.saveRootElement(root, false);
  //     element.componentName = element.name;
  // }
  
  
}
