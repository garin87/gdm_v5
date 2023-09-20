import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({   
     selector: 'csi-alert-modal',   
     templateUrl: './alert.modalObject.component.html'
})

 
export class AlertModalObjectComponent {

    public messageText: string;    
    public messageClass: string;    
    public title: string;   
    public cancelBtnCaption: string;    
    public primaryBtnCaption: string;

    constructor(private dialogRef: MatDialogRef<AlertModalObjectComponent>) { }
   
    execute(answer: string): void {   
       this.dialogRef.close(answer);    
    }

    closeSelectElement($ev : Event){}
}
