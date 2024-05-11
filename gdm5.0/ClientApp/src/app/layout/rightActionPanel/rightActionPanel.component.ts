import { Component, OnInit, ViewChild } from '@angular/core';
import { MatSidenav } from '@angular/material/sidenav';
import { BehaviorSubject, Subject, Subscription } from 'rxjs';
import { AlertService } from 'src/app/alert/alert.service';
import { ExecuteCommand, IExecuteCommand, valueUpdatedData } from 'src/app/common/objects/common';
import { ApplicationService } from 'src/app/common/services/application.service';
import { AppStateService,  } from 'src/app/common/services/appState.service';
import { FormEditorService } from 'src/app/common/services/formEditor.service';
import { ActionDialogComonent } from '../actionDialog/actionDialog.component';
import { ActionManager } from './actionManager';
import { CurrenciesService } from 'src/app/common/services/currencies.service';
import { LabelsService } from 'src/app/common/services/labels.service';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'rightActionPanel',
  templateUrl: './rightActionPanel.component.html',
  styleUrls: ['./rightActionPanel.component.css']
})

export class rightActionPanelComponent implements OnInit{

 // @Input("eventToggle") eventToggle: any;
 // @Input("commands") commands: any;
  @ViewChild("drawer", { static: true }) drawer : MatSidenav;

  public sidePanel: MatSidenav;
  public executeCommands : IExecuteCommand[];
  private _actionManager: ActionManager;
  get actionManagerService(): ActionManager { return this._actionManager; }
  private selectedPage: string = "";
  private selectedRowGridSubscription$:Subscription;
  private selectedSideSubPanelValueSubscription$:Subscription;

  constructor(private _appStateService:AppStateService,
    private _formEditorService:FormEditorService,
    private _applicationService : ApplicationService,
    private _alertService: AlertService, 
    private _currenciesService: CurrenciesService,
    public dialog: MatDialog,
    public _labelsService: LabelsService) { 
    // setTimeout(()=>{this.drawer.toggle()}, 500)
    this._actionManager = new ActionManager(this._appStateService, 
    this._formEditorService, this._applicationService, this._alertService, this._currenciesService);
  };

  ngOnInit(){

   this.selectedRowGridSubscription$ = this._appStateService.selectedRowGrid.subscribe(data => {
      if(!data) return;
      this.disposeFormProperties();
      this.executeCommands = this.productActions;
      if(this._appStateService.optionValue == "OrderProductGrid"){
         this.executeCommands = this.cartProductActions;
      }
      if(this._appStateService.optionValue == "OrderGrid"){
        this.executeCommands = this.orderActions;
      }
      if(this._appStateService.optionValue == "PriceListValuesGrid"){
        this.executeCommands = this.priceValuesActions;
      }


      
      if(this._appStateService.initRightActionPanel && !this.drawer.opened){
          this.drawer.open();
      }
    });

    this.selectedSideSubPanelValueSubscription$ = this._appStateService.selectedSideSubPanelValue.subscribe(data=>{
      if(!data) return;
    //  this.disposeFormProperties();
      this.selectedPage = data?.page;
      if(this._appStateService?.selectedInstancePanel == "Company") {
        this.executeCommands = this.companyActions; 
      };
      if(this._appStateService?.selectedInstancePanel == "WareHouse") {
        this.executeCommands = this.wareHouseActions;    
      };
      if(this._appStateService?.selectedInstancePanel == "Currency") {
        this.executeCommands = this.currencyActions;    
      };
      if(this._appStateService?.selectedInstancePanel == "PriceListBase") {
        this.executeCommands = this.priceListBaseActions;    
      };
      this.drawer.open();
    });  
  };

  ngOnChanges(change){
    if(change["commands"]){
    }
  };

  sidenavToggle(){
    this.drawer.toggle()
  }

