import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { AuthorizeService} from '../authorize.service';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AlertService } from 'src/app/alert/alert.service';
import { UserForLoginDto } from 'src/app/common/objects/common';
import { ApplicationService } from 'src/app/common/services/application.service';



@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {


  public hide = true;
  public loginForm: FormGroup;
  
  password = new FormControl( '', [Validators.required, Validators.minLength(4)]);
  username  = new FormControl( '', [Validators.required, Validators.minLength(4)]);
  
  constructor(
    private router: Router,
    private cdr: ChangeDetectorRef,
    private applicationService: ApplicationService,
    private alertService:AlertService) { }
  
  ngAfterViewInit(): void {
    this.cdr.detectChanges();
  } 

  async ngOnInit() {
    this.loginForm = new FormGroup({
      username: this.username ,
      password: this.password
    });
  }

  login(loginFormValue) {
    const user: UserForLoginDto = {
      username: loginFormValue.username,
      password: loginFormValue.password
    }
    const credentials = JSON.stringify(user);
    
    this.applicationService.login(credentials)
    .subscribe(response => {
        const token = (<any>response).accessToken;
        const refreshToken = (<any>response).refreshToken;
        const userRole = (<any>response).userRole;
        
        console.log("----- resp");
        console.log(response);
        localStorage.setItem("userRole", userRole);
        localStorage.setItem("jwt", token);
        localStorage.setItem("refreshToken", refreshToken);
        this.router.navigate(["/"]);
    }, err => {
       console.log(err);
       this.alertService.error(err?.error.message)
    });
  }

  

      // const action = this.activatedRoute.snapshot.url[1];
    // switch (action.path) {
    //   case LoginActions.Login:
    //     await this.login(this.getReturnUrl());
    //     break;
    //   case LoginActions.LoginCallback:
    //     await this.processLoginCallback();
    //     break;
    //   case LoginActions.LoginFailed:
    //     const message = this.activatedRoute.snapshot.queryParamMap.get(QueryParameterNames.Message);
    //     this.message.next(message);
    //     break;
    //   case LoginActions.Profile:
    //     this.redirectToProfile();
    //     break;
    //   case LoginActions.Register:
    //     this.redirectToRegister();
    //     break;
    //   default:
    //     throw new Error(`Invalid action '${action}'`);
    // }
  // private async login(returnUrl: string): Promise<void> {
  //   const state: INavigationState = { returnUrl };
  //   const result = await this.authorizeService.signIn(state);
  //   this.message.next(undefined);
  //   switch (result.status) {
  //     case AuthenticationResultStatus.Redirect:
  //       break;
  //     case AuthenticationResultStatus.Success:
  //       await this.navigateToReturnUrl(returnUrl);
  //       break;
  //     case AuthenticationResultStatus.Fail:
  //       await this.router.navigate(ApplicationPaths.LoginFailedPathComponents, {
  //         queryParams: { [QueryParameterNames.Message]: result.message }
  //       });
  //       break;
  //     default:
  //       throw new Error(`Invalid status result ${(result as any).status}.`);
  //   }
  // }

  // private async processLoginCallback(): Promise<void> {
  //   const url = window.location.href;
  //   const result = await this.authorizeService.completeSignIn(url);
  //   switch (result.status) {
  //     case AuthenticationResultStatus.Redirect:
  //       // There should not be any redirects as completeSignIn never redirects.
  //       throw new Error('Should not redirect.');
  //     case AuthenticationResultStatus.Success:
  //       await this.navigateToReturnUrl(this.getReturnUrl(result.state));
  //       break;
  //     case AuthenticationResultStatus.Fail:
  //       this.message.next(result.message);
  //       break;
  //   }
  // }

  // private redirectToRegister(): any {
  //   this.redirectToApiAuthorizationPath(
  //     `${ApplicationPaths.IdentityRegisterPath}?returnUrl=${encodeURI('/' + ApplicationPaths.Login)}`);
  // }

  // private redirectToProfile(): void {
  //   this.redirectToApiAuthorizationPath(ApplicationPaths.IdentityManagePath);
  // }

  // private async navigateToReturnUrl(returnUrl: string) {
  //   // It's important that we do a replace here so that we remove the callback uri with the
  //   // fragment containing the tokens from the browser history.
  //   await this.router.navigateByUrl(returnUrl, {
  //     replaceUrl: true
  //   });
  // }

  // private getReturnUrl(state?: INavigationState): string {
  //   const fromQuery = (this.activatedRoute.snapshot.queryParams as INavigationState).returnUrl;
  //   // If the url is coming from the query string, check that is either
  //   // a relative url or an absolute url
  //   if (fromQuery &&
  //     !(fromQuery.startsWith(`${window.location.origin}/`) ||
  //       /\/[^\/].*/.test(fromQuery))) {
  //     // This is an extra check to prevent open redirects.
  //     throw new Error('Invalid return url. The return url needs to have the same origin as the current page.');
  //   }
  //   return (state && state.returnUrl) ||
  //     fromQuery ||
  //     ApplicationPaths.DefaultLoginRedirectPath;
  // }

  // private redirectToApiAuthorizationPath(apiAuthorizationPath: string) {
  //   // It's important that we do a replace here so that when the user hits the back arrow on the
  //   // browser they get sent back to where it was on the app instead of to an endpoint on this
  //   // component.
  //   const redirectUrl = `${window.location.origin}/${apiAuthorizationPath}`;
  //   window.location.replace(redirectUrl);
  // }
}

// interface INavigationState {
//   [ReturnUrlType]: string;
// }
