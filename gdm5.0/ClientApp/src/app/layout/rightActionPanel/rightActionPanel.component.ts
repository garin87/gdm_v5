import { ChangeDetectorRef, Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatSidenav } from '@angular/material/sidenav';
import { BehaviorSubject, Observable, of, Subject } from 'rxjs';
import { debounceTime, map, startWith, tap } from 'rxjs/operators';
import { AlertService } from 'src/app/alert/alert.service';
import { ExecuteCommand, IExecuteCommand, valueUpdatedData } from 'src/app/common/objects/common';
import { ApplicationService } from 'src/app/common/services/application.service';
import { AppStateService,  } from 'src/app/common/services/appState.service';
import { FormEditorService } from 'src/app/common/services/formEditor.service';
import { MetadataService } from 'src/app/common/services/metadata.service';
import { StringLiteralLike } from 'typescript';
import { ActionDialogComonent } from '../actionDialog/actionDialog.component';
import { ActionManager } from './actionManager';
import { CurrenciesService } from 'src/app/common/services/currencies.service';

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


  constructor(private _appStateService:AppStateService,
    private _formEditorService:FormEditorService,
    private _applicationService : ApplicationService,
    private _alertService: AlertService, 
    private _currenciesService: CurrenciesService,
    public dialog: MatDialog) { 
   // setTimeout(()=>{this.drawer.toggle()}, 500)
   this._actionManager = new ActionManager(this._appStateService, 
    this._formEditorService, this._applicationService, this._alertService, this._currenciesService);
  };

  ngOnInit(){

    this._appStateService.selectedRowGrid.subscribe(data => {
      if(!data) return;
      this.disposeFormProperties();
      this.executeCommands = this.productActions;
      if(this._appStateService.optionValue == "OrderProductGrid"){
         this.executeCommands = this.cartProductActions;
      }
      if(this._appStateService.optionValue == "OrderGrid"){
        this.executeCommands = this.orderActions;
      }
      if(this._appStateService.initRightActionPanel && !this.drawer.opened){
          this.drawer.open();
      }
    });
    this._appStateService.selectedSideSubPanelValue.subscribe(data=>{
      if(!data) return;
      this.disposeFormProperties();
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
    console.log(command);

    const dialogRef = this.dialog.open(ActionDialogComonent, {
      width: '900px',
      data: {commandName: command.command, 
             typeInstance: command.data?.typeInstance,
             actionName: command.data?.actionName, 
             actiontitle: command.data?.actiontitle, 
             actionButton: command.data?.actionButton,
             parentType:  command.data?.parentType },
    });

    dialogRef.afterClosed().subscribe((result:any) => {
      console.log("---------------------- ActionDialogComonent close");
      console.log(command.data.typeInstance);
      console.log(result);
      if(result){
        const excommand = new ExecuteCommand(command.command,"","", 
        {data: this._formEditorService.instanceData, typeInstance: command.data.typeInstance,
           actionName:"",parentType: command.data?.parentType});

        this._actionManager.executeCommand(excommand); 
      }

      this._formEditorService.customProperties = new Subject<any>();
      this._formEditorService.dependentProperties = new Subject<any>();
      this._formEditorService.valueUpdated = new BehaviorSubject<valueUpdatedData>(undefined);
      this._formEditorService.instanceData = {};
      this._formEditorService.listParameters = {};
    });

  }
  
  onDestroy(){
    this.dispose();
  }

  dispose(){
    this._appStateService.initRightActionPanel = false;
    this._appStateService.instanceOfProduct = undefined;
    this.executeCommands = undefined;
  } 

  disposeFormProperties(){
    this._formEditorService.customProperties = new Subject<any>();
    this._formEditorService.dependentProperties = new Subject<any>();
    this._formEditorService.valueUpdated = new BehaviorSubject<valueUpdatedData>(undefined);
    this._formEditorService.instanceData = {};
    this._formEditorService.listParameters = {};
  }

  private cartProductActions: any = [
    new ExecuteCommand("Delete","delete","", 
    {typeInstance:'selectedCartProduct', actionName: "Delete Product", actiontitle:"Delete product from cart", 
    actionButton:"Delete Product", parentType:"cartProduct"})
  ];
  private orderActions: any = [
    new ExecuteCommand("Delete","delete","", 
    {typeInstance:'selectedOrder', actionName: "Delete order", actiontitle:"Delete order", 
    actionButton:"Delete Order", parentType:"orderProduct"})
  ];
  private companyActions: any = [
    new ExecuteCommand("Edit","edit","", 
    {typeInstance:'selectedcompany', actionName: "Edit", actiontitle:"Update company instance", 
    actionButton:"Update", parentType:"company"}),
    new ExecuteCommand("Delete","delete","", {actionName: "Delete", actiontitle:"Delete the company", typeInstance:'',
    actionButton:"Delete", parentType:"company"}),
  ];

  private wareHouseActions: any = [
    new ExecuteCommand("Edit","edit","", 
    {typeInstance:'selectedWarehouse', actionName: "Edit", actiontitle:"Update warehouse instance", 
    actionButton:"Update", parentType:"warehouse"}),
    new ExecuteCommand("Delete","delete","", {actionName: "Delete", actiontitle:"Delete the warehouse", typeInstance:'',
    actionButton:"Delete", parentType:"warehouse"}),
  ];

  private currencyActions: any = [
    new ExecuteCommand("Edit","edit","", 
      {typeInstance:'currency', actionName: "Edit", actiontitle:"Update currency instance", 
      actionButton:"Update", parentType:"currency"}),
    new ExecuteCommand("Delete","delete","", {actionName: "Delete", actiontitle:"Delete the currency", typeInstance:'',
      actionButton:"Delete", parentType:"currency"}),
  ];


  private productActions: any = [
    new ExecuteCommand("Order","shopping_cart","",
      {typeInstance:'ProductOrder', actionName: "Order", actiontitle:"Order product",
      actionButton:"Quick Order", parentType:"product"}),
    new ExecuteCommand("AddToCart","add_shopping_cart","",
      {typeInstance:'ProductOrder', actionName: "AddToCart", actiontitle:"Add to Cart",
      actionButton:"Add to Cart", parentType:"product"}),
    new ExecuteCommand("Edit","edit","", {typeInstance:'UpdateInstanceProduct', actionName: "Edit", 
      actiontitle:"Update company instance", actionButton:"Update", parentType:"product"}),
    new ExecuteCommand("Delete","delete","", {actionName: "Delete", typeInstance:"", actiontitle:"Delete the company", 
      actionButton:"Delete", parentType:"product"})
  ];

}
