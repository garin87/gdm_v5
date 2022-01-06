import { Component, Input, OnInit } from "@angular/core";

@Component({
    selector: 'field-editor',
    templateUrl: './fieldEditor.component.html',
    styleUrls: ['./fieldEditor.component.css']
  })

export class FieldEditorComponent implements OnInit  {
    @Input("property") property: any;
    @Input("valueUpdated") valueUpdated: any;
    editorName:string;
    
    constructor(){
      this.editorName = "text";

    }
    
    ngOnInit(){
      console.log("create FieldEditorComponent");
      console.log(this.property.typeView);
      this.editorName = this.applyTypeView(this.property.typeView);
      console.log(this.valueUpdated);
    }
  
    applyTypeView(typeView:string){
       if(typeView == "DateTime") return "dateTime";
       if(typeView == "selector") return "selector";
       if(typeView == "parameter") return "parameter";
       return "input"
    }
}