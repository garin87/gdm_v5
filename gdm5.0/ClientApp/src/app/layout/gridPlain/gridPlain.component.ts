import { animate, state, style, transition, trigger } from '@angular/animations';
import { ChangeDetectorRef, Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSidenav } from '@angular/material/sidenav';
import { MatSort, Sort } from '@angular/material/sort';
import { MatTable } from '@angular/material/table';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { debounceTime, map, startWith, tap } from 'rxjs/operators';
import { gridParameter, IGetOrderInstancesRequest, IgetProductTypeInstancesRequest, 
  IGridColumnDefinition, IPaginationAction, ISortOption, OrdersRequest, PageFilter, ProductTypeInstancesRequest,} from 'src/app/common/objects/common';
import { ApplicationService } from 'src/app/common/services/application.service';
import { AppStateService } from 'src/app/common/services/appState.service';
import { MetadataService } from 'src/app/common/services/metadata.service';
import { CommonUtil } from 'src/app/common/utils/common-utils';


@Component({
  selector: 'gridPlain-control',
  templateUrl: './gridPlain.component.html',
  styleUrls: ['./gridPlain.component.css']
})
export class gridPlainComponent implements OnInit{

  @Input("gridData") gridData: any;
  @Input("gridMetadataType") gridMetadataType: any;
  @Input("selectedProduct") selectedProductName: any;
  
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild("table") table!: MatTable<any>;
  
  gridMetadata:any;
  gridcolumns:any;
  displayedColumns:any;
  dataSource:any;
  totalRecords:number = 0;
  pageSizeDefault:number = 10;
  currentPageSize:number = this.pageSizeDefault;
  isLoadingResults:boolean = false;
  paginationParameters:any;
  sortOptionsPublick:ISortOption;
  pageFilterPublick:PageFilter;
  GetOrdersRequest:IGetOrderInstancesRequest;
  selectedRow:any;

  
  listProps : Observable<string[]>;

  constructor(private _metadataService:MetadataService,
              private _appStateService:AppStateService,
              private _applicationService : ApplicationService) { };


  ngOnInit(){
      this.gridMetadata = this._metadataService.getMetadataType(this.gridMetadataType);
      this.gridcolumns = this.createColumns(this.gridMetadata);
      this.gridcolumns = this.gridcolumns.filter(item => item != undefined);
      this.dataSource = this.gridData || [];
      this.gridcolumns = this.gridcolumns.concat(this.convertParametersToColumn(this.dataSource));
      this.gridcolumns = this.gridcolumns.filter(item => item != undefined);
      this.displayedColumns = this.gridcolumns.map(c => c.columnDef);
      this.totalRecords = this.gridData?.totalRecords;
      this.sortOptionsPublick = null;
      this.pageFilterPublick = new PageFilter();
      this.GetOrdersRequest = new OrdersRequest();

      // this._appStateService.changedGridOption.subscribe(item =>{
      //   console.log("--------- -------- ---------changedGridOption.subscribe");
      //   this.dispose();
      //   if(item){
      //     this.getGridData(this.selectedProductName, this.pageFilterPublick.pageNumber = 1,
      //        this.pageFilterPublick.pageSize = 10, this.sortOptionsPublick, item);
      //   }
      // });


      // this._appStateService.refreshGridData.subscribe(data => {
      //   if(data){
      //        this.refreshGridData();
      //   }
      // })
  };
 
  ngOnChanges(changes): void {
    console.log("--------------------- -----console.log(changes);");
    console.log(changes);
    if(changes['gridData']) {
      if( this.gridData?.data || this.gridData?.currentValue || changes['gridData']?.currentValue){
        this.dataSource = [];
        this.sortOptionsPublick = null;
        this.dataSource = this.gridData?.data || this.gridData?.currentValue || changes['gridData']?.currentValue;
        this.totalRecords = this.gridData?.totalRecords || this.gridData?.totalRecords;
       
      } 
      if( this.table )this.table.renderRows();
    }
  }

