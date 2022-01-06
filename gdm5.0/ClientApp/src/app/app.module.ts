import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { JwtConfig, JwtModule } from "@auth0/angular-jwt";

import { AppComponent } from './core/app/app.component';
import { CoreModule } from './core/core.module';

export function tokenGetter() {
  return localStorage.getItem("jwt");
}


const Jwtconfig : JwtConfig  = {
  tokenGetter: tokenGetter,
  allowedDomains: ["localhost: 5000"],
  disallowedRoutes: [],
}

@NgModule({
  declarations: [
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    CoreModule,
    JwtModule.forRoot({config: Jwtconfig }),

  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
