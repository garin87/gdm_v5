import { Injectable } from "@angular/core";
import { from, Observable, of } from "rxjs";
import { IMetadataProperty, ISelectableItem } from "../objects/common";
import { ApplicationService } from "./application.service";
import { AppStateService } from "./appState.service";
import { MetadataService } from "./metadata.service";


@Injectable()
export class DataValueService {

        constructor(private _applicationService: ApplicationService,
                private _appStateService: AppStateService,
                private _metadataService: MetadataService) {}
    

        public getSelectableItems(property: IMetadataProperty, instance: any):Observable<ISelectableItem[]> {
            let providerName = property.provider;
    
            if( providerName ){
                let provider: (p: IMetadataProperty, f: (v: ISelectableItem[]) => void, instance?: any) => void = this[providerName];
                if ( typeof provider == "function" ) {
                    let pr = provider.call(this, property, instance);
                    if(pr instanceof Promise)
                        return from(pr);
                    else
                        return pr;
                }
            }
    
            // error
            console.error("No provider for " + property.name);
            let empty : ISelectableItem[] = [];
            return from([empty]);
        }

        public loadProductParameters():Observable<ISelectableItem[]>{
           console.log("--------------  -------------- loadProductParameters");
           let listProductNames : ISelectableItem[] = [];
           this._applicationService.getProductTypes().subscribe( data =>{
                data.forEach(element => {
                listProductNames.push(
                    {
                        "name": "Product name",
                        "value": element
                    },
                  );

               });
              
           })
            return of(listProductNames);
        }
        
        public loadWarehousesNames():Observable<ISelectableItem[]>{
            console.log("--------------  -------------- loadWarehousesNames");
            let listProductNames : ISelectableItem[] = [];
            this._applicationService.GetNamesWareHouses().subscribe( data =>{
                 data.forEach(element => {
                 listProductNames.push(
                     {
                         "name": "WareHouse name",
                         "value": element
                     },
                   );
                });
               
            })
            return of(listProductNames);
        }

        public loadNamesCurrencies():Observable<ISelectableItem[]>{
            console.log("--------------  -------------- loadNamesCurrencies");
            let listProductNames : ISelectableItem[] = [];
            this._applicationService.getNamesCurrencies().subscribe( data =>{
                 data.forEach(element => {
                 listProductNames.push(
                     {
                         "name": "Currency name",
                         "value": element
                     },
                   );
                });
               
            })
            return of(listProductNames);
        }

        public loadCompanies():Observable<ISelectableItem[]>{
            console.log("--------------  -------------- loadCompanies");
            let listCompanies : ISelectableItem[] = [];
            this._applicationService.getCompaniesNames().subscribe( data =>{
                 data.forEach(element => {
                    listCompanies.push(
                     {
                         "name": "Company",
                         "value": element
                     },
                   );
                });
               
            })
            return of(listCompanies);
        }

        public loadOrderProductNames():Observable<ISelectableItem[]>{
            let listCompanies : ISelectableItem[] = [];
            this._applicationService.getNamesProduct().subscribe( data =>{
                 data.forEach(element => {
                    listCompanies.push(
                     {
                         "name": "ProductName",
                         "value": element
                     },
                   );
                });
               
            })
            return of(listCompanies);
        }

        public loadProductManufacturers():Observable<ISelectableItem[]>{
            console.log("--------------  -------------- loadProductManufacturers");
            let listCompanies : ISelectableItem[] = [];
            this._applicationService.getProductManufacturers().subscribe( data =>{
                 data.forEach(element => {
                    listCompanies.push(
                     {
                         "name": "Manufacturer",
                         "value": element
                     },
                   );
                });
               
            })
            return of(listCompanies);
        }

        public loadProductManufacturersByProductName():Observable<ISelectableItem[]>{
            console.log("--------------  -------------- loadProductManufacturersByProductName");
            let listCompanies : ISelectableItem[] = [];
            const instanceName = this._appStateService.selectedInstancePanel;
            this._applicationService.loadProductManufacturersByProductName(instanceName)
                .subscribe( data =>{
                 data.forEach(element => {
                    listCompanies.push(
                     {
                         "name": "Manufacturer",
                         "value": element
                     },
                   );
                });
               
            })
            return of(listCompanies);
        }

        public loadNamesWareHouses():Observable<ISelectableItem[]>{
            console.log("--------------  -------------- loadNamesWareHouses");
            let listCompanies : ISelectableItem[] = [];
            this._applicationService.GetNamesWareHouses().subscribe( data =>{
                 data.forEach(element => {
                    listCompanies.push(
                     {
                         "name": "WareHouse",
                         "value": element
                     },
                   );
                });
               
            })
            return of(listCompanies);
        }

        public loadInstancesParameter(property):Observable<ISelectableItem[]>{
           let option = property;
           console.log("---------------------------- loadInstancesParameter");
           console.log(option);

           const instanceName = this._appStateService.selectedInstancePanel;
           const paramertName = property.name;
           const isParameter = property?.isParameter;
           console.log(paramertName);
           console.log(instanceName);
           let listProductNames : ISelectableItem[] = [
            {
                "name": "notSet",
                "value": "---------Not set---------"
            }
           ];
           this._applicationService.getInstancesParameter(instanceName, paramertName, isParameter)
            .subscribe( data =>{
                data.forEach(element => {
                    listProductNames.push(
                        {
                            "name": element.name,
                            "value": element.value
                        },
                    );
                });

                console.log("----------- data ==================== listProductNames");
                console.log(listProductNames);
            })
            return of(listProductNames);
        }

        public loadInstancesParameterProduct(property):Observable<ISelectableItem[]>{
            let option = property;
            console.log("---------------------------- loadInstancesParameter");
            console.log(option);
 
            const instanceName = property.category;
            const paramertName = property.name;
            const isParameter = property?.isParameter;
            console.log(paramertName);
            console.log(instanceName);
            let listProductNames : ISelectableItem[] = [
             {
                 "name": "notSet",
                 "value": "---------Not set---------"
             }
            ];
            this._applicationService.getInstancesParameter(instanceName, paramertName, isParameter)
             .subscribe( data =>{
                 data.forEach(element => {
                     listProductNames.push(
                         {
                             "name": element.name,
                             "value": element.value
                         },
                     );
                 });
 
                 console.log("----------- data ==================== listProductNames");
                 console.log(listProductNames);
             })
             return of(listProductNames);
         }
}

export class SelectableItem implements ISelectableItem {
    constructor( public name:string, public value:any, public type :string = "",
                 public childCount :number = 0 ){}

}