import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { AuthorizeService } from './authorize.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class AuthorizeGuard  {
  constructor(private _authorize: AuthorizeService, 
    private router: Router, private jwtHelper: JwtHelperService, private http: HttpClient) {
  }

  async canActivate(){
    const isAuthenticated = await this._authorize.isAuthenticated();
   
    if (isAuthenticated === true) return isAuthenticated;
      
    this.router.navigate(['/login']);
  }

}
