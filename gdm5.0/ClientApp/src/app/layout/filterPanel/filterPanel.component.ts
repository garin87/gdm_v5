import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatSidenav } from '@angular/material/sidenav';
import { AppStateService } from 'src/app/common/services/appState.service';
import {MatAccordion} from '@angular/material/expansion';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-filterPanel',
  templateUrl: './filterPanel.component.html',
  styleUrls: ['./filterPanel.component.css']
})

export class filterPanelComponent implements OnInit{
  @ViewChild(MatAccordion) accordion: MatAccordion;
  @Input("eventToggle") eventToggle: any;
  @Input("selectedProduct") selectedProduct: any;
  @Input("typeFilter") typeFilter: any;

  public sidePanel: MatSidenav;
  public selectedItem:String;

  listProps : Observable<string[]>;

  constructor(private _appStateService:AppStateService) { };
  filterControl = new FormControl();

  ngOnInit(){
   // this.accordion.openAll();
    //this.sidePanel.open();
  };

  ngOnChanges(change){
    if(change["properties"]){}
  };

  select(element){}

  reset(){
    console.log("--- reset --");
    if(this.typeFilter === "OptionalOfOrder"){
      this._appStateService.resetOrderTable.next(true);
    }
    if(this.typeFilter === "OptionalOfProduct"){
      this._appStateService.selectedSidePanelValue.next(this._appStateService.selectedInstancePanel);
    }
  }

}
