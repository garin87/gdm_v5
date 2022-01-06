import { ChangeDetectorRef, Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSidenav } from '@angular/material/sidenav';
import { MatSort, Sort } from '@angular/material/sort';
import { MatTable } from '@angular/material/table';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { debounceTime, map, startWith, tap } from 'rxjs/operators';
import { IGridColumnDefinition, IPaginationAction, SortOptions } from 'src/app/common/objects/common';
import { ApplicationService } from 'src/app/common/services/application.service';
import { appStateService } from 'src/app/common/services/appState.service';
import { MetadataService } from 'src/app/common/services/metadata.service';
import { StringLiteralLike } from 'typescript';


@Component({
  selector: 'grid-control',
  templateUrl: './gridControl.component.html',
  styleUrls: ['./gridControl.component.css']
})
export class GridControlComponent implements OnInit{

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
  pageSizeDefault:number = 5;
  currentPageSize:number = this.pageSizeDefault;
  isLoadingResults:boolean = false;
  paginationParameters:any;
  sortOptionsPublick:SortOptions = null;

  @ViewChild(MatSort) sort: MatSort;
  
  listProps : Observable<string[]>;
  constructor(private _metadataService:MetadataService,
              private _appStateService:appStateService,
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
      let sortOptions:SortOptions = {
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
      this.gridMetadata = this._metadataService.getMetadataType(this.gridMetadataType);
      this.gridcolumns = this.createColumns(this.gridMetadata);
      this.dataSource = this.gridData?.data;
      this.gridcolumns = this.gridcolumns.concat(this.convertParametersToColumn(this.dataSource));
      console.log("---------- convertParametersToColumn"); 
      console.log(this.convertParametersToColumn(this.dataSource));
      this.displayedColumns = this.gridcolumns.map(c => c.columnDef);
      this.totalRecords = this.gridData?.totalRecords;
      this.sortOptionsPublick = null;
  };
 


  ngOnChanges(changes): void {
    console.log("--------------------- -----console.log(changes);");
    console.log(changes);
    if(changes['gridData']) {
      if( this.gridData?.data ){
        this.sortOptionsPublick = null;
        this.dataSource = this.gridData?.data;
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
   this.isLoadingResults = true;
   this.getGridData(this.selectedProductName,actionName.pageNumber,actionName.pageSize, this.sortOptionsPublick);
    
  }

  getGridData(nameProduct, pageNumber, pageSize, sortOptions:SortOptions = null){

    this._applicationService.getProductTypeInstances(nameProduct,pageNumber,pageSize, sortOptions).subscribe(response => {
      console.log("--------- getProductTypeIntances");
      console.log(response);
      this.isLoadingResults = false;
      this.dataSource = response.data;
      this.table.renderRows();
   
    },
    err => {
        this.isLoadingResults = false;
       // this.alertService.error(err.error.message);
        console.log("----  error getProductTypeIntances");
        console.log(err);
    });
  }

  convertParametersToColumn(gridData){
     let paramerts = gridData[0]?.parameters;

     if(!paramerts) return;
     return  paramerts.map(element => {
        return {
          columnDef : element.name,
          header: element.name,
          isSortable: true,
          cell: (row, column, i) => `${this.getCellValue(row, column, i)}`, 
        }
     });
  }
 
  getCellValue(row, column, i){

    let startPageNumber;
    if(this.paginator){
      startPageNumber = (this.paginator.pageSize * this.paginator.pageIndex) + i + 1;
    }else startPageNumber = i + 1;

    let valueColumn = column.columnDef == "position" ? startPageNumber : row[column.columnDef];
    valueColumn = column.columnDef  == "dateOfReceipt"? valueColumn.split("T")[0] : valueColumn;
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
      }
      gridColumns.push(col);
    });

    let t = gridMetadata.map(element => {
      return {
        columnDef : element.name,
        header: element.displayedName,
        isSortable: element.isSortable,
        cell: (item) => `${item}`, 
      }
    });

    console.log("----------------------------- t");
    console.log(t);

    return gridColumns;
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
}

