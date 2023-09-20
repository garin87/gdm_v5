
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort';
import { MatTable } from '@angular/material/table';
import { Observable, Subscription } from 'rxjs';
import { FilterParameters, gridParameter, IgetProductTypeInstancesRequest, IGridColumnDefinition, IOptionParameterValues, 
         IPaginationAction, IParameter, IParameterSelectionValue, ISortOption, PageFilter, 
         ProductParameterFilter, ProductTypeInstancesRequest } from 'src/app/common/objects/common';
import { ApplicationService } from 'src/app/common/services/application.service';
import { AppStateService } from 'src/app/common/services/appState.service';
import { CustomParameterService } from 'src/app/common/services/custom-parameter.service';
import { FormEditorService } from 'src/app/common/services/formEditor.service';
import { MetadataService } from 'src/app/common/services/metadata.service';
import { CommonUtil } from 'src/app/common/utils/common-utils';


@Component({
  selector: 'grid-control',
  templateUrl: './gridControl.component.html',
  styleUrls: ['./gridControl.component.css']
})
export class GridControlComponent implements OnInit{

  @Input("gridData") gridData: any;
  @Input("gridMetadataType") gridMetadataType: any;
  @Input("selectedProduct") selectedProductName: any;
  @Input("typeFilter") typeFilter: any;
  
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild("table") table!: MatTable<any>;
  
  // filter tile
  dataTile:any;
  customParameters:IParameter[];
  priority:any;

  // grid metadata
  gridMetadata:any;
  gridcolumns:any;
  displayedColumns:any;
  dataSource:any;
  totalRecords:number = 0;
  totalQuantity:number = 0;
  totalPrimeCost:number = 0;
  totalPrimeCostUSD:number = 0;
  totalPrimeCostEUR:number = 0;
  pageSizeDefault:number = 10;
  currentPageSize:number = this.pageSizeDefault;
  paginationParameters:any;
  sortOptionsPublick:ISortOption;
  pageFilterPublick:PageFilter;
  selectedRow:any;
  getProductTypeInstancesRequest:IgetProductTypeInstancesRequest; 

  private changedGridOptionSubscription$:Subscription;
  private refreshGridDataSubscription$:Subscription;
  private loadingGridResultsSubscription$:Subscription;
  private filter_clickByTileFilterSubscription$:Subscription;
  //private loadInstancesParameterProductSubscription$:Subscription;
  private loadCustomParameterValuesByDefaultSubscription$:Subscription;

  public isLoadingResults:boolean = false;
  
  @ViewChild(MatSort) sort: MatSort;
  
  listProps : Observable<string[]>;
  constructor(private _metadataService:MetadataService,
              public  _appStateService:AppStateService,
              private _applicationService : ApplicationService,
              private _customParameterService: CustomParameterService,
              private _FormEditorService:FormEditorService) { };

  announceSortChange(sortState: Sort) {
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

      this.isLoadingResults = true;
      this.sortOptionsPublick = sortOptions;
      this.getGridData(this.selectedProductName,0,this.paginator.pageSize, sortOptions);
  }

  getCustomPrameter(parameterName): IParameter | undefined{
    if(this.customParameters) return this.customParameters.find((item: IParameter) => item.value === parameterName);
    return;
  };

 private setValueCreatedSeletor(nameControl, valueControl){
    this._FormEditorService.listCreatedField.forEach( item =>{

      if(item.propertyName == nameControl.toLowerCase()){
         item.htmlRef.value = valueControl;
      }

    })
  }
  
  private setValuesOfFilterPanel(parameterVal:IParameterSelectionValue){
      let seletedParameter: IParameter | undefined = this.getCustomPrameter(parameterVal?.name);
      if(seletedParameter){
        const option:gridParameter = {
          isParameter: true,
          value: parameterVal.value,
          name: parameterVal.name,
          priority: seletedParameter.priority,
        }

        this.getProductTypeInstancesRequest.Filter.Parameters = this.getProductTypeInstancesRequest.Filter.Parameters.filter(item =>{
            return item.Priority >= option.priority;
        });

        this._appStateService.changedGridOption.next(option);
        this.setValueCreatedSeletor(option.name, option.value);
      }
  }
  
