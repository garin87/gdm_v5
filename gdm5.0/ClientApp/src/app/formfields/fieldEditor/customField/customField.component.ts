import { Component, Input, OnInit } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { Subject } from "rxjs";
import { MetadataProperty, ParameterForm, valueUpdatedData } from "src/app/common/objects/common";
import { FormEditorService } from "src/app/common/services/formEditor.service";
import { ParameterDialogComponent } from "./parameterDialog/parameterDialog.component";

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
            console.log('The parameter dialog was closed');
            console.log(result);
            const navPriority = result.parameterPriority == "" || 
                                result.parameterPriority == null ? 0 : result.parameterPriority;

            let newField = new MetadataProperty(result.parameterName, "string", null, null, result.parameterName,"",
            undefined,undefined,undefined,undefined, 0, true, navPriority, false, true,false,true,true,true);
        

            this._formEditorService.customProperties.next([newField]);
            
            let d = new valueUpdatedData(result.parameterName, "", "string", navPriority,undefined, false);
            this.valueUpdated.next(d);

          }
        
        });
    }
    
    ngOnInit(){
        console.log(" init TextEditorComponent");
    }
}


