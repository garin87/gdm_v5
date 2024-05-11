import { Component, ElementRef, HostListener, Inject, Input, OnInit, ViewChild } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material/dialog";
import { BehaviorSubject, Subscription } from "rxjs";
import { AlertService } from "src/app/alert/alert.service";
import { IMetadataProperty } from "src/app/common/objects/common";
import { FormEditorService } from "src/app/common/services/formEditor.service";
import { LabelsService } from "src/app/common/services/labels.service";

@Component({
    selector: 'action-dialog',
    templateUrl: './actionDialog.component.html',
    styleUrls: ['./actionDialog.component.css']
})

export class ActionDialogComonent implements OnInit {
    public action:string;
    public titleDialog:string = "Product dialog";
    public actionButton:string = "OK";
    public rejectButton:string = this._labelsService.labels.productActionPopUpButton_Cancel; // "Cancel";
    public actionOrderButton:string = this._labelsService.labels.productActionPopUpButton_AddToCart; // "Add to cart";
    public isActionOrder:boolean = false;
    public selectedType:string;
    public properties:BehaviorSubject<IMetadataProperty[]>;
    public propertiesSub$:Subscription;

    @HostListener('document:keydown.enter')
    onDocumentKeydownEnter() {
        this.executeAction(this.actionButton)
    }
    
    constructor( public dialogRef: MatDialogRef<ActionDialogComonent>,
                 @Inject(MAT_DIALOG_DATA) public data: any,
                 public _labelsService: LabelsService,
                 private _formEditorService : FormEditorService,
                 private _alertService: AlertService, ) {
        this.properties = new BehaviorSubject<IMetadataProperty[]>(undefined);
    }
    
 
    ngOnInit(){
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
        if(!this.selectedType || this.action == 'Delete'){
            this.dialogRef.close(data);
            return;
        } 
        const id = this.selectedType + "controls-id";
        // OrderProductcontrols-id
        if(!this._formEditorService.isRequiredValue(id)){
          this._alertService.warning("Пожалуйста, заполните обязательные поля");
          return;
        }
        
       this.dialogRef.close(data);
    }

    onNoClick(data): void {
        this.dialogRef.close(data);
    }


    ngOnDestroy(){
        this.properties = new BehaviorSubject<IMetadataProperty[]>(undefined);
      }
}


