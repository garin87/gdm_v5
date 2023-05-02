import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthorizeService } from '../authorization/authorize.service';
import { MetadataService } from '../common/services/metadata.service';
import {MatBadgeModule} from '@angular/material/badge';
import { ApplicationService } from '../common/services/application.service';
import { AppStateService } from '../common/services/appState.service';
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

  constructor(private _authorizeService: AuthorizeService,
              private _applicationService: ApplicationService,
              private _appStateService: AppStateService) { };

  async ngOnInit(){
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
