import { Component, ElementRef, Input, OnInit, ViewChild } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { Subject } from "rxjs";
import { AlertService } from "src/app/alert/alert.service";
import { valueUpdatedData } from "src/app/common/objects/common";

@Component({
    selector: 'button-control',
    templateUrl: './button.component.html',
    styleUrls: ['./button.component.css']
  })
export class ButtonComponent implements OnInit {
    @Input() value: string;
    @Input("valueUpdated") valueUpdated: Subject<valueUpdatedData>;
    @Input("property") _property: any;

    @ViewChild("textInput") textInput: ElementRef;

    name: string;
    typeName:string;
    isTypeArea: boolean;
    displayedName: string;
    defaultValue: any;
    required: boolean;
    readOnly: boolean;
    isEditable: boolean;
    isDeleted: boolean;
    navPriority:string;
    min:any = null;
    step:any = null;

    constructor(public dialog: MatDialog,
                public _alertService : AlertService){}

    ngOnInit(){
        this.typeName = this.applyTypeInput(this._property.type);
        this.required = this._property.required != undefined ? this._property.required: false;
        this.readOnly = this._property.readOnly != undefined ? this._property.readOnly: false;
        this.displayedName = this._property.displayedName != undefined ? this._property.displayedName : this._property.name;
        this.navPriority = this._property.navPriority != undefined ? this._property.navPriority : 0;
        this.isEditable = this._property.isEditable;
        this.name = this._property.name;
        this.value = this._property.defaultValue !== undefined? this._property.defaultValue : "";
        this.isTypeArea = false;
        this.isDeleted = this._property.isDeleted;

    }
    
    applyTypeInput(type:string){
      if(type == "Double" || type == "Int") return "number";
      if(type == "DateTime") return "date";

      return "text";
    }

    changeText(){

    }
       
}