  private resetValuesOfFilterPanel(filter:FilterParameters){
    const delitedOptionPruduct = this.getProductTypeInstancesRequest.Filter.Parameters.filter((item:ProductParameterFilter) => item.Priority < filter.Priority);
    delitedOptionPruduct.forEach((item:ProductParameterFilter)=>{
      if(item){
         this.setValueCreatedSeletor(item.ParameterName, "");
      }
    });
  }

  private SetValueTileFilterParameters(filter: FilterParameters){
    const listOptionFilter = this._appStateService.filter_TileFilterParameters;
    if(Array.isArray(listOptionFilter) && listOptionFilter.length > 0 ){
        this._appStateService.filter_TileFilterParameters = listOptionFilter.filter((item:FilterParameters)=>{
              return item.ParameterName !== filter.ParameterName;
        });
    }
  }

  getTotalQuantity(){
    console.log("-------------- getTotalQuantity")  
    console.log(this.dataSource);
    if(Array.isArray(this.dataSource) && this.dataSource.length > 0){
        const total = this.dataSource.map(t => t?.quantity).reduce((acc, value) => acc + value, 0);
        console.log(total);
        return total;
    }
  }

  ngOnInit(){
      this.gridMetadata = this._metadataService.getMetadataType(this.gridMetadataType);
      this.gridcolumns = this.createColumns(this.gridMetadata);
      this.dataSource = this.gridData?.data || [];
      this.gridcolumns = this.gridcolumns.concat(this.convertParametersToColumn(this.dataSource));
      this.gridcolumns = CommonUtil.sortProperties(this.gridcolumns);
      this.displayedColumns = this.gridcolumns.map(c => c.columnDef);
      this.totalRecords = this.gridData?.totalRecords;
      this.totalQuantity = this.gridData?.totalQuantity;
      this.totalPrimeCost = this.gridData?.totalPrimeCost;
      this.totalPrimeCostUSD = this.gridData?.totalPrimeCostUSD;
      this.totalPrimeCostEUR = this.gridData?.totalPrimeCostEUR;

      this.sortOptionsPublick = null;
      this.pageFilterPublick = new PageFilter();
      this.getProductTypeInstancesRequest = new ProductTypeInstancesRequest();
      
      // need to improve tile filter
      this.loadCustomParameterValuesByDefaultSubscription$ = this._customParameterService.loadCustomParameterValuesByDefault(this.selectedProductName)
             .subscribe((parameterValues:IOptionParameterValues) => {
              if(parameterValues){
                this.priority = parameterValues?.Priority;
                this.dataTile = parameterValues?.ParameterValues;
                this.customParameters = this._customParameterService.filterCustomParamaters(parameterValues?.Parameters);
                           
                this._appStateService.filter_initCreateFilterTileComponent.next(parameterValues);
              }
         }); 

      // if(this.selectedProductName == "Шток хромированный" || this.selectedProductName ==  "Труба хонингованная"){

      //   this.loadCustomParameterValuesByDefaultSubscription$ = this._customParameterService.loadCustomParameterValuesByDefault(this.selectedProductName)
      //        .subscribe((parameterValues:IOptionParameterValues) => {
      //         if(parameterValues){
      //           this.priority = parameterValues?.Priority;
      //           this.dataTile = parameterValues?.ParameterValues;
      //           this.customParameters = this._customParameterService.filterCustomParamaters(parameterValues?.Parameters);
                           
      //           this._appStateService.filter_initCreateFilterTileComponent.next(parameterValues);
      //         }
      //    }); 
      // };
     
      // tile filter
      this.filter_clickByTileFilterSubscription$ = this._appStateService.filter_clickByTileFilter.subscribe((parameterVal:IParameterSelectionValue)=>{
          if(parameterVal){
            const nextParameterOption:IOptionParameterValues = this._customParameterService.createNextParameterOptions(parameterVal, this.customParameters);

            if(!nextParameterOption){
              this.dataTile = undefined;
              this.priority = undefined;
              //  seletedParameter = this.getCustomPrameter(parameterVal?.name);
              this.setValuesOfFilterPanel(parameterVal);
              // if(seletedParameter){
              //   const option:gridParameter = {
              //     isParameter: true,
              //     value: parameterVal.value,
              //     name: parameterVal.name,
              //     priority: seletedParameter.priority,
              //   }
    
              //   this.getProductTypeInstancesRequest.Filter.Parameters = this.getProductTypeInstancesRequest.Filter.Parameters.filter(item =>{
              //        return item.Priority >= option.priority;
              //   });

              //   this._appStateService.changedGridOption.next(option);
              //   this.setValueCreatedSeletor(option.name, option.value);
                
               
              //   // const delitedOptionPruduct = listOptionFilter.filter((item:FilterParameters) => item.Priority < filter.Priority);
              //   // delitedOptionPruduct.forEach((item:FilterParameters)=>{
              //   //   if(item){
              //   //      this.setValueCreatedSeletor(item.ParameterName, "");
              //   //   }
              //   // });
              // }  
              return;
            }
         
            const seletedParameter: IParameter | undefined = this.getCustomPrameter(parameterVal?.name);

            if (!seletedParameter) {
               this.dataTile = undefined;
               this.priority = undefined;
               return;
            }

            const filter:FilterParameters = {
                ParameterName: parameterVal.name,
                ParameterValue: parameterVal.value,
                IsParameter:true,
                Priority:seletedParameter?.priority
            };

            // reset deleted parameters  
            this.resetValuesOfFilterPanel(filter);

            this._appStateService.filter_TileFilterParameters = this._appStateService.filter_TileFilterParameters.filter((item:FilterParameters) => item.Priority >= filter.Priority);
            
            const option:gridParameter = {
              isParameter: true,
              value: parameterVal.value,
              name: parameterVal.name,
              priority: filter.Priority,
            };

            this.getProductTypeInstancesRequest.Filter.Parameters = this.getProductTypeInstancesRequest.Filter.Parameters.filter(item =>{
              return item.Priority >= option.priority;
            });

            this._appStateService.changedGridOption.next(option);

            this.setValueCreatedSeletor(option.name, option.value);
            this.SetValueTileFilterParameters(filter);
          
            this._appStateService.filter_TileFilterParameters.push(filter); // Need to improve
            nextParameterOption.FilterParameters = this._appStateService.filter_TileFilterParameters;
            this.priority = nextParameterOption?.Priority;

            // this.loadInstancesParameterProductSubscription$ = 
            this._customParameterService.loadCustomParameterValues(nextParameterOption).subscribe((data:IOptionParameterValues)=>{
                if( data && data?.ParameterValues.length > 0){
                    this.dataTile = data?.ParameterValues;
                    this._appStateService.filter_initCreateFilterTileComponent.next(data);
                }
            });
          };         
      });

      this.changedGridOptionSubscription$ = this._appStateService.changedGridOption.subscribe(item =>{
        console.log("--------- -------- ---------changedGridOption.subscribe");
        this.dispose();
        if(item){
          
          const filterParameters:FilterParameters = {
            IsParameter: item.isParameter,
            ParameterName: item.name, 
            ParameterValue: item.value,
            Priority: item?.priority ?? 0
          }; 

          if(filterParameters.IsParameter){
            const seletedParameter: IParameter | undefined = this.customParameters.find((item: IParameter) => item.value === item.name);
            if(seletedParameter) filterParameters.Priority = seletedParameter?.priority;
          }
         // this._appStateService.listFilterParameters.push(filterParameters);
          this.getGridData(this.selectedProductName, this.pageFilterPublick.pageNumber = 1,
          this.pageFilterPublick.pageSize = 10, this.sortOptionsPublick, item); 
        }
      });

      this.refreshGridDataSubscription$ = this._appStateService.refreshGridData.subscribe(data => {
        if(data){
             this.refreshGridData();
        }
      })

      this.loadingGridResultsSubscription$ = this._appStateService.LoadingGridResults.subscribe((isLodingGrid)=>{
         this.isLoadingResults = isLodingGrid;
      })
  };
 
