import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginMenuComponent } from './login-menu/login-menu.component';
import { LoginComponent } from './login/login.component';
import { RouterModule } from '@angular/router';

import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { RegisterUserComponent } from './register-user/register-user.component';
import { ReactiveFormsModule } from '@angular/forms';
import { MatInputModule} from '@angular/material/input';
import { MatIconModule} from '@angular/material/icon';
import { MatFormFieldModule} from '@angular/material/form-field';
import { MatButtonModule} from '@angular/material/button';
import { MatCardModule} from '@angular/material/card';
import { MatMenuModule } from '@angular/material/menu';
import { MatRippleModule } from '@angular/material/core';
import { MatSelectModule} from '@angular/material/select';

@NgModule({
  imports: [
    CommonModule,
    HttpClientModule,
    FormsModule,    
    ReactiveFormsModule, 
    MatInputModule,
    MatIconModule,
    MatMenuModule,
    MatFormFieldModule,
    MatButtonModule,
    MatCardModule,
    RouterModule,
    MatRippleModule,
    MatSelectModule
  ],
  declarations: [ LoginMenuComponent , LoginComponent, RegisterUserComponent],
  exports: [ LoginMenuComponent, LoginComponent, RegisterUserComponent],
  
})
export class ApiAuthorizationModule { }
