import { Injectable } from '@angular/core';
import { IDictionaryArray, ILocalState, IUserProfile } from '../objects/common';

@Injectable({
  providedIn: 'root'
})

export class LocalService {
  _localState: ILocalState;
  public isAdministrator: any;
  
  get isAdmin(){
    return this.getUserRole() === "Admin" ? true : false;
  };

  constructor() {
      this.getStateFromLocalStorage();
  }

  getCurrenciesRateToday(): IDictionaryArray {
     return this._localState.CurrenciesRateToday;
  }

  setCurrenciesRateToday(currenciesRateToday: any): boolean {
    this._localState.CurrenciesRateToday = currenciesRateToday;
    this.saveStateToLocalStorage();
    return true;
  }

  getCurrenciesRateLast(): IDictionaryArray {
    return this._localState.CurrenciesRateLast;
  }

  setCurrenciesRateLast(currenciesRateLast: any): boolean {
    this._localState.CurrenciesRateLast = currenciesRateLast;
    this.saveStateToLocalStorage();
    return true;
  }
  
  getUserProfile(): IUserProfile {
      return this._localState.UserProfile;
  }

  setUserProfile(userProfile: IUserProfile): boolean {
      this._localState.UserProfile = userProfile;
      this.saveStateToLocalStorage();
      return true;
  }

  getUserRole():string{
    return localStorage.getItem("userRole");
  }

  resetSettings(): void {
      this.initializeState();
      this.saveStateToLocalStorage();
  }


  public isLogged(): boolean {
      if (this._localState && this._localState.UserProfile && this._localState.UserProfile.SessionID) {
          return true;
      }

      return false;
  }


  private initializeState(): void {
      if (!this._localState)
          this._localState = <ILocalState>{};

      this._localState.MetaData = {};
      this._localState.CurrenciesRateToday = {};
      this._localState.CurrenciesRateLast = {};
  }

  private getStateFromLocalStorage(): void {
      let localStorageState = localStorage.getItem('GDM_LOCAL_SETTINGS');
      this._localState = <ILocalState>JSON.parse(localStorageState);

      if (!this._localState)
          this.initializeState();

      this.saveStateToLocalStorage();
  }

  private saveStateToLocalStorage(): void {
      localStorage.setItem("GDM_LOCAL_SETTINGS", JSON.stringify(this._localState));
  }

}
