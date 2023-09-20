import { ModuleWithProviders } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AuthorizeGuard } from "src/app/authorization/authorize.guard";
import { LoginComponent } from "../authorization/login/login.component";
import { RegisterUserComponent } from "../authorization/register-user/register-user.component";

import { HomeComponent } from "../home/home.component";
import { ModelingComponent } from "../modeling/modeling.component";
import { OrderComponent } from "../order/order.component";
import { ProductComponent } from "../product/product.component";

const routes: Routes = [
    { path: 'home', component: HomeComponent, pathMatch: 'full', canActivate: [AuthorizeGuard] },
    { path: 'product', component: ProductComponent, canActivate: [AuthorizeGuard]},
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterUserComponent, canActivate: [AuthorizeGuard]},
    { path: 'modeling', component: ModelingComponent, canActivate: [AuthorizeGuard]},
    { path: 'order', component: OrderComponent, canActivate: [AuthorizeGuard]},
    
    
    { path: '', redirectTo: '/home', pathMatch: 'full' },
    { path: '**', redirectTo: ''}
  ];

export const Routing : ModuleWithProviders<RouterModule> = RouterModule.forRoot(routes, {});