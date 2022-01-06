import { Component, ElementRef, Input, OnInit, ViewChild } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { Observable, Subject } from "rxjs";
import { ISelectableItem, valueUpdatedData } from "src/app/common/objects/common";

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
    @ViewChild("selectedOption") selectedOption: any;


    name: string;
    typeName:string;
    isDateTimeRange: boolean;
    displayedName: string;
    defaultValue: any;
    required: boolean;
    readOnly: boolean;
    values: any;

    constructor() {
       
    }

    ngOnInit(){
        console.log(" init DateTimePikerComponent");
        console.log(this._property);
        console.log(this.valueUpdated);
 
        this.values = this._property.name == "CurrencyName" ? currencies : [{value: 'dfsdf', viewValue: 'sdfsdf'}];
        this.required = this._property.required != undefined ? this._property.required: false;
        this.readOnly = this._property.readOnly != undefined ? this._property.readOnly: false;
        this.displayedName = this._property.displayedName != undefined ? this._property.displayedName : this._property.name;
        this.name = this._property.name;
        this.value = this._property.defaultValue;

        if(this.value){
            this.valChanged(this.value)
        }
    }

    optionSelected(){
        if(this.readOnly) return;
        
        console.log(this.selectedOption);
        if(this.selectedOption){
            this.valChanged(this.selectedOption.value);
        }

    }

    valChanged(tValue:string){
      
        let convertedValue: any = tValue;
        this.value = tValue;
        
        let d = new valueUpdatedData(this._property.name, convertedValue, this.typeName);
        this.valueUpdated.next(d);
    }
    
}

const currencies = [
    {value: 'USD', viewValue: 'USD'},
    {value: 'EUR', viewValue: 'EUR'},
    {value: 'RUB', viewValue: 'RUB'},
    {value: 'BYR', viewValue: 'BYR'},
];

