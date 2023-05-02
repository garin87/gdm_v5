import { Component, ElementRef, Inject, Input, OnInit, ViewChild } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { ParameterForm, valueUpdatedData } from "src/app/common/objects/common";

@Component({
    selector: 'action-dialog',
    templateUrl: './actionDialog.component.html',
    styleUrls: ['./actionDialog.component.css']
})

export class ActionDialogComonent implements OnInit {
    public action:string;
    public titleDialog:string = "Product dialog";
    public actionButton:string = "OK";
    public rejectButton:string = "Cancel";
    public actionOrderButton:string = "Add to cart";
    public isActionOrder:boolean = false;
    public selectedType:string;
    constructor( public dialogRef: MatDialogRef<ActionDialogComonent>,
                 @Inject(MAT_DIALOG_DATA) public data: any ) {}
    
 
    ngOnInit(){
        console.log(" init TextEditorComponent");
        console.log(this.data);
        this.isActionOrder = false;
        if(this.data.commandName){
           this.action = this.data.commandName;
           this.titleDialog = this.data?.actiontitle;
           this.actionButton = this.data?.actionButton;
           this.selectedType = this.data?.typeInstance;
           if(this.data?.actionButton == "Quick Order"){
             this.isActionOrder = true;
           }
        }
        else{
           this.action = "Default";
        }

    }
  
    executeAction(data){
       this.dialogRef.close(data);
    }

    onNoClick(data): void {
        this.dialogRef.close(data);
    }
}


