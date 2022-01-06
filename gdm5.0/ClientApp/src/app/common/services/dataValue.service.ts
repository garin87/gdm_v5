import { Injectable } from "@angular/core";
import { from, Observable, of } from "rxjs";
import { IMetadataProperty, ISelectableItem } from "../objects/common";
import { ApplicationService } from "./application.service";
import { MetadataService } from "./metadata.service";


@Injectable()
export class DataValueService {

    constructor(private _applicationService: ApplicationService,
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

}

export class SelectableItem implements ISelectableItem {
    constructor( public name:string, public value:any, public type :string = "",
                 public childCount :number = 0 ){}

}