  getGridData(name, pageNumber, pageSize, sortOptions:ISortOption = null, filter:gridParameter = null){
   
    this.GetOrdersRequest.Name = name;
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
      console.log("--------- getGridData ------------ getOrderProductList");
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

  // handlePage($event){
  //  console.log("---------- handlePage($event)");
  //  console.log($event);
  //  let actionName = this.getPaginationEventName($event);
  //  //  this.paginationParameters = this.getPaginationEventName($event);
  //  console.log(actionName);
  //  // this._appStateService.changedPageGrid.next(actionName);
  //  this.pageFilterPublick.pageNumber = actionName.pageNumber;
  //  this.pageFilterPublick.pageSize = actionName.pageSize;
  //  this.isLoadingResults = true;
  //  this.getGridData("Order", actionName.pageNumber,
  //                   actionName.pageSize, this.sortOptionsPublick);
  // }

  // getGridData(name, pageNumber, pageSize, sortOptions:ISortOption = null, filter:gridParameter = null){
   
  //   this.GetOrdersRequest.Name = name;
  //   this.GetOrdersRequest.SortOption = {
  //     name: sortOptions?.name.trim(),
  //     direction: sortOptions?.direction.trim(),
  //     isParameter: sortOptions?.isParameter
  //   };
  //   this.GetOrdersRequest.PageFilter = {
  //     pageNumber : pageNumber,
  //     pageSize: pageSize
  //   }

  //   if(filter){
  //     this.GetOrdersRequest.Filter[filter?.name] = filter.value;
  //   }
        
    
  //   this._applicationService.getOrderProductList(this.GetOrdersRequest).subscribe(response => {
  //     console.log("--------- getGridData ------------ getOrderProductList");
  //     console.log(response);
  //     this.isLoadingResults = false;
  //     this.dataSource = response.data;
  //     this.totalRecords = this.gridData?.totalRecords;
  //     if(this.table){
  //       this.table.renderRows();
  //     }
      
  //   },
  //   err => {
  //       this.isLoadingResults = false;
  //       console.log(err);
  //   });
  // }

  refreshGridData(){
    this.getGridData(this.selectedProductName, this.pageFilterPublick.pageNumber = 1,
                     this.pageFilterPublick.pageSize = 10, this.sortOptionsPublick);
  }

  convertParametersToColumn(gridData){
    if(!gridData) return [];
    let uniqParameterNames: string[] = [];
    let uniqParameters = [];
    gridData.forEach(element => {
        if(element?.parameters){
          element?.parameters.forEach(param => {
              if(param.name && !uniqParameterNames.includes(param.name)){
                 uniqParameterNames.push(param.name);
                 uniqParameters.push(param);
              }  
          });
        };
    });

    if(!uniqParameters) return;
    return  uniqParameters.map(element => {
      return {
        columnDef : element.name,
        header: element.name,
        isSortable: true,
        cell: (row, column, i) => `${this.getCellValue2(row, column, i)}`,
        order: element?.order || element?.priority  
      }
    });
  }
 
  getCellValue(row, column, i){
    let startPageNumber: number;
    if(this.paginator){
      startPageNumber = (this.paginator.pageSize * this.paginator.pageIndex) + i + 1;
    }else startPageNumber = i + 1;

    let valueColumn = column.columnDef == "position" ? startPageNumber : row[column.columnDef];
    //  valueColumn = column.columnDef  == "dateOfReceipt"? valueColumn.split("T")[0] : valueColumn;
    if(!valueColumn && valueColumn != 0){
      if(row?.parameters){
        valueColumn = row?.parameters.find(item => item.name == column.columnDef)?.value;
      }
    }

    return typeof valueColumn == "undefined" ? null : valueColumn;
  }
  
  private getCellValue2(row: any, column: any, i: number): string | number | null {
    let startPageNumber: number;
  
    if (this.paginator) {
      startPageNumber = (this.paginator.pageSize * this.paginator.pageIndex) + i + 1;
    } else {
      startPageNumber = i + 1;
    }
  
    if (column.columnDef === 'position') {
      return startPageNumber;
    }
  
    let valueColumn = row[column.columnDef];
  
    if (valueColumn === undefined || valueColumn === null) {
      if (row?.parameters) {
        const parameter = row.parameters.find((param) => param.name === column.columnDef);
        valueColumn = parameter?.value;
      }
    }
  
    if (valueColumn === undefined || valueColumn === null) {
      return null;
    }
  
    // If the column is a date column, format the value as a string
    if (column.columnDef === 'dateOfReceipt') {
      const date = new Date(valueColumn);
      return date.toISOString().split('T')[0];
    }
  
    return valueColumn;
  }

  createColumns(gridMetadata){
    let gridColumns: IGridColumnDefinition[] = [];
    gridMetadata.forEach(element => {
      let col :IGridColumnDefinition = {
        columnDef : element.name,
        header: element.displayedName,
        isSortable: element.isSortable,
        cell: (row, column, i) => `${this.getCellValue2(row, column, i)}`,
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
     this._appStateService.initRightActionPanel = false;
     if(this.gridMetadataType == "OrderProductGrid"){
       this._appStateService.optionValue = "OrderProductGrid";
       this._appStateService.initRightActionPanel = true;
       this._appStateService.instanceOfProduct = row;
       this._appStateService.selectedRowGrid.next(row);
     }
    
  }

  // private getPaginationEventName(pEvent):IPaginationAction{
  //   let actionName: string;
  //   let paginationData: IPaginationAction = {};

  //   if(pEvent.pageSize != this.currentPageSize){ 
  //     this.currentPageSize = pEvent.pageSize;
  //     actionName = "changedPageSize";
  //   }else if(!this.paginator.hasNextPage()){
  //     actionName = "lastPage";
  //   }else if(!this.paginator.hasPreviousPage()){
  //     actionName = "firstPage";
  //   }else if(pEvent.previousPageIndex > pEvent.pageIndex){
  //     actionName = "previousPage";
  //   }else{
  //     actionName = "nextPage";
  //   }

  //   paginationData.paginationEventName = actionName; 
  //   paginationData.gridName = this.gridMetadataType;
  //   paginationData.pageSize = pEvent.pageSize
  //   paginationData.pageNumber = (pEvent.pageSize * pEvent.pageIndex) + 1;
  
  //   return paginationData;
  // }

  onDestroy(){
    this.dispose();
  }

  dispose(){
    this.selectedRow = null;
    this._appStateService.initRightActionPanel = false;
    this._appStateService.instanceOfProduct = undefined;
    this._appStateService.optionValue = undefined;
    this.dataSource = [];
  }
}

