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

  warning(){
    this.alertService.warning("test alertService - warning");
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

  downloadReport(): void {
    this._applicationService.getPeport().subscribe((data: any) => {
      
      const blob = this.b64toBlob(data.data.fileContents, data.data.contentType);
      let url = window.URL.createObjectURL(blob);
      if ("download" in document.createElement("a")) {
          let a = document.createElement("a");
          a.style.display = "none";
          a.href = url;
          a.setAttribute("download", "test");
          a.click();
      }
      
    })
  }

  b64toBlob(b64Data: string, contentType = '', sliceSize = 512) {
    const byteCharacters = atob(b64Data);
    const byteArrays = [];

    for (let offset = 0; offset < byteCharacters.length; offset += sliceSize) {
      const slice = byteCharacters.slice(offset, offset + sliceSize);

      const byteNumbers = new Array(slice.length);
      for (let i = 0; i < slice.length; i++) {
        byteNumbers[i] = slice.charCodeAt(i);
      }

      const byteArray = new Uint8Array(byteNumbers);
      byteArrays.push(byteArray);
    }

    const blob = new Blob(byteArrays, { type: contentType });
    return blob;
  }
  getCurrencyInfoByAbbreviation(){
    const currency = this._currenciesService.getCurrencyInfoByAbbreviation(this._appStateService.currencyNBRB,"USD");
    console.log(currency);
  }
}