  createNextParameterOptions(parameterVal:IParameterSelectionValue):IOptionParameterValues | undefined{
    if(parameterVal){

      const parameterName = parameterVal?.name;
      if (!this.customParameters || this.customParameters.length == 0) return;

      const filterExceptionValues = ["номер"];
      this.customParameters = this.customParameters.filter(item => {
        const lowercaseValue = item?.value?.toLowerCase();
        return lowercaseValue && !filterExceptionValues.includes(lowercaseValue);
      });
      
      const selectParameterIndexArr = this.customParameters.filter((item) => (item.value == parameterName)).map(m => this.customParameters.indexOf(m));
      const selectParameterNextIndex =  selectParameterIndexArr[0] + 1;
      const selectParameterName = this.customParameters[selectParameterNextIndex]?.value;
      const selectParameterPriority = this.customParameters[selectParameterNextIndex]?.priority;
      if(!selectParameterName){
         this.dataTile = undefined;
         this.priority = undefined;
         return undefined;
      }
      this.priority = selectParameterPriority;
      const parameterOption:IOptionParameterValues = {
          NameType: this.selectedProductName,
          NameParameter: selectParameterName,
          IsParameter: true,
          Priority: selectParameterPriority,
          FilterParameters: [],
          Parameters:  [],
          ParameterValues:[]
      };

      
      return parameterOption;
    };
    
    return undefined;
  }

