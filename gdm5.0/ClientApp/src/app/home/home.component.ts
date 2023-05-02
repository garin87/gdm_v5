import { Component } from '@angular/core';
import { AlertService } from '../alert/alert.service';
import { ApplicationService } from '../common/services/application.service';
import { CurrenciesService } from '../common/services/currencies.service';
import { AppStateService } from '../common/services/appState.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {

  constructor(private alertService : AlertService, 
              private _applicationService: ApplicationService,
              private _currenciesService: CurrenciesService,
              private _appStateService: AppStateService){}
  success(){
    this.alertService.success("test alertService - success");
  }
  error(){
    this.alertService.error("test alertService - ERROR");
  }

  getMetadata(){
    this._applicationService.getMetadata().subscribe(el => console.log(el))
  }

  test(){
    this.alertService.confirmModal("test alertService - ERROR","Make a confirm");
  }
  test2(){
    this.alertService.choiceModal("tttttttttt", "Make a choice");
  }
  test3(){
    this.alertService.warningModal("eeeeeeeddcc", "Warning");
  }

  getNBRBCurrencies(){
    this._currenciesService.getNBRBCurrenciesOnDate("2020-07-07");
  }


  getCurrencyInfoByAbbreviation(){
    const currency = this._currenciesService.getCurrencyInfoByAbbreviation(this._appStateService.currencyNBRB,"USD");
    console.log(currency);
  }
}
