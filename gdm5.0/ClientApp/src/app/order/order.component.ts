import { Component } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { AlertService } from '../alert/alert.service';
import { IGetOrderInstancesRequest, OrdersRequest } from '../common/objects/common';
import { ApplicationService } from '../common/services/application.service';
import { AppStateService } from '../common/services/appState.service';
import { FormEditorService } from '../common/services/formEditor.service';
import { LabelsService } from '../common/services/labels.service';

@Component({
  selector: 'app-order-component',
  styleUrls: ['./order.component.css'],
  templateUrl: './order.component.html'
  
})

export class OrderComponent {
      public isCreateModel: boolean = false;
      public isCompanyPage: boolean = false;

      // public instanceName:string = 'Product';
      public sidenavToggle;
      public namesParoduct: any;
      public isGridOrderShow:boolean = false;
      public isGridOrderReportShow:boolean = false;
      public isOrderCart:boolean = false;
      public isOrderReport:boolean = false;
      public gridOrderData;
      public gridOrderReportData;
      public gridMetadaType = "OrderGrid";
      public isLoadingResults = false;
      public currentModelName : string;
      public currentModelInstanceName : string;
      public selectedProductName: any = "Order";
      private routerEvents$:Subscription;

      private resetOrderTableValueSubscription$:Subscription;
      private refreshPainGridSubscription$:Subscription;
      private refreshOrderGridSubPanelSubscription$:Subscription;
  refreshGridPainData: any;

      constructor(private _alertService : AlertService,
                  private _applicationService : ApplicationService, 
                  private _formEditorService : FormEditorService,
                  private alertService:AlertService,
                  private _appStateService:AppStateService,
                  private router: Router,
                  public _labelsService: LabelsService){
       //   this._appStateService.detectClickOnPanel.next(true);
            this.isCreateModel = false;
      }
                         
      ngOnInit(){
        
        this._appStateService.initRightActionPanel = false;
        this.isLoadingResults = true;
        this.isOrderCart = true;
        this.isGridOrderShow = false;
        this._appStateService.selectedSideSubPanelValue.next(undefined);
        if(this.isGridOrderShow){
          this.getOrdersData();
        }
        this.getOrderFromCart();
        this.resetOrderTableValueSubscription$ = this._appStateService.resetOrderTable.subscribe( data=> {
          if(data){
            this.isGridOrderShow = false;
            setTimeout(() => {
               this.isGridOrderShow = true;
               this.getOrdersData();
            }, 0);
          }   
        });

        this.refreshPainGridSubscription$ = this._appStateService.refreshPainGrid.subscribe(refresh=>{
          if(refresh)
             this.getOrderFromCart();
        });

        this.refreshOrderGridSubPanelSubscription$ = this._appStateService.refreshOrderGrid.subscribe(refresh=>{
          if(refresh)
             this.getOrdersData();
        });

        
        this.routerEvents$ =   this.router.events.subscribe((event) => {
          if (event instanceof NavigationEnd) {
              this._appStateService.initRightActionPanel = false;
          }
        });
      }
      
      getOrdersData(nameCompany = "order", pageNumber = 0, pageSize = 20){
        const getOrdersRequest : IGetOrderInstancesRequest = new OrdersRequest();
        getOrdersRequest.PageFilter = {
          pageNumber: pageNumber,
          pageSize: pageSize
        };
        
        getOrdersRequest.Name = nameCompany;
        this.isLoadingResults = true;
        this._applicationService.getOrderProductList(getOrdersRequest)
            .subscribe(response => {
              console.log("------------------------ getOrders --------- getOrdersRequest");
              console.log(response);
              this.isLoadingResults = false;
              this.gridOrderData = response;
              //this.isGridShow = true;
              // setTimeout(() => {             
              //   this.activePanel("isGridShow");
              // }, 0);         
            },
            err => {
                this.isLoadingResults = false;
                this.alertService.error(err.error.message);

                console.log("----  error getProductTypeIntances");
                console.log(err);
            });
      }
      
      getOrderFromCart(nameCompany = "order"){
        const getOrdersRequest : IGetOrderInstancesRequest = new OrdersRequest();
        
        getOrdersRequest.Name = nameCompany;
        this.isLoadingResults = true;
        this._applicationService.getCartOrderProducts().subscribe(response => {
          this.isLoadingResults = false;
          this.gridOrderData = response?.data[0];
          //this.isGridShow = true;
          // if(this.refreshGridPainData){
          //   this.refreshGridPainData.next(true);
          // }
          
          // setTimeout(() => {             
          //    this.activePanel("isGridShow");
          // }, 0);         
        },

        err => {
            this.isLoadingResults = false;
            this.alertService.error(err?.error?.message || err );
            console.log(err);
        });
      }
  
      activePanel(namePanel){
        this._appStateService.initRightActionPanel = false;
        if(namePanel == "orderHistory"){
          this.isOrderCart = false;
          this.isOrderReport = false;
          this.isGridOrderShow = true;
          this._appStateService.initRightActionPanel = false;
          this.getOrdersData();
        }
        if(namePanel == "orderCart"){
          this.isGridOrderShow = false;
          this.isOrderReport = false;
          this.isOrderCart = true; 
          this.getOrderFromCart();
        }
        if(namePanel == "orderReport"){
          this.isGridOrderShow = false;
          this.isOrderCart = false; 
          this.isOrderReport = true;
        }



        
        this._formEditorService.resetValueProperties();
        this.dispose();
      }

      saveOrder(){
        this.isLoadingResults = true;
        this._applicationService.saveOrderCart()
        .subscribe(response => {
          this.isLoadingResults = false;
          this.gridOrderData = false;
          this.alertService.success(response?.message);
          this._appStateService.cartProductCount.next(0);
        },
        err => {
            this.isLoadingResults = false;
            this.alertService.error(err.error.message);

            console.log("----  error getProductTypeIntances");
            console.log(err);
        });
      }

      cancelCartOrder(){
        this.isLoadingResults = true;
        this._applicationService.cancelOrderCart()
        .subscribe(response => {
          console.log("------------------------ cancelOrderCart");
          console.log(response);
          this.isLoadingResults = false;
          this.gridOrderData = false;
          this.alertService.success(response.message);
          this._appStateService.cartProductCount.next(0);
        },
        err => {
          this.isLoadingResults = false;
          this.alertService.error(err.error.message);
          console.log("----  error getProductTypeIntances");
          console.log(err);
        });
      }


      loadReport(){
        const id = "ReportProduct" + "controls-id";
        if(!this._formEditorService.isRequiredValue(id)){
          this._alertService.warning("Please fill out all required fields");
          return;
        } 

        this.isLoadingResults = true;
        const reportFilterRequest = this.formFilterForReport();
        this._applicationService.loadOrederReport(reportFilterRequest)
          .subscribe(response => {
            this.isLoadingResults = false;
            this.gridOrderData = false;
            this.gridOrderReportData = response;
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
        this.routerEvents$.unsubscribe();
        this.resetOrderTableValueSubscription$.unsubscribe();
        this.refreshPainGridSubscription$.unsubscribe();
        this.refreshOrderGridSubPanelSubscription$.unsubscribe();
      }
    
      dispose(){
        this._appStateService.initRightActionPanel = false;
        this._appStateService.instanceOfProduct = undefined;
      }
}
