import { Component, Inject, OnInit } from "@angular/core";
import { UntypedFormControl, UntypedFormGroup, Validators } from "@angular/forms";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material/dialog";
import { ParameterForm } from "src/app/common/objects/common";

@Component({
    selector: 'parameter-dialog',
    templateUrl: './parameterDialog.component.html',
    styleUrls: ['./parameterDialog.component.css']
  })
export class ParameterDialogComponent implements OnInit {
    public parameterForm: UntypedFormGroup;
  
    parameterName = new UntypedFormControl( '', [Validators.required,]);
    parameterPriority  = new UntypedFormControl( '');
    
    constructor( public dialogRef: MatDialogRef<ParameterDialogComponent>,
                 @Inject(MAT_DIALOG_DATA) public data: any ) {}
    
    onNoClick(): void {
        console.log("--------- onNoClick():");
        this.dialogRef.close();
    }
    ngOnInit(){
        console.log(" init TextEditorComponent");
        this.parameterForm = new UntypedFormGroup({
            parameterName: this.parameterName ,
            parameterPriority: this.parameterPriority
        });
    }
  
    saveParameters(data:ParameterForm){
       this.dialogRef.close(data);
       console.log("----- --------- -------------- ----------- saveParameters");
       console.log(data);
    }
}


