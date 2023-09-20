import { animate, state, style, transition, trigger } from '@angular/animations';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort';
import { MatTable } from '@angular/material/table';
import { Observable, Subscription } from 'rxjs';
import { gridParameter, IGetOrderInstancesRequest, 
  IGridColumnDefinition, IPaginationAction, ISortOption, OrdersRequest, PageFilter } from 'src/app/common/objects/common';
import { ApplicationService } from 'src/app/common/services/application.service';
import { AppStateService } from 'src/app/common/services/appState.service';
import { MetadataService } from 'src/app/common/services/metadata.service';


@Component({
  selector: 'gridExpandRow-control',
  templateUrl: './gridExpandRow.component.html',
  styleUrls: ['./gridExpandRow.component.css'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({height: '0px', minHeight: '0'})),
      state('expanded', style({height: '*'})),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ],
})

export class gridExpandRowComponent implements OnInit{

  @Input("gridData") gridData: any;
  @Input("gridMetadataType") gridMetadataType: any;
  @Input("selectedProduct") selectedProductName: any;
  @Input("typeFilter") typeFilter: any;
  
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild("table") table!: MatTable<any>;
  
  contentTableColumns = ['productName', 'productNumber', 'quantity', 'totalPrice', 'manufacturer'];
  gridMetadata:any;
  gridcolumns:any;
  displayedColumns:any;
  displayedColumns2:any;
  dataSource:any;
  totalRecords:number = 0;
  pageSizeDefault:number = 10;
  currentPageSize:number = this.pageSizeDefault;
  isLoadingResults:boolean = false;
  paginationParameters:any;
  sortOptionsPublick:ISortOption;
  pageFilterPublick:PageFilter;
  GetOrdersRequest:IGetOrderInstancesRequest;;
  selectedRow:any;
  private changedGridOptionSubscription$:Subscription;
  private refreshGridDataSubscription$:Subscription;
  private loadingGridResultsSubscription$:Subscription;
  @ViewChild(MatSort) sort: MatSort;
  
  listProps : Observable<string[]>;
  constructor(private _metadataService:MetadataService,
              private _appStateService:AppStateService,
              private _applicationService : ApplicationService) { };

  announceSortChange(sortState: Sort) {
      console.log(sortState);
      console.log(this.paginator);
      //this.paginator._pageSize
      if(!sortState.direction){
        this.sortOptionsPublick = null;
        return;
      } 
      let paramerts = this.dataSource[0]?.parameters;
      let isParameter = paramerts.some(element => element.name == sortState.active);
      let sortOptions:ISortOption = {
        isParameter: isParameter,
        name: sortState.active,
        direction: sortState.direction
      }
      console.log(sortOptions);
      this.isLoadingResults = true;
      this.sortOptionsPublick = sortOptions;
      this.getGridData(this.selectedProductName,0,this.paginator.pageSize, sortOptions);
  }

  ngOnInit(){
      this.isLoadingResults = true;
      this.gridMetadata = this._metadataService.getMetadataType(this.gridMetadataType);
      this.gridcolumns = this.createColumns(this.gridMetadata);
      this.dataSource = this.gridData?.data || [];

     // this.gridcolumns = this.gridcolumns.concat(this.convertParametersToColumn(this.dataSource));
    //  this.gridcolumns = CommonUtil.sortProperties(this.gridcolumns);
      // this.gridcolumns = [...this.gridcolumns, {
      //   columnDef : 'isExpanded',
      //   header: '', 
      //   cell: ()=> '',
      // }]
      this.displayedColumns = this.gridcolumns.map(c => c.columnDef);
      this.totalRecords = this.gridData?.totalRecords;
      this.sortOptionsPublick = null;
      this.pageFilterPublick = new PageFilter();
      this.GetOrdersRequest = new OrdersRequest();

      this.changedGridOptionSubscription$ = this._appStateService.changedGridOption.subscribe(item =>{
        console.log("--------- -------- ---------changedGridOption.subscribe");
        this.dispose();
        if(item){
          this.getGridData(this.selectedProductName, this.pageFilterPublick.pageNumber = 1,
             this.pageFilterPublick.pageSize = 20, this.sortOptionsPublick, item);
        }
      });

      this.refreshGridDataSubscription$ = this._appStateService.refreshGridData.subscribe(data => {
        if(data){
             this.refreshGridData();
        }
      })
  };
 
  ngOnChanges(changes): void {
    console.log("--------------------- -----console.log(changes);");
    console.log(changes);
    if(changes['gridData']) {
      if( this.gridData?.data || this.gridData?.currentValue){
        this.sortOptionsPublick = null;
        this.dataSource = this.gridData?.data || this.gridData?.currentValue;
        this.dataSource.forEach(item=>{
           item.isExpanded = false;
        })
        this.totalRecords = this.gridData?.totalRecords || this.gridData?.totalRecords;
      }
      if( this.table )this.table.renderRows();
    }
  }

  handlePage($event){
   console.log("---------- handlePage($event)");
   console.log($event);
   let actionName = this.getPaginationEventName($event);
   //  this.paginationParameters = this.getPaginationEventName($event);
   console.log(actionName);
   // this._appStateService.changedPageGrid.next(actionName);
   this.pageFilterPublick.pageNumber = actionName.pageNumber;
   this.pageFilterPublick.pageSize = actionName.pageSize;
   this.isLoadingResults = true;
   this.getGridData(this.selectedProductName, actionName.pageNumber,
                    actionName.pageSize, this.sortOptionsPublick);
  }

