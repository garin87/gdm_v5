
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
  allowedDomains: ["localhost: 5000", "https://api.nbrb.by"],
  disallowedRoutes: [],
}

@NgModule({
  declarations: [
  ],
  imports: [
    HttpClientModule,
    FormsModule,
    CoreModule,
    JwtModule.forRoot({config: Jwtconfig }),

  ],
  providers : [{ provide: 'ng-cli-universal', useValue: 'serverApp' }],
  bootstrap: [AppComponent]
})
export class AppModule { }
