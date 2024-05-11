
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatTable } from '@angular/material/table';
import { Observable, Subscription } from 'rxjs';
import { gridParameter, IGetOrderInstancesRequest,
  IGridColumnDefinition, IParameter, ISortOption, OrdersRequest, PageFilter } from 'src/app/common/objects/common';
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
  dataTile:any;
  listProps : Observable<string[]>;
  refreshGridDataSubscription$:Subscription;

  constructor(private _metadataService:MetadataService,
              private _appStateService:AppStateService,
              private _applicationService : ApplicationService) { };


  ngOnInit(){
      this.gridMetadata = this._metadataService.getMetadataType(this.gridMetadataType);
      this.gridcolumns = this.createColumns(this.gridMetadata);
      this.gridcolumns = this.gridcolumns.filter(item => item != undefined);
      this.dataSource = this.gridData || [];
      this.gridcolumns = this.gridcolumns.concat(this.convertParametersToColumn(this.dataSource));
      this.gridcolumns = CommonUtil.sortProperties(this.gridcolumns);
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

      // this.refreshGridDataSubscription$ = this._appStateService.refreshGridPainData.subscribe(data => {
      //   if(data){
      //         this.refreshGridData();
      //   }
      // })
  };
 
  ngOnChanges(changes): void {
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
    if(!gridData) return [];
    let uniqParameterNames: string[] = [];
    let uniqParameters = [];
    gridData.forEach(element => {
        if(element?.parameters){
          element?.parameters.forEach(param => {
              if(param.name && !uniqParameterNames.includes(param.name)){
                if(param.name.toLowerCase() !== 'номер'){
                   uniqParameterNames.push(param.name);
                   uniqParameters.push(param);
                }
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

  

    return typeof valueColumn == "undefined" ? "" : valueColumn;
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
    
    if(column.columnDef  == "price"){
      if(valueColumn == 0){
         const priceEUR = row["priceEUR"];
         const CurEUR = this._appStateService.Cur_OfficialRate_EUR;
         valueColumn = (priceEUR * CurEUR).toFixed(2);
      }
    }

    if(column.columnDef  == "priceNDS"){
      if(valueColumn == 0){
         const priceEURNDS = row["priceEURNDS"];
         const CurEUR = this._appStateService.Cur_OfficialRate_EUR;
         valueColumn = (priceEURNDS * CurEUR).toFixed(2);
      }
    }

    if (valueColumn === undefined || valueColumn === null) {
      if (row?.parameters) {
        const parameter = row.parameters.find((param) => param.name === column.columnDef);
        valueColumn = parameter?.value;
      }
    }
  
    if (valueColumn === undefined || valueColumn === null) {
      return null;
    }
  
    if (column.columnDef === 'dateOfReceipt') {
      const date = new Date(valueColumn);
      return date.toISOString().split('T')[0];
    }

    if(valueColumn === null){
       valueColumn = ""
    }
    
    return typeof valueColumn == "undefined" ? "" : valueColumn;
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
     this.selectedRow = row;
     this._appStateService.initRightActionPanel = false;
     if(this.gridMetadataType == "OrderProductGrid"){
       this._appStateService.optionValue = "OrderProductGrid";
       this._appStateService.initRightActionPanel = true;
       this._appStateService.instanceOfProduct = row;
       this._appStateService.selectedRowGrid.next(row);
     }
     if(this.gridMetadataType == "PriceListValuesGrid"){
      this._appStateService.optionValue = "PriceListValuesGrid";
      this._appStateService.initRightActionPanel = true;
      this._appStateService.instanceOfProduct = row;
      this._appStateService.selectedRowGrid.next(row);
    }
  }

  public  loadCustomParameters(selectedProduct:string){
    let listParameters = [];
    this._applicationService.getProductParameters2(selectedProduct)
        .subscribe((data:IParameter[] | any[]) =>{
            if(typeof data == "object" && data.length > 0){
 
            }  
        })
  }


  ngOnDestroy(){
    this.dispose();
  //  this.refreshGridDataSubscription$.unsubscribe();
  }

  dispose(){
    this.selectedRow = null;
    this._appStateService.initRightActionPanel = false;
    this._appStateService.instanceOfProduct = undefined;
    this._appStateService.optionValue = undefined;
    this.dataSource = [];
  }
}

