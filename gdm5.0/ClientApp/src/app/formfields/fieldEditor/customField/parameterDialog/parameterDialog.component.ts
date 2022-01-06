import { Component, ElementRef, Inject, Input, OnInit, ViewChild } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { Subject } from "rxjs";
import { ParameterForm, valueUpdatedData } from "src/app/common/objects/common";

@Component({
    selector: 'parameter-dialog',
    templateUrl: './parameterDialog.component.html',
    styleUrls: ['./parameterDialog.component.css']
  })
export class ParameterDialogComponent implements OnInit {
    public parameterForm: FormGroup;
  
    parameterName = new FormControl( '', [Validators.required,]);
    parameterPriority  = new FormControl( '');
    
    constructor( public dialogRef: MatDialogRef<ParameterDialogComponent>,
                 @Inject(MAT_DIALOG_DATA) public data: any ) {}
    
    onNoClick(): void {
        console.log("--------- onNoClick():");
        this.dialogRef.close();
    }
    ngOnInit(){
        console.log(" init TextEditorComponent");
        this.parameterForm = new FormGroup({
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