  actionManager(command:ExecuteCommand){
    const dialogRef = this.dialog.open(ActionDialogComonent, {
      width: '930px',
      data: {commandName: command.command, 
             typeInstance: command.data?.typeInstance,
             actionName: command.data?.actionName, 
             actiontitle: command.data?.actiontitle, 
             actionButton: command.data?.actionButton,
             parentType:  command.data?.parentType },
    });

    dialogRef.afterClosed().subscribe((result:any) => {
      if(result){
        const excommand = new ExecuteCommand(command.command,"","","", 
        {data: this._formEditorService.instanceData, typeInstance: command.data.typeInstance,
           actionName:"",parentType: command.data?.parentType});

        this._actionManager.executeCommand(excommand); 
      }

      this._formEditorService.customProperties = new Subject<any>();
    //  this._formEditorService.dependentProperties = new Subject<any>();
      this._formEditorService.valueUpdated = new BehaviorSubject<valueUpdatedData>(undefined);
      this._formEditorService.instanceData = {};
      this._formEditorService.listParameters = {};
    });
    this._labelsService.labels.modelingActionPopUp_DeleteOrder


  }
  
  ngOnDestroy(){
    this.dispose();
    this.selectedRowGridSubscription$.unsubscribe();
    this.selectedSideSubPanelValueSubscription$.unsubscribe();
  }

  dispose(){
    this._appStateService.initRightActionPanel = false;
    this._appStateService.instanceOfProduct = undefined;
    this.executeCommands = undefined;
  } 

  disposeFormProperties(){
    this._formEditorService.customProperties = new Subject<any>();
   // this._formEditorService.dependentProperties = new Subject<any>();
    this._formEditorService.valueUpdated = new BehaviorSubject<valueUpdatedData>(undefined);
    this._formEditorService.instanceData = {};
    this._formEditorService.listParameters = {};
  }

  private cartProductActions: any = [
    new ExecuteCommand("Delete",this._labelsService.labels.productActionMenu_Delete,"delete","", 
    {typeInstance:'selectedCartProduct', actionName: "Delete Product", actiontitle:this._labelsService.labels.modelingActionPopUp_DeleteProductFromCart, 
    actionButton: this._labelsService.labels.productActionPopUpButton_Delete, parentType:"cartProduct"})
  ];

  private orderActions: any = [
    new ExecuteCommand("Delete",this._labelsService.labels.productActionMenu_Delete,"delete","", 
    {typeInstance:'selectedOrder', actionName: "Delete order", actiontitle:this._labelsService.labels.modelingActionPopUp_DeleteOrder, 
    actionButton: this._labelsService.labels.productActionPopUpButton_Delete, parentType:"orderProduct"})
  ];
  private priceListBaseActions: any = [
    new ExecuteCommand("Edit", this._labelsService.labels.productActionMenu_Edit,"edit","", 
      {typeInstance:'pricelistbaseEdit', 
      actionName: "Edit", 
      actiontitle:this._labelsService.labels.modelingActionPopUp_UpdateCompany, 
      actionButton:this._labelsService.labels.productActionPopUpButton_Update, 
      parentType:"pricelistbase"}),
    new ExecuteCommand("Delete",this._labelsService.labels.productActionMenu_Delete,"delete","", 
      {typeInstance:'pricelistbase', 
      actionName: "Delete", 
      actiontitle:this._labelsService.labels.modelingActionPopUp_DeleteOrder, 
      actionButton: this._labelsService.labels.productActionPopUpButton_Delete, 
      parentType:"pricelistbase"})
  ];


  private priceValuesActions: any = [
    new ExecuteCommand("Edit", this._labelsService.labels.productActionMenu_Edit,"edit","", 
      {typeInstance:'PriceListValuesRowSelected', 
      actionName: "Edit", 
      actiontitle:this._labelsService.labels.modelingActionPopUp_UpdateCompany, 
      actionButton:this._labelsService.labels.productActionPopUpButton_Update, 
      parentType:"PriceListValuesGrid"}),
    new ExecuteCommand("Delete",this._labelsService.labels.productActionMenu_Delete,"delete","", 
      {typeInstance:'selectedOrder', 
      actionName: "Delete", 
      actiontitle:this._labelsService.labels.modelingActionPopUp_DeleteOrder, 
      actionButton: this._labelsService.labels.productActionPopUpButton_Delete, 
      parentType:"PriceListValuesGrid"})
  ];


