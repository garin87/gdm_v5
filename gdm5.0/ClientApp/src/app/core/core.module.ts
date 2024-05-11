import { HttpClientModule } from "@angular/common/http";
import { NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { RouterModule } from "@angular/router";
import { AlertModule } from "../alert/alert.module";
import { ApiAuthorizationModule } from "../authorization/api-authorization.module";
import { AuthorizeService } from "../authorization/authorize.service";

import { HomeComponent } from "../home/home.component";
import { NavMenuComponent } from "../nav-menu/nav-menu.component";
import { AppComponent } from "./app/app.component";
import { Routing } from "./routing";

import {MatToolbarModule, } from '@angular/material/toolbar';
import { MatIconModule } from "@angular/material/icon";
import { ApplicationService } from "../common/services/application.service";
import { ProductComponent } from "../product/product.component";
import { MetadataService } from "../common/services/metadata.service";
import { FormPropertiesModule } from "../formfields/formProperties.module";
import { FormEditorService } from "../common/services/formEditor.service";
import { DataAccessorsService } from "../common/services/dataAccessors.service";
import { DataValueService } from "../common/services/dataValue.service";
import { AppStateService } from "../common/services/appState.service";
import { ModelingComponent } from "../modeling/modeling.component";
import { OrderComponent } from "../order/order.component";
import { MatBadgeModule } from "@angular/material/badge";
import { CurrenciesService } from "../common/services/currencies.service";
import { LabelsService } from "../common/services/labels.service";
import { CustomParameterService } from "../common/services/custom-parameter.service";
import { LocalService } from "../common/services/local.service";
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { MatMenuModule } from "@angular/material/menu";
import { MatButtonModule } from "@angular/material/button";
import {MatCardModule} from '@angular/material/card';

@NgModule({
    declarations: [
      AppComponent,
      NavMenuComponent,
      HomeComponent,
      ProductComponent,
      ModelingComponent,
      OrderComponent
    ],
    imports: [
      HttpClientModule,
      FormsModule,
      RouterModule,
      BrowserAnimationsModule,
      ApiAuthorizationModule,
      AlertModule,
      FormPropertiesModule,
      MatToolbarModule,
      MatMenuModule,
      MatButtonModule,
      MatIconModule,
      MatBadgeModule,
      MatProgressSpinnerModule,
      MatBadgeModule,
      MatSidenavModule,
      MatCardModule,
      MatIconModule,
      MatSidenavModule,
      MatButtonModule,
      Routing,
    ],
    providers: [AuthorizeService,
                ApplicationService, 
                MetadataService, 
                FormEditorService, 
                DataAccessorsService,
                DataValueService,
                AppStateService,
                CurrenciesService,
                LabelsService,
                CustomParameterService,
                LocalService]
  })

  export class CoreModule { }