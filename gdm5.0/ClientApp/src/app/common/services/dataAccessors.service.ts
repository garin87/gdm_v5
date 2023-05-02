import { Injectable } from "@angular/core";
import { Subject } from "rxjs";
import { gridParameter, IMetadataProperty, IParameter, MetadataProperty } from "../objects/common";
import { ApplicationService } from "./application.service";
import { AppStateService } from "./appState.service";
import { FormEditorService } from "./formEditor.service";
import { MetadataService } from "./metadata.service";


@Injectable()
export class DataAccessorsService {

    constructor(private _applicationService: ApplicationService,
                private _metadataService: MetadataService,
                private _FormEditorService: FormEditorService,
                private _appStateService: AppStateService,
                ) {}
    
    public CallSetter(value:any, propertyContext:any, property?:IMetadataProperty, properties?:IMetadataProperty[]):boolean{
        let accessor: PropertyAccessor = this[property.accessor];
        if(accessor && typeof accessor.setter == "function"){
            accessor.setter(value, propertyContext, property);
            return true;
        }
        return false;
    }

    public CallGetter(value:any, propertyContext:any, property?:IMetadataProperty, properties?:IMetadataProperty[]):any{
        let accessor: PropertyAccessor = this[property.accessor];
        if(accessor && typeof accessor.getter == "function"){
            return accessor.getter(value, propertyContext,property);
        }
        return;
    }

    public loadParameterFields = new PropertyAccessor( undefined, 
        (value:any, propertyContext:any) =>{
          console.log(" ------- ------ - - -- - - --  init --- Accessor --- loadParametersField");
          console.log(value);
          console.log(propertyContext);
          let listParameters = [];
          this._applicationService.getProductParameters(value).subscribe((data:IParameter[] | any[]) =>{
             if(typeof data == "object" && data.length > 0){
                 data.forEach((item:IParameter) => {
                    const p = new MetadataProperty(item.value, "string", undefined, 
                    item.id, item.value, undefined,undefined,undefined,
                    undefined,undefined, item.priority,true,item.priority,false,true,false,true,true,false, propertyContext.category);
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
        (value:any, propertyContext:any) =>{
          console.log(" ------- ------ - - -- - - --  init --- Accessor --- loadDependParameters");
          console.log(value);
          console.log(propertyContext);
          let listParameters = [];
          this._appStateService.selectedInstancePanel = value;
          this._applicationService.getProductParameters(value).subscribe((data:IParameter[] | any[]) =>{
             if(typeof data == "object" && data.length > 0){
                 data.forEach((item:IParameter) => {
                    const p = new MetadataProperty(item.value, "string", undefined, 
                    item.id, item.value, undefined,undefined,undefined,
                    undefined,undefined,item.priority,false,item.priority,false,false,false,false,true,false, propertyContext.category);
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

    public  createParametersAsPickList = new PropertyAccessor( undefined, 
        (value:any, propertyContext:any) => {
        this._FormEditorService.resetValueUpdated();
        let listParameters = [];
        let selectedProduct = value;
        this._appStateService.selectedProductName = selectedProduct;
        let category = "OptionalOfProduct";
        // const name = this._FormEditorService.instanceData["name"];

        const metaDataTypeName = propertyContext.category;
        // let metaDataType = this._metadataService.getMetadataType(metaDataTypeName);
        //     metaDataType.forEach(item =>{
        //         if(item.name == "Name" || item.name == "name"){
        //              //   item.defaultValue = selectedProduct;
        //                 // return;
        //         }
        //     });
        //   this._metadataService.setMetadataType(metaDataTypeName, metaDataType)
               
        this._appStateService.refreshAddProductSection.next(true);
        this._applicationService.getProductParameters2(selectedProduct)
        .subscribe((data:IParameter[] | any[]) =>{
           if(typeof data == "object" && data.length > 0){
               data.forEach((item:IParameter) => {
                   const p = new MetadataProperty(item.value, "string", undefined, 
                   item.id, item.value,"picklist","picklist","loadInstancesParameterProduct",undefined,
                   undefined,item.priority,false,item.priority,false,false,false,false,true,false, selectedProduct);
                   listParameters.push(p);
               })
                   console.log("--------- ------ listParameters");
                   console.log(listParameters);
              
               const dateofreceipt = this._FormEditorService.instanceData["dateofreceipt"];
               const currencyname =  this._FormEditorService.instanceData["currencyname"];
               const name = { name : "name", navPriority : 1, type : "string", value: selectedProduct};
               this._FormEditorService.instanceData = {};  
               this._FormEditorService.instanceData["dateofreceipt"] = dateofreceipt;
               this._FormEditorService.instanceData["currencyname"] = currencyname;
               this._FormEditorService.instanceData["name"] = name;
               

               if(this._FormEditorService.dependentProperties.observers.length == 0){
                   this._FormEditorService.dependentProperties = new Subject<any>();
                }

               this._FormEditorService.listParameters = {};
               this._FormEditorService.dependentProperties.next(listParameters);
           }
           
       })
        // setTimeout(() => {             
          
        //  }, 0);
       

    })

    public loadDependParametersAsPickList = new PropertyAccessor( undefined, 
        (value:any, propertyContext:any) => {
          console.log("------------- Accessor --- loadDependParametersAaPickList");
          console.log(value);
          console.log(propertyContext);
          let listParameters = [];
          //this._appStateService.selectedInstancePanel = value;
          let productName = value;
          this._applicationService.getProductParameters(productName).subscribe((data:IParameter[] | any[]) =>{
             
            if(typeof data == "object" && data.length > 0){
                 data.forEach((item:IParameter) => {
                    const p = new MetadataProperty(item.value, "string", undefined, 
                    item.id, item.value, undefined,undefined,undefined,
                    undefined,undefined,item.priority,false,item.priority,false,false,false,false,true,false, propertyContext.category);
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

    public loadDependParametersForDialog = new PropertyAccessor( undefined, 
        (value:any, propertyContext:any) =>{
          console.log(" -------- Accessor --- loadDependParametersForDialog");
          console.log(value);
          console.log(propertyContext);
          let listParameters = [];
          this._applicationService.getProductParameters3(value).subscribe((data:IParameter[] | any[]) =>{
             if(typeof data == "object" && data.length > 0){
                 data.forEach((item:IParameter) => {
                    const p = new MetadataProperty(item.value, "string", undefined, 
                    item.id, item.value, undefined,undefined,undefined,
                    undefined,undefined,item.priority,false,item.priority,false,false,false,false,true,false, propertyContext.category);
                    listParameters.push(p);
                 })
             }
             console.log("--------- ------ listParameters");
             console.log(listParameters);
        
             delete this._FormEditorService.instanceData.parameters;    
             this._FormEditorService.listParameters = {};
             this._FormEditorService.dependentPropertiesforDialog.next(listParameters);
             
          })
    })

    public getGridDataByParameter = new PropertyAccessor( undefined, 
        (value:any, propertyContext:any, property:IMetadataProperty) =>{
          console.log(" ------- ------ - - -- - - --  init --- Accessor --- getGridDataByParameter");
          console.log(value);
          console.log(propertyContext);
          const option:gridParameter = {
            isParameter: property.isParameter,
            value: value,
            name: property.name
          }

          this._appStateService.changedGridOption.next(option);
     
    })


}

export class PropertyAccessor {
    constructor( public getter:(value:any,propertyContext:any, property?:IMetadataProperty, properties?:IMetadataProperty[])=> any,
                 public setter?:(value:any, propertyContext:any, property?:IMetadataProperty, properties?:IMetadataProperty[] ) => void) {}
}