import { Injectable } from '@angular/core';
import { IResultStatus } from '../common/objects/common'; // any 
import { Alert, AlertTypes } from './alert';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AlertModalComponent } from './alert.modal.component';
import { AlertModalObjectComponent } from './alert.modalObject.component';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable()
export class AlertService {
    public alerts: Alert[] = [];
    constructor(       
    private _dialog: MatDialog,        
    //private _labelService: LabelService,        
    private _snackBar: MatSnackBar) {    }

    public init() : void {        
       this.alerts = [];    
    }

    public displayResultStatus(status: IResultStatus) {      
      this.addAlert(AlertTypes.success, status, false);  
    }

    public error(err: string | IResultStatus | any) {      
      this.addAlert(AlertTypes.danger, err, false);    
    }

    public warning(err: string | IResultStatus | any) {      
      this.addAlert(AlertTypes.warning, err, false);    
    }

    public success(err: string | IResultStatus | any) {        
      this.addAlert(AlertTypes.success, err, false);    
    }

    public displayResultStatusModal(status: IResultStatus) {       
      this.addAlert(AlertTypes.success, status, true);   
    }

    public warningModal(err: string | IResultStatus | any, title?: string | any): Observable<boolean> {        
       return this.openModalAlert(AlertTypes.warning, err, title).pipe(map(action => action == "primary"));  
    }  
     
    public warningModalObject(err: string | IResultStatus | any, title?: string | any): Observable<boolean> {        
        return this.openModaObjectAlert(AlertTypes.warning, err, title).pipe(map(action => action == "primary"));  
    } 

    public errorModal(err: string | IResultStatus | any, title?: string | any) {   
             return this.openModalAlert(AlertTypes.danger, err, title);  
    }

    public confirmModal(txt: string | any, title?: string | any): Observable<boolean>  {
           return this.openModalAlert(AlertTypes.choice, txt, title).pipe(map(action => action == "primary"));    
    }

    public choiceModal(txt: string | any, title?: string | any, choices?: any[]): Observable<string>  {     
       return this.openModalAlert(AlertTypes.choice, txt, title, choices);   
    }

    private resolveText(textOrLabelOrStatus: string | IResultStatus | any): string {     
         let txt: string = textOrLabelOrStatus;
//         if (typeof textOrLabelOrStatus === "string")  txt = <string>textOrLabelOrStatus;       
//         else if ((<any>textOrLabelOrStatus).Name) {           
//             let lbl = <any>textOrLabelOrStatus;            
//             txt = lbl.arguments ?
//                  this._labelService.getLabelwithArguments(lbl.Name, lbl.DefaultValue, lbl.arguments) :
//                  this._labelService.getLabel(lbl.Name, lbl.DefaultValue);   
//          }   
//         else {            
//             let rs = <IResultStatus>textOrLabelOrStatus;
//             txt = rs.Message || (rs.ExceptionData && rs.ExceptionData.Description) || "";        
//         }

        return txt;  
    }

    private createAlert(t: AlertTypes, err: string | IResultStatus | any, modal: boolean, title?: string | any): Alert {
        let alert: Alert = new Alert(t, this.resolveText(err));    
        alert.modal = modal;        
        alert.title = this.getDefaultTitle(t);
        if (title !== undefined) {          
            alert.title = this.resolveText(title);        
        }
        return alert;    
    }

    private addAlert(t: AlertTypes, err: string | IResultStatus | any, modal: boolean): void {
        let newAlerts = this.alerts.map<Alert>(a => a);        
        let alert = this.createAlert(t, err, modal);        
        if (alert) {            
           newAlerts.push(alert);            
            let snackBarConfig;
            switch (alert.type) {                
               case 1:                   
                    snackBarConfig = {                  
                         panelClass: 'snack-bar-warning',
                         duration: 7000          
                    };                    
                     break;               
                case 3:                   
                    snackBarConfig = {                      
                         panelClass: 'snack-bar-success',          
                         duration: 5000                 
                    };                   
                 break;            
            }
            let ref = this._snackBar.open(alert.message, 'Закрыть', snackBarConfig);    
            ref.onAction().subscribe(action => {});      
        }                            
        this.alerts = newAlerts;   
    }

    private openModaObjectAlert(alertType: AlertTypes, messageObject: string | IResultStatus | any, title?: string | any, choices?: any[]): Observable<string> {      
           // let modalAlert = this.createAlert(alertType, txt, true, title);
           let modalRef = this._dialog.open(AlertModalObjectComponent); 
           let am = modalRef.componentInstance;       
           am.messageText = messageObject;    
           //  am.messageClass = modalAlert.class;      
           am.title = title;       
           am.cancelBtnCaption = 'Cancel' 
           am.primaryBtnCaption = 'OK'
         
           return modalRef.afterClosed();   
    }

    private openModalAlert(alertType: AlertTypes, txt: string | IResultStatus | any, title?: string | any, choices?: any[]): Observable<string> {      
       let modalAlert = this.createAlert(alertType, txt, true, title);
       let modalRef = this._dialog.open(AlertModalComponent, {disableClose: true}); 
       let am = modalRef.componentInstance;       
       am.messageText = modalAlert.message;    
       am.messageClass = modalAlert.class || "teeee";      
       am.title = modalAlert.title;       
       am.cancelBtnCaption = 'Cancel' //(alertType == AlertTypes.choice) ? this._labelService.getLabel('PS_ACTION_CANCEL', 'Cancel') : '';  
       am.primaryBtnCaption = 'OK'//this._labelService.getLabel('PS_ACTION_OK', 'Ok');
       if( !choices ){            
          am.cancelBtnCaption = 'Cancel'// (alertType == AlertTypes.choice) ? this._labelService.getLabel('PS_ACTION_CANCEL', 'Cancel') : '';        
          am.primaryBtnCaption = 'OK' //this._labelService.getLabel('PS_ACTION_OK', 'Ok');        
       }    
       else{            
          am.cancelBtnCaption = choices[0] //this._labelService.getLabel(choices[0].Name, choices[0].DefaultValue); 
          am.primaryBtnCaption = choices[1] //this._labelService.getLabel(choices[1].Name, choices[1].DefaultValue);  
       }       
       return modalRef.afterClosed();   
    }
   
    private getDefaultTitle(alertType: AlertTypes): string {     
      let t = '';       
         switch (alertType) {            
            case AlertTypes.success:          
                t = 'Success'; break;            
            case AlertTypes.danger:               
                t = 'Error'; break;           
            case AlertTypes.warning:                
                t = 'Warning'; break;            
            case AlertTypes.choice:                
                t = 'Confirm'; break;            
            case AlertTypes.info:                
                t = 'Info'; break;        
        }   
             
      return t;   
    }
}
