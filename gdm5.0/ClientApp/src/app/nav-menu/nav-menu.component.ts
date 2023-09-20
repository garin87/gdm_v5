import { Component, OnInit } from '@angular/core';
import { AuthorizeService } from '../authorization/authorize.service';
import { ApplicationService } from '../common/services/application.service';
import { AppStateService } from '../common/services/appState.service';
import { LabelsService } from '../common/services/labels.service';


@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})

export class NavMenuComponent implements OnInit{
  isExpanded = false;
  public isAuthenticated: any;
  public hidden = true;
  public countCartProducts = 0;
  //public isAdministrator:boolean;

  constructor(private _authorizeService: AuthorizeService,
              private _applicationService: ApplicationService,
              private _appStateService: AppStateService,
              public _labelsService: LabelsService) { };

  async ngOnInit(){
   // this._authorizeService.userRole.subscribe( el => el == "Admin" ? this.isAdministrator = true : this.isAdministrator = false)
    this._authorizeService.isLogin.subscribe(el => this.isAuthenticated = el );

    this._applicationService.GetCartOrderCount().subscribe( data => {
      if(data && !data?.isEmptyCart){
         this.countCartProducts = data?.countCartProduct; 
         this.hidden = false;
      }
    });

    this._appStateService.cartProductCount.subscribe(productCount =>{
      if(productCount === 0){
        this.hidden = true;
        this.countCartProducts = 0;
      }else{
        this.countCartProducts = this.countCartProducts + productCount; 
        this.hidden = false;
        if(this.countCartProducts === 0)
          this.hidden = true;
      }
    });

  };
  

  toggleBadgeVisibility() {
    this.hidden = !this.hidden;
  }
  
  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }


}
