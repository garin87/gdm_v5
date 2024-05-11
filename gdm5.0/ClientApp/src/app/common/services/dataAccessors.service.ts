import { Injectable } from "@angular/core";
import { Subject } from "rxjs";
import { gridParameter, IDependentProperties, IMetadataProperty, IParameter, MetadataProperty, valueUpdatedData } from "../objects/common";
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

          let listParameters = [];
          this._applicationService.getProductParameters(value).subscribe((data:IParameter[] | any[]) =>{
             if(typeof data == "object" && data.length > 0){
                 data.forEach((item:IParameter) => {
                    const p = new MetadataProperty(item.value, item?.nameType ?? "string", undefined, 
                    item.id, item.value, undefined,undefined,undefined,
                    undefined,undefined, item.priority,true, item.priority, item.isRequired,true,false,true,true,false, propertyContext.category);
                    listParameters.push(p);//true,10,false,true
                 })
             }

             listParameters.push(
                 new MetadataProperty("TestParameter","string","",null, "Parameter","parameter",
                  undefined,undefined,undefined,
                  undefined,-2,true,0,false,false,false,true))

             delete this._FormEditorService.instanceData.parameters;    
             this._FormEditorService.listParameters = {};
             const dependentProperties: IDependentProperties = {
                metadataTypeName : "",
                properties : listParameters
             }

             this._FormEditorService.dependentProperties.next(dependentProperties);
          })
    })


    public loadDependParameters = new PropertyAccessor( undefined, 
        (value:any, propertyContext:any) =>{
          let listParameters = [];
          this._appStateService.selectedInstancePanel = value;
          this._applicationService.getProductParameters(value).subscribe((data:IParameter[] | any[]) =>{
             if(typeof data == "object" && data.length > 0){
                 data.forEach((item:IParameter) => {
                    const p = new MetadataProperty(item.value, item?.nameType ?? "string", undefined, 
                    item.id, item.value, undefined,undefined,undefined,
                    undefined,undefined,item.priority,false, item.priority, item.isRequired,false,false,false,true,false, propertyContext.category);
                    listParameters.push(p);
                 })
             }

             delete this._FormEditorService.instanceData.parameters;    
             this._FormEditorService.listParameters = {};
             const dependentProperties: IDependentProperties = {
                metadataTypeName : "",
                properties : listParameters
             }
             this._FormEditorService.dependentProperties.next(dependentProperties);
             
          })
    })

    public  createParametersAsPickList = new PropertyAccessor( undefined, 
        (value:any, propertyContext:any) => {
        this._FormEditorService.resetValueUpdated();
        let listParameters = [];
        let selectedProduct = value;
        this._appStateService.selectedProductName = selectedProduct;

        const metaDataTypeName = propertyContext.category;
               
        this._appStateService.refreshAddProductSection.next(true);
        this._applicationService.getProductParameters2(selectedProduct)
        .subscribe((data:IParameter[] | any[]) =>{
           if(typeof data == "object" && data.length > 0){
               data.forEach((item:IParameter) => {
                   const p = new MetadataProperty(item.value, item?.nameType ?? "string", undefined, 
                   item.id, item.value,"picklist","picklist","loadInstancesParameterProduct",undefined,
                   undefined,item.priority,false,item.priority,true,false,false,false,true,false, selectedProduct);
                   listParameters.push(p);
               })

              
               const dateofreceipt = this._FormEditorService.instanceData["dateofreceipt"];
               const currencyname =  this._FormEditorService.instanceData["currencyname"];
               const name = { name : "name", navPriority : 1, type : "string", value: selectedProduct};
               this._FormEditorService.instanceData = {};  
               this._FormEditorService.instanceData["dateofreceipt"] = dateofreceipt;
               this._FormEditorService.instanceData["currencyname"] = currencyname;
               this._FormEditorService.instanceData["name"] = name;
               this._appStateService.selectedInstancePanel = selectedProduct;
                if(this._FormEditorService.dependentProperties.observers.length == 0){
                   this._FormEditorService.dependentProperties = new Subject<any>();
                }

                this._FormEditorService.listParameters = {};
                const dependentProperties: IDependentProperties = {
                    metadataTypeName : "",
                    properties : listParameters
                }
                this._FormEditorService.dependentProperties.next(dependentProperties);
           }
           
       })

    })

    public  createJustParametersAsPickList = new PropertyAccessor( undefined, 
        (value:any, propertyContext:any) => {

        this._FormEditorService.resetValueUpdated();
        let listParameters = [];
        let selectedProduct = value;
        this._appStateService.selectedProductName = selectedProduct;

        const metaDataTypeName = propertyContext.category;
               
        //this._appStateService.refreshAddProductSection.next(true);
        this._applicationService.getProductParameters2(selectedProduct)
        .subscribe((data:IParameter[] | any[]) =>{
           if(typeof data == "object" && data.length > 0){
               data.forEach((item:IParameter) => {
                   if(item.value.toLowerCase() !== "номер"){
                    const p = new MetadataProperty(item.value, item?.nameType ?? "string", undefined, 
                    item.id, item.value,"picklist","picklist","loadInstancesParameterProduct",undefined,
                    undefined,item.priority,false,item.priority,false,false,false,false,true,false, selectedProduct);
                    listParameters.push(p);
                   }
                   
               })

               const name = { name : "name", navPriority : 1, type : "string", value: selectedProduct};
               this._FormEditorService.instanceData = {};  

               this._FormEditorService.instanceData["name"] = name;
               this._appStateService.selectedInstancePanel = selectedProduct;
                if(this._FormEditorService.dependentProperties.observers.length == 0){
                   this._FormEditorService.dependentProperties = new Subject<any>();
                }

                this._FormEditorService.listParameters = {};
                const dependentProperties: IDependentProperties = {
                    metadataTypeName : "",
                    properties : listParameters
                }
                this._FormEditorService.dependentProperties.next(dependentProperties);
           }
           
       })

    })


    public  createParametersAsSelect = new PropertyAccessor( undefined, 
        (value:any, propertyContext:any) => {
        this._FormEditorService.resetValueUpdated();
        let listParameters = [];
        let selectedProduct = value;
        this._applicationService.getProductParameters2(selectedProduct)
        .subscribe((data:IParameter[] | any[]) =>{
           if(typeof data == "object" && data.length > 0){
               data.forEach((item:IParameter) => {

                   if(item?.value && (item.value.toLocaleLowerCase() == "диаметр" || 
                   item.value.toLocaleLowerCase() == "размер")){
                    const p = new MetadataProperty(item.value, item?.nameType ?? "string", undefined, 
                    item.id, item.value,"picklist","picklist","loadInstancesParameterProduct", undefined,
                    "disableFilterDimension", item.priority, false, item.priority, item.isRequired, false,
                    false, false, true, false, selectedProduct);
                    listParameters.push(p);
                   }else{
                    const p = new MetadataProperty(item.value, item?.nameType ?? "string", undefined, 
                    item.id, item.value,"picklist","picklist","loadInstancesParameterProduct",undefined,
                    undefined, item.priority,false,item.priority,item.isRequired,false,false,false,true,false, selectedProduct);
                    listParameters.push(p);
                   }

                   if(item?.value && (item.value.toLocaleLowerCase() == "диаметр" || 
                                      item.value.toLocaleLowerCase() == "размер")){
                    let p = new MetadataProperty("FilterStartDimension", item?.nameType ?? "string", undefined, 
                    item.id, "FilterStartDimension", "picklist", "picklist", "loadInstancesPP", undefined, 
                    "disableDimensionField", item.priority, false, item.priority, item.isRequired, false, false, false, true, false,
                    selectedProduct, item.value);
                    listParameters.push(p);
                    p = new MetadataProperty("FilterEndDimension", item?.nameType ?? "string", undefined, 
                    item.id, "FilterEndDimension", "picklist", "picklist", "loadInstancesPP", undefined,
                    "disableDimensionField",item.priority,false,item.priority,item.isRequired,false,false,false,true,false, 
                    selectedProduct, item.value);
                    listParameters.push(p);
                   }
               })
            
               if(this._FormEditorService.dependentProperties.observers.length == 0){
                   this._FormEditorService.dependentProperties = new Subject<any>();
                }

               this._FormEditorService.listParameters = {};
               const dependentProperties: IDependentProperties = {
                metadataTypeName : "OptionOfPriceListProduct",
                properties : listParameters
               }

               propertyContext.dependentProperties = dependentProperties;
               this._FormEditorService.dependentProperties.next(dependentProperties);
               
           }
           
       })
    })

    public loadDependParametersAsPickList = new PropertyAccessor( undefined, 
        (value:any, propertyContext:any) => {

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

             delete this._FormEditorService.instanceData.parameters;    
             this._FormEditorService.listParameters = {};
          
             const dependentProperties: IDependentProperties = {
                metadataTypeName : "",
                properties : listParameters
             }

             this._FormEditorService.dependentProperties.next(dependentProperties);
             
          })
    })

    public loadDependParametersForDialog = new PropertyAccessor( undefined, 
        (value:any, propertyContext:any) =>{

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

             delete this._FormEditorService.instanceData.parameters;    
             this._FormEditorService.listParameters = {};
             this._FormEditorService.dependentPropertiesforDialog.next(listParameters);
             
          })
    })

    public getGridDataByParameter = new PropertyAccessor( undefined, 
        (value:any, propertyContext:any, property:IMetadataProperty) =>{
          const option:gridParameter = {
            isParameter: property.isParameter,
            value: value,
            name: property.name,
            priority: property?.navPriority,
          }
          this._appStateService.isActiveRightActionPanel.next(false);
         // this._appStateService.initRightActionPanel = false;
          this._appStateService.changedGridOption.next(option);
        
     
    })

    public  setValueTotalPrice = new PropertyAccessor( undefined, 
        (value:any, propertyContext:any) => {

        if(value || value == ""){
            // const ndsValue = (value * 1.2).toFixed(2); // 20%
            // this._FormEditorService.listCreatedField.forEach( item =>{
            //     if(item.propertyName == "totalprice"){
            //         setTimeout(()=>{
            //             item.htmlRef.value = ndsValue;
            //         }, 500)
                 
            //     }
            // })
            // let d = new valueUpdatedData("totalprice", parseFloat(ndsValue), "double");
            // this._FormEditorService.valueUpdated.next(d);
        }

    })

    public  createPriceNDS = new PropertyAccessor( undefined, 
        (value:any, propertyContext:any) => {

        if(value || value == ""){
            const ndsValue = (value * 1.2).toFixed(2); // 20%
            let stdCost = 0;
            this._FormEditorService.listCreatedField.forEach( item =>{
                if(item.propertyName == "standartcost"){
                   stdCost = item.htmlRef.value;
                }
                if(item.propertyName == "totalpricends"){
                   item.htmlRef.value = ndsValue;
                }
                if(item.propertyName == "markup"){
                   if(stdCost){
                     item.htmlRef.value = (value - stdCost).toFixed(2);
                   }
                }
            })
            let d = new valueUpdatedData("totalpricends", ndsValue, "double");
            this._FormEditorService.valueUpdated.next(d);
        }

    })


    public  disablePrice = new PropertyAccessor( undefined, 
        (value:any, propertyContext:any) => {

        if(value){
            this._FormEditorService.listCreatedField.forEach( item =>{
                if(item.propertyName.toLowerCase() == "Price".toLowerCase()){
                   item.htmlRef.disabled = true;
                }
            })
        }

        if(value == ""){
            this._FormEditorService.listCreatedField.forEach( item =>{
                if(item.propertyName.toLowerCase() == "Price".toLowerCase()){
                    item.htmlRef.disabled = false;
                }
            })
        }
    })

    public  disablePercent = new PropertyAccessor( undefined, 
        (value:any, propertyContext:any) => {

        if(value){
            this._FormEditorService.listCreatedField.forEach( item =>{
                if(item.propertyName.toLowerCase() == "PercentOfMarkup".toLowerCase()){
                   item.htmlRef.disabled = true;
                }
            })
        }

        if(value == ""){
            this._FormEditorService.listCreatedField.forEach( item =>{
                if(item.propertyName.toLowerCase() == "PercentOfMarkup".toLowerCase()){
                   item.htmlRef.disabled = false;
                }
            })
        }
    })

    
    public  disableDimensionField = new PropertyAccessor( undefined, 
        (value:any, propertyContext:any) => {

        if(value){
            this._FormEditorService.listCreatedField.forEach( item =>{
                if(item.propertyName.toLowerCase() == "диаметр" || item.propertyName.toLowerCase() == "размер"){
                   item.htmlRef.nativeElement.disabled = true;
                }
            })
        }

        if(value == ""){
            this._FormEditorService.listCreatedField.forEach( item =>{
                if(item.propertyName.toLowerCase() == "диаметр" || item.propertyName.toLowerCase() == "размер"){
                    item.htmlRef.nativeElement.disabled = false;
                }
            })
        }

    })

    public  disableFilterDimension = new PropertyAccessor( undefined, 
        (value:any, propertyContext:any) => {

        if(value || value == ""){
            this._FormEditorService.listCreatedField.forEach( item =>{
                if(item.propertyName.toLowerCase() == "FilterStartDimension".toLowerCase()){
                   item.htmlRef.nativeElement.disabled = true;
                }
                if(item.propertyName.toLowerCase() == "FilterEndDimension".toLowerCase()){
                   item.htmlRef.nativeElement.disabled = true;
                }
            })
        }

        if(value == ""){
            this._FormEditorService.listCreatedField.forEach( item =>{
                if(item.propertyName.toLowerCase() == "FilterStartDimension".toLowerCase()){
                    item.htmlRef.nativeElement.disabled = false;
                }
                if(item.propertyName.toLowerCase() == "FilterEndDimension".toLowerCase()){
                    item.htmlRef.nativeElement.disabled = false;
                }
            })
        }

    })
}

export class PropertyAccessor {
    constructor( public getter:(value:any,propertyContext:any, property?:IMetadataProperty, properties?:IMetadataProperty[])=> any,
                 public setter?:(value:any, propertyContext:any, property?:IMetadataProperty, properties?:IMetadataProperty[] ) => void) {}
}