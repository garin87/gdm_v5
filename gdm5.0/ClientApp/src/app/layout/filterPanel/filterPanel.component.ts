import { Component, HostListener, Input, OnInit, ViewChild } from '@angular/core';
import { UntypedFormControl } from '@angular/forms';
import { MatSidenav } from '@angular/material/sidenav';
import { AppStateService } from 'src/app/common/services/appState.service';
import {MatAccordion} from '@angular/material/expansion';
import { Observable } from 'rxjs';
import { LabelsService } from 'src/app/common/services/labels.service';
import { FormEditorService } from 'src/app/common/services/formEditor.service';

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

  constructor(private _appStateService:AppStateService,
    public _labelsService: LabelsService,
    public _FormEditorService: FormEditorService) { };

  filterControl = new UntypedFormControl();

  readonly mobileMaxSize = 820;

  ngOnInit(){};

  ngAfterViewInit(){
    if(window.innerWidth < this.mobileMaxSize){
       if(this.accordion){
          this.accordion.closeAll();
       }
    }
  }
  
  ngOnChanges(change){
    if(change["properties"]){}
  };

  reset(){
    this._appStateService.listFilterParameters = [];
    this._appStateService.filter_TileFilterParameters = [];
    this._FormEditorService.listCreatedField = [];
    this._appStateService.changedGridOption.next(undefined);
    if(this.typeFilter === "OptionalOfOrder"){
      this._appStateService.resetOrderTable.next(true);
    }
    if(this.typeFilter === "OptionalOfProduct"){
      this._appStateService.selectedSidePanelValue.next(this._appStateService.selectedInstancePanel);
    }
  }

  ngOnDestroy(){};
}
