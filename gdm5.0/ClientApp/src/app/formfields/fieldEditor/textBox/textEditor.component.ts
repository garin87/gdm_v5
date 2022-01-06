import { Component, ElementRef, Input, OnInit, ViewChild } from "@angular/core";
import { FormControl, Validators } from "@angular/forms";
import { MatDialog } from "@angular/material/dialog";
import { Subject } from "rxjs";
import { AlertService } from "src/app/alert/alert.service";
import { ParameterForm, parameterUpdatedData, valueUpdatedData } from "src/app/common/objects/common";
import { ParameterDialogComponent } from "../customField/parameterDialog/parameterDialog.component";

@Component({
    selector: 'text-editor',
    templateUrl: './textEditor.component.html',
    styleUrls: ['./textEditor.component.css']
  })
export class TextEditorComponent implements OnInit {
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
        console.log(" init TextEditorComponent");
        console.log(this._property);
        console.log(this.value);
        console.log(this.valueUpdated);
        this.typeName = this.applyTypeInput(this._property.type);
        this.required = this._property.required != undefined ? this._property.required: false;
        this.readOnly = this._property.readOnly != undefined ? this._property.readOnly: false;
        this.displayedName = this._property.displayedName != undefined ? this._property.displayedName : this._property.name;
        this.navPriority = this._property.navPriority != undefined ? this._property.navPriority : 0;
        this.isEditable = this._property.isEditable;
        this.name = this._property.name;
        this.value = this._property.defaultValue !== undefined? this._property.defaultValue : "";
        this.isTypeArea = false;
        this.min =  this.typeName == "number" ? "0.001" : null;
        this.step = this.typeName == "number" ? "any" : null;
        this.isDeleted = this._property.isDeleted;
        if(this.value){
            this.valChanged(this.value)
        }
    }
    
    applyTypeInput(type:string){
      if(type == "Double" || type == "Int") return "number";
      if(type == "DateTime") return "date";

      return "text";
    }

    changeText(){
        if(this.readOnly) return;
        
        console.log(this.textInput);
        if(this.textInput){
            this.valChanged(this.textInput.nativeElement.value);
        }

    }
    editField(){
        console.log("----- edit field");
        console.log(this._property.name);
        const dialogRef = this.dialog.open(ParameterDialogComponent, {
            width: '300px',
            data: {},
          });
        
        let paramDialog = dialogRef.componentInstance;
        paramDialog.parameterName.setValue(this.displayedName);
        paramDialog.parameterPriority.setValue(this.navPriority);

        dialogRef.afterClosed().subscribe((result:ParameterForm) => {
            if(result){
              console.log('The dialog was closed');
              console.log(result);
              console.log(this.textInput);
              let propName = this._property.name;
              this.navPriority = result.parameterPriority
              this.displayedName = result.parameterName;

              const updatedData = new valueUpdatedData(propName, "", "string", result.parameterPriority, result.parameterName,true);
              this.valueUpdated.next(updatedData);
            }
          
          });
    }

    deleteField(){
        console.log("----- delete field");
        console.log(this._property.name);
        const message = "Do you want to delete the parameter - " + this._property.name;
        const title = "Delete prameter of product";
        const choices = ["Cancel", "Delete"];
        this._alertService.choiceModal(message, title, choices).subscribe(result =>{
            console.log("----- delete field result");
            console.log(result);
            if(result == "primary"){
                this.displayedName = this.displayedName + " - Deleted";
                this.isEditable = false;
                this.isDeleted = false;
                const propName = this._property.newName == ''? this._property.newName : this._property.name;
                const updatedData = new valueUpdatedData(propName, "", "string",0,"",false,false,true);
                this.valueUpdated.next(updatedData);
            }
          
        })
    }

    valChanged(tValue:string){
      
        let convertedValue: any = tValue;
        this.value = tValue;
        
        if(this.typeName == "DataTime"){
            if(tValue){
                this.value = new Date(tValue).toISOString();
                convertedValue = new Date(tValue);
            }
        }else if(this.typeName == "int" || this.typeName == "Int32"){
            let number = parseFloat(tValue);
            if(!isNaN(number)){
                this.value = number.toString();
                convertedValue = number;
            }else{
                this.value = "";
                convertedValue = null;
            }
        }

        let d = new valueUpdatedData(this._property.name, convertedValue, this.typeName);
        this.valueUpdated.next(d);
    }
    
}


