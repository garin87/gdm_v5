import { Component, Input, OnInit } from "@angular/core";
import { Subject } from "rxjs";
import { MetadataProperty, ParameterForm, valueUpdatedData } from "src/app/common/objects/common";
import { FormEditorService } from "src/app/common/services/formEditor.service";
import { ParameterDialogComponent } from "./parameterDialog/parameterDialog.component";
import { MatDialog } from "@angular/material/dialog";

@Component({
    selector: 'customField-editor',
    templateUrl: './customField.component.html',
    styleUrls: ['./customField.component.css']
  })
export class CustomFieldComponent implements OnInit {
    @Input("valueUpdated") valueUpdated: Subject<valueUpdatedData>;
    @Input("property") _property: any;
    constructor(public dialog: MatDialog, private _formEditorService:FormEditorService){}

    openParameterDialog(): void {
        const dialogRef = this.dialog.open(ParameterDialogComponent, {
          width: '300px',
          data: {},
        });
    
        dialogRef.afterClosed().subscribe((result:ParameterForm) => {
          if(result){
            const navPriority = result.parameterPriority == "" || 
                                result.parameterPriority == null ? 0 : result.parameterPriority;

            let newField = new MetadataProperty(result.parameterName, result.typeName, null, null, result.parameterName,"",
            undefined,undefined,undefined, undefined, 0, true, navPriority, result.required, true,false,true,true,true);
        
            this._formEditorService.customProperties.next([newField]);


            let d = new valueUpdatedData(result.parameterName, "", result.typeName, "", navPriority,
            undefined, false,true,false,undefined, result.required);
            this.valueUpdated.next(d);
          }
        });
    }
    
    ngOnInit(){
    }
}


