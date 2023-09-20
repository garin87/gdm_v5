import { Component, Input, OnInit } from "@angular/core";

@Component({
    selector: 'field-editor',
    templateUrl: './fieldEditor.component.html',
    styleUrls: ['./fieldEditor.component.css']
  })

export class FieldEditorComponent implements OnInit  {
    @Input("property") property: any;
    @Input("valueUpdated") valueUpdated: any;
    @Input("listCreatedField") listCreatedField: any;
    editorName:string;
    
    constructor(){
      this.editorName = "text";
    }
    
    ngOnInit(){
      this.editorName = this.applyTypeView(this.property.typeView);
    }
  
    applyTypeView(typeView:string){
       if(typeView == "DateTime") return "dateTime";
       if(typeView == "selector") return "selector";
       if(typeView == "parameter") return "parameter";
       if(typeView == "listparameter") return "listparameter";
       if(typeView == "picklist") return "picklist";
       
       return "input"
    }
}