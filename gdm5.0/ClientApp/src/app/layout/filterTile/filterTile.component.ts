import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { UntypedFormControl } from '@angular/forms';
import { MatSidenav } from '@angular/material/sidenav';
import { AppStateService } from 'src/app/common/services/appState.service';
import { MatAccordion } from '@angular/material/expansion';
import { Observable } from 'rxjs';
import { LabelsService } from 'src/app/common/services/labels.service';
import { IParameterSelectionValue } from 'src/app/common/objects/common';

@Component({
  selector: 'app-filterTile',
  templateUrl: './filterTile.component.html',
  styleUrls: ['./filterTile.component.css']
})

export class FilterTileComponent implements OnInit{
  @ViewChild(MatAccordion) accordion: MatAccordion;
  @Input("eventToggle") eventToggle: any;
  @Input("selectedProduct") selectedProduct: any;
  @Input("dataTile") dateTile: any;
  @Input("priority") priority: any;

  public sidePanel: MatSidenav;
  public selectedItem:String;
  public titleTile:String;
  public clickedButtonIndex: number | null = null;
  public cliked:boolean;

  listProps : Observable<string[]>;

  constructor(private _appStateService:AppStateService, public _labelsService: LabelsService) { };

  filterControl = new UntypedFormControl();

  ngOnInit(){
    if(this.dateTile){
      
    }
  };

  ngOnChanges(change){
    if(change["dateTile"]){
      if(change["dateTile"].currentValue && change["dateTile"].currentValue?.length > 0){
        this.titleTile = change["dateTile"].currentValue[0].name.toUpperCase();
      };
    };
  };

  selectTile(element, i:number){
    this.clickedButtonIndex = i;
    const selectedParameter:IParameterSelectionValue = {
        name: element.name,
        value: element.value,
        typeProductName: this.selectedProduct,
    };
    this._appStateService.filter_clickByTileFilter.next(selectedParameter);
  }

  reset(){
    console.log("--- reset --");
  }

  ngOnDestroy(){
    this.clickedButtonIndex = 0;
    this.titleTile = undefined;
  };
}
