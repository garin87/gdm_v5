

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AlertService } from './alert.service';
import { AlertComponent } from './alert.component';
import { AlertModalComponent } from './alert.modal.component';
import { ClosableDirective } from './closable.directive';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatButtonModule } from '@angular/material/button';
import { AlertModalObjectComponent } from './alert.modalObject.component';


@NgModule({
    declarations: [
        AlertComponent,
        AlertModalComponent,
        AlertModalObjectComponent,
        ClosableDirective,
        
    ],
    imports: [
        CommonModule,
        MatDialogModule,
        MatSnackBarModule,
        MatButtonModule
    ],
    
    exports: [
        AlertComponent,
        AlertModalComponent,
        AlertModalObjectComponent
    ],
    providers: [
        AlertService,
    ]
})
export class AlertModule {
}


