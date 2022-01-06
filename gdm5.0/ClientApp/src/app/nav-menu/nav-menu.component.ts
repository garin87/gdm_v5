import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthorizeService } from '../authorization/authorize.service';
import { MetadataService } from '../common/services/metadata.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit{
  isExpanded = false;
  public isAuthenticated: any;
  
  constructor(private _authorizeService: AuthorizeService,
              private _metadataService : MetadataService ) { };

  async ngOnInit(){
    this._authorizeService.isLogin.subscribe(el => this.isAuthenticated = el );
  };

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}
