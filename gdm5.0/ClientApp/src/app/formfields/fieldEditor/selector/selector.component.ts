import { Component, Input, OnInit, ViewChild } from "@angular/core";
import { UntypedFormControl} from "@angular/forms";
import { Observable, of, Subject, Subscription } from "rxjs";
import { debounceTime, map } from "rxjs/operators";
import { ISelectableItem, valueUpdatedData } from "src/app/common/objects/common";
import { AppStateService } from "src/app/common/services/appState.service";

@Component({
    selector: 'selector-editor',
    templateUrl: './selector.component.html',
    styleUrls: ['./selector.component.css']
  })
export class SelectorComponent implements OnInit {
    @Input() value: string;
    @Input("valueUpdated") valueUpdated: Subject<valueUpdatedData>;
    @Input("property") _property: any;
    @Input("items") items: Observable<ISelectableItem[]>;
    @Input("listCreatedField") _listCreatedField: any;
    @ViewChild("selectedOption") selectedOption: any;
    @ViewChild("filterContent") filterInput: any;

    name: string;
    typeName:string;
    isDateTimeRange: boolean;
    displayedName: string;
    defaultValue: any;
    required: boolean;
    readOnly: boolean;
    values: any;
    listOptions:Observable<ISelectableItem[]>;
    optionFilter = new UntypedFormControl();
    inputControl = new UntypedFormControl();
    
    localvalueUpdated: any;
    category:string

    private valueUpdatedSubscription$:Subscription;
    
    constructor(private _appStateService:AppStateService) {}

    ngOnInit(){
        this.localvalueUpdated = this.valueUpdated;



        this.category = this._property.category;
        this.listOptions = this._property.name?.toLowerCase() == "currency" ? of(currencies) : this.items;
        this.required = this._property.required != undefined ? this._property.required: false;
        this.readOnly = this._property.readOnly != undefined ? this._property.readOnly: false;
        this.displayedName = this._property.displayedName != undefined ? this._property.displayedName : this._property.name;
        this.name = this._property.name;
        this.value = this._property.value;
     
        if(this.value){
            this.valChanged(this.value)
        }
        if(this._property.category === "AddInstanceProduct"
        && (this._property.name === "Name" || this._property.name === "name")){
            this.value = this._appStateService.selectedProductName;
        }
        this.optionFilter.valueChanges.pipe(
            debounceTime(800)
         ).subscribe(data=> {
             this.listOptions = this._filter(data);
         });

         this.setCreatedField();
    }
    
    ngOnChanges(change){
        if(change["items"]){
          this.listOptions = this.items;
        }
    };

    ngAfterViewInit(){
        this.setCreatedField();
    }

    private setCreatedField(){
        let p = {
            propertyName: this.name,
            htmlRef: this.selectedOption
        }
        this._listCreatedField.push(p);
    }

    optionSelected(){
        if(this.readOnly) return;
        
        console.log(this.selectedOption);
        if(this.selectedOption){
            this.valChanged(this.selectedOption.value);
        }

    }

    changeText(){
        this.filterInput;
        if(this.filterInput){
            this.valChanged(this.filterInput.nativeElement.value);
        }
    }
    
    valChanged(tValue:string){
      
        if(tValue == "---------Not set---------"){
            tValue = "";
        }
        
        let convertedValue: any = tValue;
        this.value = tValue;
        const d = new valueUpdatedData(this._property.name, convertedValue, this.typeName, this.category);
        this.localvalueUpdated.next(d);
    }

    clickBySelect(){
        setTimeout(()=>{
            this.filterInput.nativeElement.focus()
        }, 400)
       
    }

    private _filter(value: string):any {
        const filterValue = value.toLowerCase();
        return this.items.pipe(
            map((option:ISelectableItem[]) => {
               const items = option.filter((item:ISelectableItem)=> item.value.toLowerCase().includes(filterValue));
               const empty:ISelectableItem = {
                    "name": "notSet",
                    "value": "---------Not set---------"
               };
               
               return items.length == 0 ? [empty] : items;
            })
        )
    }

    ngOnDestroy() {
        this.localvalueUpdated.unsubscribe();
    }
    // onKey(value) { 
    //   //  this.listOptions = this.search(value);
    // }
}

const currencies:ISelectableItem[] = [
    {name: 'USD', value: 'USD'},
    {name: 'EUR', value: 'EUR'},
    {name: 'RUB', value: 'RUB'},
    {name: 'BYR', value: 'BYR'},
];

