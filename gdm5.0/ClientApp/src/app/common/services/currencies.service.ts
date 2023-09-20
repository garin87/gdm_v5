import { Injectable } from "@angular/core";
import { ApplicationService } from "./application.service";
import { AppStateService } from "./appState.service";
import { AlertService } from "src/app/alert/alert.service";
import { Observable } from "rxjs";
import { LocalService } from "./local.service";
import { IDictionaryArray } from "../objects/common";


@Injectable()
export class CurrenciesService {
    
    private alltypes:any;
    get metadataTypes():any { return this.alltypes};

    public loaded:boolean = false;
    private Cur_AbbreviationEUR = "EUR";
    private Cur_AbbreviationUSD = "USD";
    constructor(private _applicationService: ApplicationService,
                private _appStateService: AppStateService,
                private alertService:AlertService,
                private _localService:LocalService ) {

            this.loaderCurrencies();
    }

    private loaderCurrencies(){
        const currenciesRateToday = this._localService.getCurrenciesRateToday();
        const dateAMToday = this.getDateAMToday();
        if(currenciesRateToday && currenciesRateToday.key === dateAMToday){
           this.getCurrencyInfoUSDandEUR(currenciesRateToday.valueArray);
        }else{
           this.getNBRBCurrenciesCurrentDate();
        }
    }

    private getDateAMToday(){
        let dateToday = new Date().toJSON();
        let dateWithoutTime = dateToday.split('T')[0];
        return dateWithoutTime + "T00:00:00";
    }

    private getLastRate(){
        const currenciesRateLast = this._localService.getCurrenciesRateLast();
        if(currenciesRateLast.key){
            this.getCurrencyInfoUSDandEUR(currenciesRateLast.valueArray);
        }
    }

    getNBRBCurrenciesCurrentDate(){
        this._applicationService.getNBRBCurrencies().subscribe(currency =>{
            if(currency){
                console.log(currency);
                this.getCurrencyInfoUSDandEUR(currency);
                const curToday:IDictionaryArray = {key: currency[0]?.Date, valueArray:currency}
                this._localService.setCurrenciesRateToday(curToday);
                this._localService.setCurrenciesRateLast(curToday);
            }
        },
        err => {
            console.log("Get currencies error today");
            console.error(err);
            const currenciesRateLast = this._localService.getCurrenciesRateLast();
            const currenciesRateLastDate = currenciesRateLast?.key;
            if(currenciesRateLastDate){
                this.alertService.warning("Get currencies error. Use last currencies rate for date: " + currenciesRateLastDate);
            }else{
                this.alertService.error("Get currencies error");
            }
            
            this.getLastRate();
        })
    }

    getNBRBCurrenciesOnDate(date): Observable<any[]> {
        return new Observable((obs) => { this._applicationService.getNBRBCurrencies(date).subscribe(currency =>{
                if(currency){
                    this._appStateService.currencyNBRB = currency;
                    obs.next(currency);
                    obs.complete();
                }
            },
            err => {
                console.log("Get currencies error");
                console.error(err);
                return;
            }) 
        })
    }

    loadCurrencyInfoOnDate(date, CurAbbreviation){
       const currencies =  this.getNBRBCurrenciesOnDate(date);
       if(currencies != undefined){
           return this.getCurrencyInfoByAbbreviation(currencies, CurAbbreviation);
       }
       return;
    }

    getCurrencyInfoUSDandEUR(NBRBCurrencies){
        NBRBCurrencies.forEach(item =>{
            if(item){
                if(item?.Cur_Abbreviation == this.Cur_AbbreviationUSD){
                    this._appStateService.Cur_OfficialRate_USD = item.Cur_OfficialRate;
                }
                if(item?.Cur_Abbreviation == this.Cur_AbbreviationEUR){
                    this._appStateService.Cur_OfficialRate_EUR = item.Cur_OfficialRate;
                }
               return;
            }
        })
    }

    
    getCurrencyInfoByAbbreviation(NBRBCurrencies, Cur_Abbreviation:string){
        return  NBRBCurrencies.filter(item =>{
            if(item && item?.Cur_Abbreviation == Cur_Abbreviation){          
               return item;
            }
        })
    }

    

}