import { Injectable } from '@angular/core';
import { ISortOption, IgetProductTypeInstancesRequest, ProductTypeInstancesRequest, gridParameter } from 'src/app/common/objects/common';

@Injectable({
  providedIn: 'root'
})
export class GridService {
  
  constructor() { }


  private sortStrategies = {
    default: {
      name: '',
      direction: '',
      isParameter: false
    },
    quantityAsc: {
      name: 'quantity',
      direction: 'asc',
      isParameter: false
    }
  };

  private specialProducts = ["Шток хромированный", "Труба хонингованная"];

  private isSpecialProduct(product: string) {
    return this.specialProducts.includes(product);
  }
  getCustomParameter(parameterName): IParameter | undefined{
    if(this.customParameters) return this.customParameters.find((item: IParameter) => item.value === parameterName);
    return;
  };

  private handleTileFilterClick(parameterVal: IParameterSelectionValue) {
    const parameterOption: IOptionParameterValues = this._customParameterService.createNextParameterOptions(parameterVal, this.customParameters);
  
    if (!parameterOption) {
      this.handleMissingParameterOption(parameterVal);
      return;
    }
  
    const selectedParameter = this.getCustomParameter(parameterVal?.name);
  
    if (!selectedParameter) {
      this.dataTile = undefined;
      this.priority = undefined;
      return;
    }
  
    const filter = this.createFilterParameter(parameterVal, selectedParameter.priority);
    const option = this.createGridParameter(option, filter.Priority);
  
    this.updateFilterParameters(filter, option);
    this.updatePriorityAndLoadData(parameterOption);
  
    this._customParameterService.loadCustomParameterValues(parameterOption).subscribe((data: IOptionParameterValues) => {
      this.handleCustomParameterValuesLoad(data);
    });
  }
  
  private handleMissingParameterOption(parameterVal: IParameterSelectionValue) {
    const selectedParameter = this.getCustomParameter(parameterVal?.name);
  
    if (selectedParameter) {
      const filter = this.createFilterParameter(parameterVal, selectedParameter.priority);
      const option = this.createGridParameter(option, filter.Priority);
  
      this.updateFilterParameters(filter, option);
      this.updatePriorityAndLoadData(parameterOption);
    }
  }
  
  private createFilterParameter(parameterVal: IParameterSelectionValue, priority: number): FilterParameters {
    return {
      ParameterName: parameterVal.name,
      ParameterValue: parameterVal.value,
      IsParameter: true,
      Priority: priority,
    };
  }
  
  private createGridParameter(parameterVal: IParameterSelectionValue, priority: number): gridParameter {
    return {
      isParameter: true,
      value: parameterVal.value,
      name: parameterVal.name,
      priority: priority,
    };
  }
  
  private updateFilterParameters(filter: FilterParameters, option: gridParameter) {
    const listOptionFilter = this._appStateService.filter_TileFilterParameters.filter(item => item.Priority >= filter.Priority);
    this._appStateService.filter_TileFilterParameters = listOptionFilter.filter(item => item.ParameterName !== filter.ParameterName);
    this._appStateService.filter_TileFilterParameters.push(filter);
  
    this.getProductTypeInstancesRequest.Filter.Parameters = this.getProductTypeInstancesRequest.Filter.Parameters.filter(item => {
      return item.Priority >= option.priority;
    });
  
    this._appStateService.changedGridOption.next(option);
  }
  
  private updatePriorityAndLoadData(parameterOption: IOptionParameterValues) {
    this.priority = parameterOption?.Priority;
  }
  
  private handleCustomParameterValuesLoad(data: IOptionParameterValues) {
    if (data && data?.ParameterValues.length > 0) {
      this.dataTile = data?.ParameterValues;
      this._appStateService.filter_initCreateFilterTileComponent.next(data);
    }
  }

  private refreshGridData() {
    // Обновление данных сетки
    // ...
  }

  private getCustomParameter(parameterName: string): IParameter | undefined {
    return this.customParameters.find(item => item.value === parameterName);
  }
  createFilterProductParameters(getProductTypeInstancesRequest:IgetProductTypeInstancesRequest,  nameProduct: string, pageNumber: number, pageSize: number,
    sortOptions: ISortOption = null, filter: gridParameter = null ): IgetProductTypeInstancesRequest {
    
    getProductTypeInstancesRequest.NameProductType = nameProduct;
    getProductTypeInstancesRequest.PageFilter.pageNumber = pageNumber;
    getProductTypeInstancesRequest.PageFilter.pageSize = pageSize;
    if(sortOptions){
      getProductTypeInstancesRequest.SortOption = {
        name: sortOptions?.name.trim(),
        direction: sortOptions?.direction.trim(),
        isParameter: sortOptions?.isParameter
      };
    }

    if (filter?.isParameter) {
      let isContainParameter: boolean = false;
      for (const param of getProductTypeInstancesRequest.Filter.Parameters) {
        if (param.ParameterName === filter.name) {
          param.Value = filter.value;
          isContainParameter = true;
          break;
        }
      }

      if (!isContainParameter) {
        getProductTypeInstancesRequest.Filter.Parameters.push({
          Value: filter.value,
          ParameterName: filter.name,
          ProductId: null,
          ParameterId: null,
          Priority: filter.priority
        });
      }

      if (!this.isSpecialProduct(nameProduct)) {
        getProductTypeInstancesRequest.SortOption = this.sortStrategies['quantityAsc'];
      }
    } 

    if(filter && !filter?.isParameter) 
       getProductTypeInstancesRequest.Filter[filter.name] = filter.value;
    

    if (!this.isSpecialProduct(nameProduct)) 
       getProductTypeInstancesRequest.SortOption = this.sortStrategies['quantityAsc'];
    

    return getProductTypeInstancesRequest;
  }
  
}
