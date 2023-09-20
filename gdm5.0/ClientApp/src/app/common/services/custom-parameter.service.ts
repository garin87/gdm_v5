import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
import { IOptionParameterValues, IParameter, IParameterSelectionValue, IParameterValues, ISelectableItem } from '../objects/common';
import { ApplicationService } from './application.service';
import { CommonUtil } from '../utils/common-utils';

@Injectable({
  providedIn: 'root'
})
export class CustomParameterService {
  constructor(private _applicationService: ApplicationService) {}


  public loadCustomParameterValuesByDefault(selectedProduct: string): Observable<IOptionParameterValues | undefined> {
    return this.loadCustomParameters(selectedProduct).pipe(
      switchMap((parameters:IParameter[]) => {
        if (parameters && parameters.length > 0) {
          const parameterOption:IOptionParameterValues = this.createParameterOption(selectedProduct, parameters);
          return this.loadCustomParameterValues(parameterOption);
        } else {
          return of(undefined);
        }
      })
    );
  }


  public loadCustomParameters(selectedProduct: string): Observable<IParameter[]> {
    if(!selectedProduct) return;  
    return this._applicationService.getProductParameters2(selectedProduct)
          .pipe(
              map(data => {
                if (Array.isArray(data) && data.length > 0) {
                  return CommonUtil.sortByPriority(data);
                }
                return [];
              })
      );
  }

  public loadCustomParameterValues(parametersOp: IOptionParameterValues): Observable<IOptionParameterValues | undefined>  {
    if(!parametersOp) return;
    return this._applicationService.getInstancesParameter(parametersOp)
      .pipe(
        map(data => {
            if (data && data.length > 0) {
                const parmValues = data.map(element => ({
                    name: element.name,
                    value: element.value
                }));

                const parameterValues:IOptionParameterValues = Object.assign(parametersOp);
                parameterValues.ParameterValues = parmValues;

                return parameterValues;
            }
            return undefined;
        })
    );
  }

  public createParameterOption(selectedProduct: string, parameters:IParameter[]): IOptionParameterValues{

    if (parameters && parameters.length > 0) {
      const sortedParam = parameters;
      if (sortedParam && sortedParam.length > 0) {

          const parametersOp:IOptionParameterValues = {
              NameType: selectedProduct,
              NameParameter: sortedParam[0].value,
              IsParameter: true,
              Priority: sortedParam[0].priority,
              FilterParameters: [],
              Parameters:  parameters,
              ParameterValues:[]
          };
          return parametersOp;
      }
    }
  }


  filterCustomParamaters(customParameters:IParameter[]):IParameter[]{
    if(!customParameters || customParameters.length == 0) return;
    const filterExceptionValues = ["номер"];
    return customParameters.filter(item => {
      const lowercaseValue = item?.value?.toLowerCase();
      return lowercaseValue && !filterExceptionValues.includes(lowercaseValue);
    });
  }

  createNextParameterOptions(parameterVal:IParameterSelectionValue, customParameters:IParameter[]):IOptionParameterValues | undefined{
    if(parameterVal){

      const parameterName = parameterVal?.name;
      if (!customParameters || customParameters.length == 0) return;

      const selectParameterIndexArr = customParameters.filter((item) => (item.value == parameterName)).map(m => customParameters.indexOf(m));
      const selectParameterNextIndex =  selectParameterIndexArr[0] + 1;
      const selectParameterName = customParameters[selectParameterNextIndex]?.value;
      const selectParameterPriority = customParameters[selectParameterNextIndex]?.priority;
      if(!selectParameterName){
         return undefined;
      };

      const parameterOption:IOptionParameterValues = {
          NameType: parameterVal.typeProductName,
          NameParameter: selectParameterName,
          IsParameter: true,
          Priority: selectParameterPriority,
          FilterParameters: [],
          Parameters:  [],
          ParameterValues:[]
      };

      return parameterOption;
    };
        
    return undefined;
  }
}