import { Component, ElementRef, Input, OnInit, ViewChild } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";

import { Subject } from "rxjs";
import { valueUpdatedData } from "src/app/common/objects/common";

@Component({
    selector: 'dateTimePiker-editor',
    templateUrl: './dateTimePiker.component.html',
    styleUrls: ['./dateTimePiker.component.css']
  })
export class DateTimePikerComponent implements OnInit {
    @Input() value: any;
    @Input("valueUpdated") valueUpdated: Subject<valueUpdatedData>;

    @Input("property") _property: any;

    @ViewChild("dateInput") textInput: ElementRef;
    date = new FormControl(new Date());
  //  serializedDate = new FormControl((new Date()).toISOString())
    name: string;
    typeName:string;
    isDateTimeRange: boolean;
    displayedName: string;
    defaultValue: any;
    category: string;
    required: boolean;
    readOnly: boolean;
    campaignOne: FormGroup;
    campaignTwo: FormGroup;
    localvalueUpdated:any;
  
    constructor() {
      const today = new Date();
      const month = today.getMonth();
      const year = today.getFullYear();
  
      this.campaignOne = new FormGroup({
        start: new FormControl(new Date(year, month, 13)),
        end: new FormControl(new Date(year, month, 16))
      });
  
      this.campaignTwo = new FormGroup({
        start: new FormControl(new Date(year, month, 15)),
        end: new FormControl(new Date(year, month, 19))
      });
    }

    ngOnInit(){
        console.log(" init DateTimePikerComponent");
        console.log(this._property);
        console.log(this.valueUpdated);
        //this.typeName = this.applyTypeInput(this._property.type);
        this.localvalueUpdated = this.valueUpdated;
        this.required = this._property.required != undefined ? this._property.required: false;
        this.readOnly = this._property.readOnly != undefined ? this._property.readOnly: false;
        this.displayedName = this._property.displayedName != undefined ? this._property.displayedName : this._property.name;
        this.name = this._property.name;
        this.value = this._property.value || this._property.defaultValue;
        this.date.setValue(this._property.value); 
        this.isDateTimeRange = false;
        this.category = this._property.category;
        if(!this.value){
           this.value = new Date();
           if(this.name == "filterEndDate"){
              this.value.setDate(this.value.getDate() + 2);
           }
        }

        if(this.value){
           this.valChanged(this.value)
        }
    }
    

    changeDate(){
      console.log("------------------------------------------ changeDate")
        if(this.readOnly) return;
        
        console.log(this.textInput);
        if(this.textInput){
            this.valChanged(this.textInput.nativeElement.value);
        }

    }

    valChanged(tValue:any){
        let convertedValue: any = tValue;
        if(tValue){
          //  convertedValue = new Date(tValue).toISOString();
            var dTimezone = new Date();
            var offset = dTimezone.getTimezoneOffset() / 60;
            convertedValue = new Date(Date.parse(convertedValue));
            convertedValue.setHours(convertedValue.getHours() + offset);
        }

        let d = new valueUpdatedData(this._property.name, convertedValue, this.typeName, this.category);
        this.localvalueUpdated.next(d);
    }
    
}


