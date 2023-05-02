import {  Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatSidenav } from '@angular/material/sidenav';
import {  Observable, of } from 'rxjs';
import { debounceTime} from 'rxjs/operators';
import { AppStateService,  } from 'src/app/common/services/appState.service';


@Component({
  selector: 'app-subSidePanel',
  templateUrl: './subSidePanel.component.html',
  styleUrls: ['./subSidePanel.component.css']
})

export class SubSidePanelComponent implements OnInit{

  @Input("eventToggle") eventToggle: any;
  @Input("properties") properties: any;
  @Input("page") page: any;
  @ViewChild("drawer", { static: true }) drawer : MatSidenav;
  @ViewChild("filterContent") textInput: ElementRef;

  public sidePanel: MatSidenav;
  //public listProps:any;
  public selectedItem:String;

  listProps : Observable<string[]>;

  constructor(private _appStateService:AppStateService) { };
  filterControl = new FormControl();

  ngOnInit(){
    this._appStateService.detectClickOnSubPanel.subscribe(data => {
      if(data){
        this.selectedItem = undefined;
        if(!this.drawer.opened) this.drawer.toggle();
      }
    });
    this._appStateService.selectedSideSubPanelValueName.subscribe( data=>{
      if(!data){
        this.selectedItem = undefined;
      }
    })
    
    this.filterControl.valueChanges.pipe(
       debounceTime(800)
    ).subscribe(data=> {
        this.listProps = of(this._filter(data));
    });
    this._appStateService.refreshSubPanelContentData.subscribe(data => {

      if(data){
        this.refreshElement();
      }
    })

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

    if(element){
      this.selectedItem = element;
      this._appStateService.selectedSideSubPanelValue.next({page: this.page, element: element});
    }

    this.dispose();
  }
 
  refreshElement(){
    this._appStateService.selectedSideSubPanelValue.next({page: this.page, element: this.selectedItem});
  }

  refreshListProps(){
  }
  
  sidenavToggle(event:any){
    console.log(this.eventToggle);
    console.log("---- toggle");
  }

  isSelected(element){
    return this.selectedItem == element;
  }

  onDestroy(){
    this.dispose();
  }

  dispose(){
    this._appStateService.initRightActionPanel = false;
    this._appStateService.instanceOfProduct = undefined;
  }

  private _filter(value: string): string[] {
    const filterValue = value.toLowerCase();
    return this.properties.filter(option => option.toLowerCase().includes(filterValue));
  }
}
