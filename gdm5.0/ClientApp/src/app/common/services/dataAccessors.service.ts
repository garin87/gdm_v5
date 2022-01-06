import { Injectable } from "@angular/core";
import { Subject } from "rxjs";
import { IMetadataProperty, IParameter, MetadataProperty } from "../objects/common";
import { ApplicationService } from "./application.service";
import { FormEditorService } from "./formEditor.service";
import { MetadataService } from "./metadata.service";


@Injectable()
export class DataAccessorsService {

    constructor(private _applicationService: ApplicationService,
                private _metadataService: MetadataService,
                private _FormEditorService: FormEditorService
                ) {}
    
    public CallSetter(value:any, propertyContext:any, property?:IMetadataProperty, properties?:IMetadataProperty[]):boolean{
        let accessor: PropertyAccessor = this[property.accessor];
        if(accessor && typeof accessor.setter == "function"){
            accessor.setter(value, propertyContext);
            return true;
        }
        return false;
    }

    public CallGetter(value:any, propertyContext:any, property?:IMetadataProperty, properties?:IMetadataProperty[]):any{
        let accessor: PropertyAccessor = this[property.accessor];
        if(accessor && typeof accessor.getter == "function"){
            return accessor.getter(value, propertyContext);
        }
        return;
    }

    public loadParameterFields = new PropertyAccessor( undefined, 
        (value:any, propertyContext:any, properties:IMetadataProperty[]) =>{
          console.log(" ------- ------ - - -- - - --  init --- Accessor --- loadParametersField");
          console.log(value);
          console.log(propertyContext);
          let listParameters = [];
          this._applicationService.getProductParameters(value).subscribe((data:IParameter[] | any[]) =>{
             if(typeof data == "object" && data.length > 0){
                 data.forEach((item:IParameter) => {
                    const p = new MetadataProperty(item.value, "string", undefined, 
                    item.id, item.value, undefined,undefined,undefined,
                    undefined,undefined,0,true,0,false,true,false,true,true,false);
                    listParameters.push(p);//true,10,false,true
                 })
             }
             console.log("--------- ------ listParameters");
             console.log(listParameters);
             listParameters.push(
                 new MetadataProperty("TestParameter","string","",null, "Parameter","parameter",
                  undefined,undefined,undefined,
                  undefined,-2,true,0,false,false,false,true))

             delete this._FormEditorService.instanceData.parameters;    
             this._FormEditorService.listParameters = {};
             this._FormEditorService.dependentProperties.next(listParameters);
             
          })
    })

    public loadDependParameters = new PropertyAccessor( undefined, 
        (value:any, propertyContext:any, properties:IMetadataProperty[]) =>{
          console.log(" ------- ------ - - -- - - --  init --- Accessor --- loadDependParameters");
          console.log(value);
          console.log(propertyContext);
          let listParameters = [];
          this._applicationService.getProductParameters(value).subscribe((data:IParameter[] | any[]) =>{
             if(typeof data == "object" && data.length > 0){
                 data.forEach((item:IParameter) => {
                    const p = new MetadataProperty(item.value, "string", undefined, 
                    item.id, item.value, undefined,undefined,undefined,
                    undefined,undefined,0,false,0,false,false,false,false,true,false);
                    listParameters.push(p);
                 })
             }
             console.log("--------- ------ listParameters");
             console.log(listParameters);
        
             delete this._FormEditorService.instanceData.parameters;    
             this._FormEditorService.listParameters = {};
             this._FormEditorService.dependentProperties.next(listParameters);
             
          })
    })

}

export class PropertyAccessor {
    constructor( public getter:(value:any,propertyContext:any, properties?:IMetadataProperty[])=> any,
                 public setter?:(value:any, propertyContext:any, properties?:IMetadataProperty[] ) => void) {}
}