import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({   
     selector: 'csi-alert-modal',   
     templateUrl: './alert.modal.component.html'
})

 
export class AlertModalComponent {

    public messageText: string;    
    public messageClass: string;    
    public title: string;   
    public cancelBtnCaption: string;    
    public primaryBtnCaption: string;

    constructor(private dialogRef: MatDialogRef<AlertModalComponent>) { }
   
    execute(answer: string): void {   
       console.log("----------- alert execute");    
       this.dialogRef.close(answer);    
    }

    closeSelectElement($ev : Event){}
}
