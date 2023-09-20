
import { Component, Input, OnChanges } from '@angular/core';
import { Alert } from './alert';

@Component({    
   selector: 'csi-alert',   
   templateUrl: './alert.component.html',
   styleUrls: ['./alert.component.css'],
})

export class AlertComponent implements OnChanges {  
     @Input() alerts: Alert[];  
     statusAlerts: Alert[];

     ngOnChanges(changes) {
        if (changes.alerts) {
          let als = (<Alert[]>changes.alerts.currentValue);
          this.statusAlerts = als.map(sa => sa);
        }    
     }

     close(alert): void {  this.remove(alert); }
    
     remove(alertToRemove: Alert) {
       let i = this.alerts.findIndex(a => a.time == alertToRemove.time);
       if (i != -1) {
         this.alerts.splice(i, 1);
         this.statusAlerts = this.alerts.filter(a => !a.modal);
       } 
     }
}