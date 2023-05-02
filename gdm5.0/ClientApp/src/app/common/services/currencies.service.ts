import { Injectable } from "@angular/core";
import { ApplicationService } from "./application.service";
import { AppStateService } from "./appState.service";
import { AlertService } from "src/app/alert/alert.service";
import { Observable } from "rxjs";


@Injectable()
export class CurrenciesService {
    
    private alltypes:any;
    get metadataTypes():any { return this.alltypes};

    public loaded:boolean = false;
    private Cur_AbbreviationEUR = "EUR";
    private Cur_AbbreviationUSD = "USD";
    constructor(private _applicationService: ApplicationService,
                private _appStateService: AppStateService,
                private alertService:AlertService) {

       this.getNBRBCurrenciesCurrentDate()
    }
    
    getNBRBCurrenciesCurrentDate(){
        this._applicationService.getNBRBCurrencies().subscribe(currency =>{
            if(currency){
                console.log(currency);
                this.getCurrencyInfoUSDandEUR(currency);
            }
        },
        err => {
            console.log("Get currencies error");
            console.error(err);
            this.alertService.error("Get currencies error");
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
                    this._appStateService.Cur_OfficialRate_USD = item.Cur_OfficialRate
                }
                if(item?.Cur_Abbreviation == this.Cur_AbbreviationEUR){
                    this._appStateService.Cur_OfficialRate_EUR = item.Cur_OfficialRate
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