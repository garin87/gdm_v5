import { Component, OnInit } from '@angular/core';
import { AuthorizeService } from '../authorize.service';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';
import { LabelsService } from 'src/app/common/services/labels.service';

@Component({
  selector: 'app-login-menu',
  templateUrl: './login-menu.component.html',
  styleUrls: ['./login-menu.component.css']
})
export class LoginMenuComponent implements OnInit {
  public isAuthenticated: any;
  public isAdministrator: any;
  public userName: Observable<string>;
  get isAdmin(){return this.isAdministrator}
  constructor(private authorizeService: AuthorizeService,
              private router: Router,
              public _labelsService: LabelsService) { }

  ngOnInit() {

    
    this.authorizeService.isLogin.subscribe( el => this.isAuthenticated = el);
    this.authorizeService.userRole.subscribe( el => el == "Admin" ? this.isAdministrator = true : this.isAdministrator = false);    
  }

  onLogout(){
    this.authorizeService.logOut();
    this.router.navigate(["/login"]);
    this.authorizeService.isLogin.next(false);
  }

}
