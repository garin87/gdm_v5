import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { NavigationEnd, Router } from '@angular/router';
import { BehaviorSubject, of, Subject } from 'rxjs';

import { AlertService } from '../alert/alert.service';
import { IGetOrderInstancesRequest, OrdersRequest } from '../common/objects/common';
import { ApplicationService } from '../common/services/application.service';
import { AppStateService } from '../common/services/appState.service';
import { FormEditorService } from '../common/services/formEditor.service';

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
      public namesParoduct: any = undefined;
      public isGridOrderShow:boolean = false;
      public isOrderCart:boolean = false;
      public gridOrderData = undefined;
      public gridMetadaType = "OrderGrid";
      public isLoadingResults = false;
      public currentModelName : string;
      public currentModelInstanceName : string;
      public selectedProductName: any = "Order";
      constructor(private _alertService : AlertService,
                  private _applicationService : ApplicationService, 
                  private _formEditorService : FormEditorService,
                  private alertService:AlertService,
                  private _appStateService:AppStateService,
                  private router: Router){
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
        this._appStateService.resetOrderTable.subscribe( data=> {
          if(data){
            this.isGridOrderShow = false;
            setTimeout(() => {
               this.isGridOrderShow = true;
               this.getOrdersData();
            }, 0);
          }   
        });
        this._appStateService.refreshPainGrid.subscribe(refresh=>{
          if(refresh)
             this.getOrderFromCart();
        });
        this._appStateService.refreshOrderGrid.subscribe(refresh=>{
          if(refresh)
             this.getOrdersData();
        });

        
        this.router.events.subscribe((event) => {
          console.log("--------------------------------   this.router.events");
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
        this._applicationService.getOrderProductList(getOrdersRequest)
            .subscribe(response => {
              console.log("------------------------ getOrders --------- getOrdersRequest");
              console.log(response);
              this.isLoadingResults = false;
              this.gridOrderData = response;
              //this.isGridShow = true;
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
      
      getOrderFromCart(nameCompany = "order"){
        const getOrdersRequest : IGetOrderInstancesRequest = new OrdersRequest();
        
        getOrdersRequest.Name = nameCompany;

        this._applicationService.getCartOrderProducts()
        .subscribe(response => {
          console.log("------------------------ getOrders --------- getOrdersRequest");
          console.log(response);
          this.isLoadingResults = false;
          this.gridOrderData = response?.data[0];
          //this.isGridShow = true;
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
        this._appStateService.initRightActionPanel = false;
        if(namePanel == "orderHistory"){
          this.isOrderCart = false;
          this.isGridOrderShow = true;
          this._appStateService.initRightActionPanel = false;
          this.getOrdersData();
        }
        if(namePanel == "orderCart"){
          this.isGridOrderShow = false;
          this.isOrderCart = true; 
          this.getOrderFromCart();
        }

        this._formEditorService.resetValueProperties();
        this.dispose();
      }



      saveOrder(){
        this._applicationService.saveOrderCart()
        .subscribe(response => {
          console.log("------------------------ getOrders --------- getOrdersRequest");
          console.log(response);
          this.isLoadingResults = false;
          this.gridOrderData = false;
          this.alertService.success(response);
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
      onDestroy(){
        this.dispose();
      }
    
      dispose(){
        this._appStateService.initRightActionPanel = false;
        this._appStateService.instanceOfProduct = undefined;
      }
}
