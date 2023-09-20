import { Component, ElementRef, Input, OnInit, ViewChild } from "@angular/core";
import { UntypedFormControl } from "@angular/forms";
import { Observable, of, Subject } from "rxjs";
import { debounceTime, map } from "rxjs/operators";
import { ISelectableItem, valueUpdatedData } from "src/app/common/objects/common";

@Component({
    selector: 'pickList-editor',
    templateUrl: './pickList.component.html',
    styleUrls: ['./pickList.component.css']
  })
export class PickListComponent implements OnInit {
    @Input() value: string;
    @Input("valueUpdated") valueUpdated: Subject<valueUpdatedData>;
    @Input("property") _property: any;
    @Input("items") items: Observable<ISelectableItem[]>;
    @ViewChild("selectedOption") selectedOption: any;
    @ViewChild("textInput") textInput: ElementRef;

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
    constructor() {}

    ngOnInit(){
        console.log(" init  pickList");
        console.log(this._property);
        console.log(this.valueUpdated);
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

        this.optionFilter.valueChanges.pipe(
            debounceTime(800)
         ).subscribe(data=> {
             this.listOptions = this._filter(data);
         });
    }
    
    ngOnChanges(change){
        if(change["items"]){
          console.log("------------- -------ngOnChanges ========== items");
          this.listOptions = this.items;
        }
    };

    optionSelected(){
        if(this.readOnly) return;
        
        console.log(this.selectedOption);
        if(this.selectedOption){
            this.valChanged(this.selectedOption.value);
        }
        this.inputControl.setValue(this.selectedOption.value);

    }
    changeText(){
        if(this.readOnly) return;
        
        console.log(this.textInput);
        if(this.textInput){
            this.valChanged(this.textInput.nativeElement.value);
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
    // onKey(value) { 
    //   //  this.listOptions = this.search(value);
    // }
    // search(value: string):Observable<ISelectableItem[]> { 
    //     let filterValue = value.toLowerCase();
    //     return this.items.pipe(
    //         filter(
    //             (option:any) => {
    //                 if(option.value.toLowerCase().includes(filterValue)){
    //                     return option
    //                 }else return;
    //             }
    //         )
    //      )
    //     // return this.items.filter((option:ISelectableItem) => option.value.toLowerCase().startsWith(filter));
    // }
    
}

const currencies:ISelectableItem[] = [
    {name: 'USD', value: 'USD'},
    {name: 'EUR', value: 'EUR'},
    {name: 'RUB', value: 'RUB'},
    {name: 'BYR', value: 'BYR'},
];

