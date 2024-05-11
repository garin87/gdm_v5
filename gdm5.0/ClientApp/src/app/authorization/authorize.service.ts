import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { BehaviorSubject} from 'rxjs';
import { ApplicationService } from '../common/services/application.service';
import { MetadataService } from '../common/services/metadata.service';

@Injectable({
  providedIn: 'root'
})
export class AuthorizeService {

  constructor(private jwtHelper: JwtHelperService, 
              private _applicationService: ApplicationService,
              private _metadataService: MetadataService){}

  public isLogin: BehaviorSubject<boolean> = new BehaviorSubject(false);
  public userRole:BehaviorSubject<string> = new BehaviorSubject("");

  public async isAuthenticated():Promise<boolean> {
      const token: string = localStorage.getItem("jwt");
      if (token && !this.jwtHelper.isTokenExpired(token)) {
        this.isLogin.next(true);
        this.userRole.next(this.getUserRole());
        return true;
      }
      if(token){
        const isRefreshSuccess = await this.tryRefreshingTokens(token);
        if(isRefreshSuccess === true){
          this.isLogin.next(isRefreshSuccess); 
          this.userRole.next(this.getUserRole());
          return isRefreshSuccess;
        }
      }else{
        this.isLogin.next(false);
        return false;
      }
  }

  private getUserRole():string{
     return localStorage.getItem("userRole");
  }

  private async tryRefreshingTokens(token: string):Promise<boolean> {

    const refreshToken: string = localStorage.getItem("refreshToken");
    if (!token || !refreshToken) { 
      return false;
    }
    const credentials = JSON.stringify({ accessToken: token, refreshToken: refreshToken });
    let isRefreshSuccess: boolean;

    await this._applicationService.refreshToken(credentials).then(response => {
      const newToken = (<any>response).body.accessToken;
      const newRefreshToken = (<any>response).body.refreshToken;
      const userRole = (<any>response).body.userRole;
      localStorage.setItem("jwt", newToken);
      localStorage.setItem("refreshToken", newRefreshToken);
      localStorage.setItem("userRole", userRole);

      isRefreshSuccess = true;
    }, err => {
       console.log(err);
       isRefreshSuccess = false;
    })

     return isRefreshSuccess;
  }


  logOut():void {
    localStorage.removeItem("jwt");
    localStorage.removeItem("refreshToken");
    localStorage.removeItem("userRole");
    this._metadataService.loaded = false;
  }
  
}