  private companyActions: any = [
    new ExecuteCommand("Edit",this._labelsService.labels.productActionMenu_Edit,"edit","", 
    {typeInstance:'selectedcompany', actionName: "Edit", actiontitle:this._labelsService.labels.modelingActionPopUp_UpdateCompany, 
    actionButton:this._labelsService.labels.productActionPopUpButton_Update, parentType:"company"}),
    new ExecuteCommand("Delete",this._labelsService.labels.productActionMenu_Delete,"delete","", {actionName: "Delete", actiontitle:this._labelsService.labels.modelingActionPopUp_DeleteCompany, typeInstance:'',
    actionButton:this._labelsService.labels.productActionPopUpButton_Delete, parentType:"company"}),
  ];

  private wareHouseActions: any = [
    new ExecuteCommand("Edit",this._labelsService.labels.productActionMenu_Edit,"edit","", 
    {typeInstance:'selectedWarehouse', actionName: "Edit", actiontitle:this._labelsService.labels.modelingActionPopUp_UpdateWarehouse, 
    actionButton:this._labelsService.labels.productActionPopUpButton_Update, parentType:"warehouse"}),
    new ExecuteCommand("Delete",this._labelsService.labels.productActionMenu_Delete,"delete","", {actionName: "Delete", actiontitle:this._labelsService.labels.modelingActionPopUp_DeleteWarehouse, typeInstance:'',
    actionButton:this._labelsService.labels.productActionPopUpButton_Delete, parentType:"warehouse"}),
  ];

  private currencyActions: any = [
    new ExecuteCommand("Edit",this._labelsService.labels.productActionMenu_Edit,"edit","", 
      {typeInstance:'currency', actionName: "Edit", actiontitle:this._labelsService.labels.modelingActionPopUp_UpdateCurrency, 
      actionButton:this._labelsService.labels.productActionPopUpButton_Update, parentType:"currency"}),
    new ExecuteCommand("Delete",this._labelsService.labels.productActionMenu_Delete,"delete","", {actionName: "Delete", actiontitle:this._labelsService.labels.modelingActionPopUp_DeleteCurrency, typeInstance:'',
      actionButton:this._labelsService.labels.productActionPopUpButton_Delete, parentType:"currency"}),
  ];

  
  private productActions: any = [
    new ExecuteCommand("Order",this._labelsService.labels.productActionMenu_Order,"shopping_cart","",
      {typeInstance:'ProductOrder', actionName: "Order", actiontitle: this._labelsService.labels.productActionPopUp_OrderProduct,
      actionButton:this._labelsService.labels.productActionPopUpButton_QuickOrder, parentType:"product"}),
    new ExecuteCommand("AddToCart",this._labelsService.labels.productActionMenu_AddToCart,"add_shopping_cart","",
      {typeInstance:'ProductOrder', actionName: "AddToCart", actiontitle: this._labelsService.labels.productActionPopUp_AddToCart,
      actionButton:this._labelsService.labels.productActionPopUpButton_AddToCart, parentType:"product"}),
    new ExecuteCommand("Edit",this._labelsService.labels.productActionMenu_Edit,"edit","", {typeInstance:'UpdateInstanceProduct', actionName: "Edit", 
      actiontitle:this._labelsService.labels.productActionPopUp_UpdateProduct, actionButton:this._labelsService.labels.productActionPopUpButton_Update, parentType:"product"}),
    new ExecuteCommand("Delete",this._labelsService.labels.productActionMenu_Delete,"delete","", {actionName: "Delete", typeInstance:"", actiontitle:this._labelsService.labels.productActionPopUp_DeleteProduct, 
      actionButton:this._labelsService.labels.productActionPopUpButton_Delete, parentType:"product"})
  ];

}