  getGridData(nameProduct, pageNumber, pageSize, sortOptions:ISortOption = null, filter:gridParameter = null){
   
    //this.getProductTypeInstancesRequest.NameProductType = nameProduct;
    this.GetOrdersRequest.SortOption = {
      name: sortOptions?.name.trim(),
      direction: sortOptions?.direction.trim(),
      isParameter: sortOptions?.isParameter
    };

    this.GetOrdersRequest.PageFilter = {
      pageNumber : pageNumber,
      pageSize: pageSize
    }


    
    if(filter){
      this.GetOrdersRequest.Filter[filter?.name] = filter.value;
    }
        
    
    this._applicationService.getOrderProductList(this.GetOrdersRequest).subscribe(response => {
      console.log("--------- getGridData ------------ getProductTypeIntances");
      console.log(response);
      this.isLoadingResults = false;
      this.dataSource = response.data;
      this.totalRecords = this.gridData?.totalRecords;
      if(this.table){
        this.table.renderRows();
      }
      
    },
    err => {
        this.isLoadingResults = false;
        console.log(err);
    });
  }

  refreshGridData(){
    this.getGridData(this.selectedProductName, this.pageFilterPublick.pageNumber = 1,
                     this.pageFilterPublick.pageSize = 20, this.sortOptionsPublick);
  }

  convertParametersToColumn(gridData){
    if(!gridData) return;
   // let parametersData = gridData[0]?.parameters;
   // let params = parameters.find(item => item.name == column.columnDef)?.value;
    let unicParameters = [];
    gridData.forEach(orderData => {
      orderData.products.forEach(product => {
        product.parameters.forEach(param=>{
            let tempP = unicParameters.find(item => item?.name == param.name)?.value;
            if(!tempP){
                unicParameters.push(param);
            }
        });
      });
    })
    
    //  if(!parametersData) return;
    //  return  parametersData.map(element => {
    //     return {
    //       columnDef : element.name,
    //       header: element.name,
    //       isSortable: true,
    //       cell: (row, column, i) => `${this.getCellValue(row, column, i)}`,
    //       order: element?.order || element?.priority  
    //     }
    //  });
  }
 
  getCellValue(row, column, i){
    let startPageNumber;

    if(this.paginator){
      startPageNumber = (this.paginator.pageSize * this.paginator.pageIndex) + i + 1;
    }else startPageNumber = i + 1;

    let valueColumn = column.columnDef == "position" ? startPageNumber : row[column.columnDef];

    if(!valueColumn && valueColumn != 0){
      if(row?.parameters){
        valueColumn = row?.parameters.find(item => item.name == column.columnDef)?.value;
      }
    }
    
    return typeof valueColumn == "undefined" ? null : valueColumn;
  }

  createColumns(gridMetadata){
    let gridColumns: IGridColumnDefinition[] = [];
    gridMetadata.forEach(element => {
      let col :IGridColumnDefinition = {
        columnDef : element.name,
        header: element.displayedName,
        isSortable: element.isSortable,
        cell: (row, column, i) => `${this.getCellValue(row, column, i)}`,
        order: element?.order || element?.priority
      }
      gridColumns.push(col);
    });

    gridMetadata.map(element => {
      return {
        columnDef : element.name,
        header: element.displayedName,
        isSortable: element.isSortable,
        cell: (item) => `${item}`, 
        order: element?.order || element?.priority
      }
    });
    return gridColumns;
  }

  selectRow(row){
     console.log("-------- grid ------ selectRow");
     console.log(row);
     this.selectedRow = row;
     row.isExpanded = !row.isExpanded;
     this._appStateService.initRightActionPanel = false;
     if(this.gridMetadataType == "OrderGrid"){
       this._appStateService.optionValue = "OrderGrid";
       this._appStateService.initRightActionPanel = true;
       this._appStateService.instanceOfProduct = row;
       this._appStateService.selectedRowGrid.next(row);
     }
    
  }

  private getPaginationEventName(pEvent):IPaginationAction{
    let actionName: string;
    let paginationData: IPaginationAction = {};

    if(pEvent.pageSize != this.currentPageSize){ 
      this.currentPageSize = pEvent.pageSize;
      actionName = "changedPageSize";
    }else if(!this.paginator.hasNextPage()){
      actionName = "lastPage";
    }else if(!this.paginator.hasPreviousPage()){
      actionName = "firstPage";
    }else if(pEvent.previousPageIndex > pEvent.pageIndex){
      actionName = "previousPage";
    }else{
      actionName = "nextPage";
    }

    paginationData.paginationEventName = actionName; 
    paginationData.gridName = this.gridMetadataType;
    paginationData.pageSize = pEvent.pageSize
    paginationData.pageNumber = (pEvent.pageSize * pEvent.pageIndex) + 1;
  
    return paginationData;
  }

  ngOnDestroy(){
    this.dispose();
    this.changedGridOptionSubscription$.unsubscribe();
    this.refreshGridDataSubscription$.unsubscribe();
  }

  dispose(){
    this.selectedRow = null;
    this._appStateService.initRightActionPanel = false;
    this._appStateService.instanceOfProduct = undefined;
  }
}

