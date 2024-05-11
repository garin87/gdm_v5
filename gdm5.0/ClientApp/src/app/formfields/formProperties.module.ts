import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

import { ReactiveFormsModule } from '@angular/forms';
import { MatIconModule} from '@angular/material/icon';
import { MatNativeDateModule } from '@angular/material/core';
import { FormEditorComponent } from './formEditor.component';
import { FieldEditorComponent } from './fieldEditor/fieldEditor.component';
import { TextEditorComponent } from './fieldEditor/textBox/textEditor.component';
import {MatDatepickerModule} from '@angular/material/datepicker'
import { DateTimePikerComponent } from './fieldEditor/dateTimePiker/dateTimePiker.component';
import { SelectorComponent } from './fieldEditor/selector/selector.component';
import { CustomFieldComponent } from './fieldEditor/customField/customField.component';
import { ParameterDialogComponent } from './fieldEditor/customField/parameterDialog/parameterDialog.component';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatSidenavModule } from '@angular/material/sidenav';
import { SidePanelComponent } from "../layout/sidePanel/sidePanel.component";
import { GridControlComponent } from '../layout/grid/gridControl.component';
import { MatSortModule } from '@angular/material/sort';
import { filterPanelComponent } from '../layout/filterPanel/filterPanel.component';
import { ParameterSelectorsComponent } from './fieldEditor/parametrSelectors/parameterSelectors.component';
import { rightActionPanelComponent } from '../layout/rightActionPanel/rightActionPanel.component';
import { ActionDialogComonent } from '../layout/actionDialog/actionDialog.component';
import { SubSidePanelComponent } from '../layout/subSidePanel/subSidePanel.component';
import { gridExpandRowComponent } from '../layout/gridExpandRow/gridExpandRow.component';
import { gridPlainComponent } from '../layout/gridPlain/gridPlain.component';
import { PickListComponent } from './fieldEditor/pickList/pickList.component';
import { FilterTileComponent } from '../layout/filterTile/filterTile.component';
import { BaseFilterPanelComponent } from '../layout/baseFilterPanel/baseFilterPanel.component';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatPaginatorModule } from '@angular/material/paginator';
import { BaseFieldEditorComponent } from './baseFieldEditor/baseFieldEditor.component';

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
    MatSortModule,
    MatExpansionModule
  ],
  declarations: [FormEditorComponent, 
                 FieldEditorComponent, 
                 TextEditorComponent, 
                 DateTimePikerComponent,
                 SelectorComponent,
                 CustomFieldComponent,
                 ParameterDialogComponent,
                 ParameterSelectorsComponent,
                 SidePanelComponent,
                 SubSidePanelComponent,
                 rightActionPanelComponent,
                 GridControlComponent,
                 gridExpandRowComponent,
                 gridPlainComponent,
                 filterPanelComponent,
                 FilterTileComponent,
                 BaseFilterPanelComponent,
                 ActionDialogComonent,
                 PickListComponent,
                 BaseFieldEditorComponent],
  exports: [FormEditorComponent, 
            FieldEditorComponent, 
            TextEditorComponent, 
            DateTimePikerComponent,
            SelectorComponent,
            CustomFieldComponent,
            ParameterDialogComponent,
            ActionDialogComonent,
            ParameterSelectorsComponent,
            SidePanelComponent,
            SubSidePanelComponent,
            rightActionPanelComponent,
            GridControlComponent,
            gridExpandRowComponent,
            gridPlainComponent,
            filterPanelComponent,
            FilterTileComponent,
            BaseFilterPanelComponent,
            PickListComponent,
            BaseFieldEditorComponent],
  providers: [  
    MatDatepickerModule
  ],
  
})
export class FormPropertiesModule { }
