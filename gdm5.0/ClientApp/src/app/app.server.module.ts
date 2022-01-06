import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { ServerModule } from '@angular/platform-server';

import { AppComponent } from './core/app/app.component';
import { AppModule } from './app.module';

@NgModule({
    imports: [AppModule, ServerModule],
    bootstrap: [AppComponent],
    schemas: [ CUSTOM_ELEMENTS_SCHEMA],
})
export class AppServerModule { }
