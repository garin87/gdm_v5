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
    typeName  = new UntypedFormControl('string', [Validators.required,]);
    required  = new UntypedFormControl(false, [Validators.required,]);
    constructor( public dialogRef: MatDialogRef<ParameterDialogComponent>,
                 @Inject(MAT_DIALOG_DATA) public data: any ) {}

    typeNames: any[] = [
        {value: 'string', viewValue: 'String'},
        {value: 'int', viewValue: 'Int'},
        {value: 'double', viewValue: 'Double'},
        {value: 'dateTime', viewValue: 'DateTime'},
    ];

    requiredTypes: any[] = [
        {value: false, viewValue: 'False'},
        {value: true, viewValue: 'True'},
    ];

    onNoClick(): void {
        this.dialogRef.close();
    }

    ngOnInit(){
      //  this.typeName = "String";

        this.parameterForm = new UntypedFormGroup({
            parameterName: this.parameterName ,
            parameterPriority: this.parameterPriority,
            typeName: this.typeName,
            required: this.required,
        });
    }
  
    saveParameters(data:ParameterForm){
       this.dialogRef.close(data);
    }
}