  ngOnChanges(changes): void {
    if(changes['gridData']) {
      if(this.gridData?.data || this.gridData?.currentValue){
        this.sortOptionsPublick = null;
        this.dataSource = this.gridData?.data || this.gridData?.currentValue;
        this.totalRecords = this.gridData?.totalRecords || this.gridData?.totalRecords;

        this.totalQuantity = this.gridData?.totalQuantity;
        this.totalPrimeCost = this.gridData?.totalPrimeCost;
        this.totalPrimeCostUSD = this.gridData?.totalPrimeCostUSD;
        this.totalPrimeCostEUR = this.gridData?.totalPrimeCostEUR;

      }; 
      if(this.table) this.table.renderRows();
    }
  }

  handlePage($event){
    let actionName = this.getPaginationEventName($event);
    this.pageFilterPublick.pageNumber = actionName.pageNumber;
    this.pageFilterPublick.pageSize = actionName.pageSize;
    this.isLoadingResults = true;
    this.getGridData(this.selectedProductName, actionName.pageNumber,
                      actionName.pageSize, this.sortOptionsPublick);
  }

  getGridData(nameProduct:string, pageNumber:number, pageSize:number, sortOptions:ISortOption = null, filter:gridParameter = null){
   
    const productInstancesReq = Object.assign(this.createFilterProductParameters(nameProduct, pageNumber, pageSize, sortOptions, filter));    
    this._applicationService.getProductTypeInstances2(productInstancesReq)
                            .subscribe(response => {
        this.isLoadingResults = false;
        this.dataSource = response.data;
        this.totalRecords = response?.totalRecords;
        this.totalQuantity = response?.totalQuantity;;
        this.totalPrimeCost = response?.totalPrimeCost;
        this.totalPrimeCostUSD = response?.totalPrimeCostUSD;;
        this.totalPrimeCostEUR = response?.totalPrimeCostEUR;;
  
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
                     this.pageFilterPublick.pageSize = 10, this.sortOptionsPublick);
  }

  convertParametersToColumn(gridData){
    if(!gridData) return;
    let parametersData = gridData[0]?.parameters;
    let unicParameters = [];
    gridData.forEach(orderData => {
      orderData.parameters.forEach(param=>{
           let tempP = unicParameters.find(item => item?.name == param.name)?.value;
           if(!tempP){
              unicParameters.push(param);
           }
      });
    })
    
     if(!parametersData) return;
     return  parametersData.map(element => {
        return {
          columnDef : element.name,
          header: element.name,
          isSortable: true,
          cell: (row, column, i) => `${this.getCellValue(row, column, i)}`,
          order: element?.order || element?.priority  
        }
     });
  }
 
  getCellValue(row, column, i){
    let totalRow = "";
    if(row == "row-total"){
      totalRow = column.columnDef == "position" ? "Total" : totalRow;
      totalRow = column.columnDef == "quantity" ? this.totalQuantity?.toFixed(2) : totalRow;
      totalRow = column.columnDef == "primeCost" ? this.totalPrimeCost?.toFixed(2) : totalRow;
      totalRow = column.columnDef == "primeCostEUR" ? this.totalPrimeCostEUR?.toFixed(2) : totalRow;
      totalRow = column.columnDef == "primeCostUSD" ? this.totalPrimeCostUSD?.toFixed(2) : totalRow;
      return typeof totalRow == "undefined" ? "" : totalRow;;
    } 

    let startPageNumber;
    if(this.paginator){
      startPageNumber = (this.paginator.pageSize * this.paginator.pageIndex) + i + 1;
    }else startPageNumber = i + 1;

    let valueColumn = column.columnDef == "position" ? startPageNumber : row[column.columnDef];
    valueColumn = column.columnDef  == "dateOfReceipt"? valueColumn.split("T")[0] : valueColumn;
    valueColumn = column.columnDef  == "dateOfLastChanged"? valueColumn.split("T")[0] : valueColumn;
    valueColumn = column.columnDef  == "standartCost"? (valueColumn * this._appStateService.Cur_OfficialRate_EUR).toFixed(2) : valueColumn;


    if(column.columnDef  == "dateOfLastChanged"){
       if(valueColumn == "01/01/0001 00:00"){
          valueColumn = "";
       }
    }

    if(column.columnDef  == "quantity"){
       // valueColumn = Math.floor(valueColumn * 100) / 100;
    }
    
    

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
     this.selectedRow = row;
     this._appStateService.initRightActionPanel = false;
     if(this.gridMetadataType == "ProductGrid"){
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

  createFilterProductParameters(nameProduct:string, pageNumber:number, pageSize:number, 
    sortOptions:ISortOption = null, filter:gridParameter = null):IgetProductTypeInstancesRequest{
    this.getProductTypeInstancesRequest.NameProductType = nameProduct;
    if(sortOptions){
      this.getProductTypeInstancesRequest.SortOption = {
        name: sortOptions?.name.trim(),
        direction: sortOptions?.direction.trim(),
        isParameter: sortOptions?.isParameter
      };
    }
   
    this.getProductTypeInstancesRequest.PageFilter = {
      pageNumber : pageNumber,
      pageSize: pageSize
    };

    if(filter?.isParameter){
      let isContainParameter:boolean = false;
      this.getProductTypeInstancesRequest.Filter.Parameters.forEach(el =>{
        if(el.ParameterName == filter.name){
          el.Value = filter.value;
          el.Priority = filter.priority;
          isContainParameter = true;
        }
      });

      if(!isContainParameter){
        this.getProductTypeInstancesRequest.Filter.Parameters.push(
          {
            Value:filter.value,
            ParameterName: filter.name,
            ProductId: null,
            ParameterId: null,
            Priority: filter.priority
          }
        ); 
      }
      if(filter?.name !== "тип штока" && (nameProduct === "Шток хромированный" || nameProduct === "Труба хонингованная")){
        this.getProductTypeInstancesRequest.SortOption = {
          name: "quantity",
          direction: "asc",
          isParameter: false
        };
      }
    }else if(filter){
      this.getProductTypeInstancesRequest.Filter[filter?.name] = filter.value;
      if(filter?.name !== "тип штока" && (nameProduct === "Шток хромированный" || nameProduct === "Труба хонингованная")){
        this.getProductTypeInstancesRequest.SortOption = {
          name: "quantity",
          direction: "asc",
          isParameter: false
        };
      }
    }

    return this.getProductTypeInstancesRequest;
  }


  ngOnDestroy(){
    this.getProductTypeInstancesRequest = undefined;
    this.dispose();
    this._appStateService.listFilterParameters = [];
    this._appStateService.filter_TileFilterParameters = [];
    this.customParameters = undefined;
    this.changedGridOptionSubscription$.unsubscribe();
    this.refreshGridDataSubscription$.unsubscribe();
    this.loadingGridResultsSubscription$.unsubscribe();
    this.filter_clickByTileFilterSubscription$.unsubscribe();
    //this.loadInstancesParameterProductSubscription$.unsubscribe();
    //this.loadCustomParameterValuesByDefaultSubscription$.unsubscribe();
  };

  dispose(){
    this.selectedRow = null;
    this._appStateService.initRightActionPanel = false;
    this._appStateService.instanceOfProduct = undefined;
    
  };


}

