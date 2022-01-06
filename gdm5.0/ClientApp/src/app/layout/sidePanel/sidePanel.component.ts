import { ChangeDetectorRef, Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatSidenav } from '@angular/material/sidenav';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { debounceTime, map, startWith, tap } from 'rxjs/operators';
import { appStateService } from 'src/app/common/services/appState.service';
import { MetadataService } from 'src/app/common/services/metadata.service';
import { StringLiteralLike } from 'typescript';


@Component({
  selector: 'app-sidePanel',
  templateUrl: './sidePanel.component.html',
  styleUrls: ['./sidePanel.component.css']
})

export class SidePanelComponent implements OnInit{

  @Input("eventToggle") eventToggle: any;
  @Input("properties") properties: any;
  @ViewChild("drawer", { static: true }) drawer : MatSidenav;
  @ViewChild("filterContent") textInput: ElementRef;

  public sidePanel: MatSidenav;
  //public listProps:any;
  public selectedItem:String;

  listProps : Observable<string[]>;

  constructor(private _appStateService:appStateService) { };
  filterControl = new FormControl();

  ngOnInit(){

    this._appStateService.detectClickOnPanel.subscribe(data => {
      if(data){
        this.selectedItem = undefined;
        this.drawer.toggle();
      }
    });

   this.filterControl.valueChanges.pipe(
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
    if(true){//this.selectedItem != element
       this.selectedItem = element;
       this._appStateService.selectedSidePanelValue.next(element);
    }
    
  }
 
  sidenavToggle(event:any){
    console.log(this.eventToggle);
    console.log("---- toggle");
  }

  isSelected(element){
    return this.selectedItem == element;
  }

  private _filter(value: string): string[] {
    const filterValue = value.toLowerCase();
    return this.properties.filter(option => option.toLowerCase().includes(filterValue));
  }
}
