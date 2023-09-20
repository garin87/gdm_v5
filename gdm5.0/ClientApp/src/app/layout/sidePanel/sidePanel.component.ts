import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { UntypedFormControl } from '@angular/forms';
import { MatSidenav } from '@angular/material/sidenav';
import { Observable, Subscription, of } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { commonConst } from 'src/app/common/objects/commonConst';

import { AppStateService,  } from 'src/app/common/services/appState.service';
import { LabelsService } from 'src/app/common/services/labels.service';


@Component({
  selector: 'app-sidePanel',
  templateUrl: './sidePanel.component.html',
  styleUrls: ['./sidePanel.component.css']
})

export class SidePanelComponent implements OnInit{

  @Input("eventToggle") eventToggle: any;
  @Input("properties") properties: any;
  @Input("page") page: any;
  @ViewChild("drawer", { static: true }) drawer : MatSidenav;
  @ViewChild("filterContent") textInput: ElementRef;

  public sidePanel: MatSidenav;
  //public listProps:any;
  public selectedItem:String;

  listProps : Observable<string[]>;
  private detectClickOnPanelSubscription$:Subscription;
  private valueChangesSubscription$:Subscription;
  readonly mobileMaxSize = 820;
  constructor(private _appStateService:AppStateService,
    public _labelsService: LabelsService) { };
  filterControl = new UntypedFormControl();

  ngOnInit(){
    
    this.detectClickOnPanelSubscription$ = this._appStateService.detectClickOnPanel.subscribe(data => {
      if(data){
        this.selectedItem = undefined;
        // if(this.properties){
        //   this.drawer.toggle();
        // }
        if(this.page == "product" || this.page == "modeling"){
          
          if(window.innerWidth < commonConst.mobileMaxSize){
             this.drawer.mode = "over";
          }  
        }
       
        
        this.drawer.toggle();
      }
    });

  this.valueChangesSubscription$ = this.filterControl.valueChanges.pipe(
       debounceTime(800)
    ).subscribe(data=> {
        this.listProps = of(this._filter(data));
    });
  };

  ngOnChanges(change){
    if(change["properties"]){
      console.log("------------- -------ngOnChanges ========== properties");
      //this.listProps = this.properties;
      this.listProps = of(this.properties);
    }
  };

  select(element){
    console.log("-------- select");
    console.log(element);

    if(true){
       if(this.page == "product"){
        this.selectedItem = element;
        this._appStateService.selectedSidePanelValue.next(element);

        if(window.innerWidth < this.mobileMaxSize){
           this.drawer.close();
        }
       }else if(this.page == "modeling"){

        if(window.innerWidth < this.mobileMaxSize){
          this.drawer.close();
        }
        this.selectedItem = element;
        this._appStateService.selectedSidePaneModelingValue.next(element);
       }
    }

    this.dispose();
  }
 
  sidenavToggle(event:any){
    console.log(this.eventToggle);
    console.log("---- toggle");
  }

  isSelected(element){
    return this.selectedItem == element;
  }

  ngOnDestroy(){
    this.dispose();
    this.detectClickOnPanelSubscription$.unsubscribe();
    this.valueChangesSubscription$.unsubscribe();
  }

  dispose(){
    this._appStateService.initRightActionPanel = false;
    this._appStateService.instanceOfProduct = undefined;
  }

  private _filter(value: string): string[] {
    const filterValue = value.toLowerCase();
    return this.properties.filter(option => option.Name.toLowerCase().includes(filterValue));
  }
}
