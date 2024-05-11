import { AfterViewInit, Component, ElementRef, Input, OnInit, ViewChild } from "@angular/core";
import { Subject } from "rxjs";
import { AlertService } from "src/app/alert/alert.service";
import { ParameterForm, valueUpdatedData } from "src/app/common/objects/common";
import { ParameterDialogComponent } from "../customField/parameterDialog/parameterDialog.component";
import { MatDialog } from "@angular/material/dialog";

@Component({
    selector: 'text-editor',
    templateUrl: './textEditor.component.html',
    styleUrls: ['./textEditor.component.css']
  })
export class TextEditorComponent implements OnInit, AfterViewInit {
    @Input() value: string;
    @Input("valueUpdated") valueUpdated: Subject<valueUpdatedData>;
    @Input("property") _property: any;
    @Input("listCreatedField") _listCreatedField: any;
    
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
    category:string;
    textType:string;

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
        this.value = this._property.value !== undefined ? this._property.value : "";
        this.isTypeArea = false;
        if(this.typeName.toLowerCase() == "number") {
            let number = parseFloat(this.value);
            if(!isNaN(number)){
                this.value = number.toFixed(2);
            }
        }
        this.min =  this.typeName.toLowerCase() == "number" ? "0.001" : null;
        this.step = this.typeName.toLowerCase() == "number" ? "any" : null;
        this.isDeleted = this._property.isDeleted;
        this.category = this._property.category;
        if(this.value){
            this.valChanged(this.value)
        }
    }

    ngAfterViewInit(){
        let p = {
            propertyName: this.name,
            htmlRef: this.textInput.nativeElement
        }
        this._listCreatedField.push(p);
    }

    applyTypeInput(type:string){
      type = type.toLowerCase();
      if(type == "double" || type == "int") this.textType = "number";
      
     // if(type == "double" || type == "int") return "number";
      if(type == "datetime") return "date";
      return "text";
    }

    changeText(){
        if(this.readOnly) return;
        
        if(this.textInput){
            if(this._property.name == "primecost"){}
            
            this.valChanged(this.textInput.nativeElement.value);
        }

    }
    
    editField(){
        const dialogRef = this.dialog.open(ParameterDialogComponent, {
            width: '300px',
            data: {},
          });
        
        let paramDialog = dialogRef.componentInstance;
        paramDialog.parameterName.setValue(this.displayedName);
        paramDialog.parameterPriority.setValue(this.navPriority);
        paramDialog.required.setValue(this.required);
        paramDialog.typeName.setValue(this._property.type);

        dialogRef.afterClosed().subscribe((result:ParameterForm) => {
            if(result){

              let propName = this._property.name;
              this.navPriority = result.parameterPriority
              this.displayedName = result.parameterName;
              let required = result.required;
              let typeName = result.typeName;
              
              const updatedData = new valueUpdatedData(propName, "", typeName ?? "string",this.category, 
              result.parameterPriority, result.parameterName, true, false, false, undefined, required);
              this.valueUpdated.next(updatedData);
            }
          
          });
    }

    deleteField(){

        const message = "Do you want to delete the parameter - " + this._property.name;
        const title = "Delete prameter of product";
        const choices = ["Cancel", "Delete"];
        this._alertService.choiceModal(message, title, choices).subscribe(result =>{
            if(result == "primary"){
                this.displayedName = this.displayedName + " - Deleted";
                this.isEditable = false;
                this.isDeleted = false;
                const propName = this._property.newName == ''? this._property.newName : this._property.name;
                const updatedData = new valueUpdatedData(propName, "", "string",this.category,0,"",false,false,true);
                this.valueUpdated.next(updatedData);
            }
          
        })
    }

    valChanged(tValue:string){
      
        let convertedValue: any = tValue;
        this.value = tValue;
        this.typeName = this.typeName.toLowerCase();
        if(this.typeName == "datatime"){
            if(tValue){
                this.value = new Date(tValue).toISOString();
                convertedValue = new Date(tValue);
            }
        }else if(this.typeName == "int" || this.typeName == "int32"){
            let number = parseFloat(tValue);
            if(!isNaN(number)){
                this.value = number.toString();
                convertedValue = number;
            }else{
                this.value = "";
                convertedValue = null;
            }
        }

        if(this.typeName?.toLowerCase() == "text"){
            if(this.textType?.toLowerCase() == "number"){
                if(convertedValue && typeof convertedValue === 'string' ){
                    convertedValue = convertedValue?.includes(",") ? convertedValue?.replace(",","."): convertedValue;  
                }
                
            }
        }

        let d = new valueUpdatedData(this._property.name, convertedValue, this.typeName);
        this.valueUpdated.next(d);
    }
    
}


