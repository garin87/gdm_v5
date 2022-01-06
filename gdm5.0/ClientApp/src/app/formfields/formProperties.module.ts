import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

import { ReactiveFormsModule } from '@angular/forms';
import { MatInputModule} from '@angular/material/input';
import { MatIconModule} from '@angular/material/icon';
import { MatFormFieldModule} from '@angular/material/form-field';
import { MatButtonModule} from '@angular/material/button';
import { MatCardModule} from '@angular/material/card';
import { MatMenuModule } from '@angular/material/menu';
import { MatNativeDateModule, MatRippleModule } from '@angular/material/core';
import { MatSelectModule} from '@angular/material/select';
import { FormEditorComponent } from './formEditor.component';
import { FieldEditorComponent } from './fieldEditor/fieldEditor.component';
import { TextEditorComponent } from './fieldEditor/textBox/textEditor.component';
import {MatDatepickerModule} from '@angular/material/datepicker'
import { DateTimePikerComponent } from './fieldEditor/dateTimePiker/dateTimePiker.component';
import { SelectorComponent } from './fieldEditor/selector/selector.component';
import {MatDialogModule} from '@angular/material/dialog'
import { CustomFieldComponent } from './fieldEditor/customField/customField.component';
import { ParameterDialogComponent } from './fieldEditor/customField/parameterDialog/parameterDialog.component';
import { FormEditorService } from '../common/services/formEditor.service';
import { MetadataService } from '../common/services/metadata.service';
import {MatTableModule} from '@angular/material/table';

import {MatSidenavModule} from '@angular/material/sidenav';
import { SidePanelComponent } from "../layout/sidePanel/sidePanel.component";
import { GridControlComponent } from '../layout/grid/gridControl.component';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
@NgModule({
  imports: [
    CommonModule,
    HttpClientModule,
    FormsModule,    
    ReactiveFormsModule, 
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSelectModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatSidenavModule,
    MatTableModule,
    MatProgressSpinnerModule,
    MatPaginatorModule,
    MatSortModule
  ],
  declarations: [FormEditorComponent, 
                 FieldEditorComponent, 
                 TextEditorComponent, 
                 DateTimePikerComponent,
                 SelectorComponent,
                 CustomFieldComponent,
                 ParameterDialogComponent,
                 SidePanelComponent,
                 GridControlComponent],
  exports: [FormEditorComponent, 
            FieldEditorComponent, 
            TextEditorComponent, 
            DateTimePikerComponent,
            SelectorComponent,
            CustomFieldComponent,
            ParameterDialogComponent,
            SidePanelComponent,
            GridControlComponent],
  providers: [  
    MatDatepickerModule
  ],
  
})
export class FormPropertiesModule { }
