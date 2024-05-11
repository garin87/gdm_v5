import { Injectable } from "@angular/core";
import { BehaviorSubject, Subject } from "rxjs";
import { IMetadataProperty, valueUpdatedData } from "../objects/common";
import { AppStateService } from "./appState.service";
import { DataValueService } from "./dataValue.service";
import { MetadataService } from "./metadata.service";
import _ from 'lodash';

@Injectable()
export class FormEditorService {
    
    customProperties:Subject<any> = new Subject<any>();
    dependentProperties:Subject<any> = new Subject<any>();
    dependentPropertiesforDialog:Subject<any> = new Subject<any>();
    valueUpdated = new Subject<valueUpdatedData>(); 
    instanceProperties:IMetadataProperty[] = [];
    setProperties:any;
    instanceData:any;
    instanceDataList:any;
    listParameters:any;
    listCreatedField:any[] = [];
    standartCost:any;
    
    constructor(private _metadataService: MetadataService,
        private _dataValueService: DataValueService,
        private _appStateService: AppStateService) {
        this.instanceData = {};
        this.instanceDataList = {};
        this.listParameters = {};
        this.setProperties = {}
    }

    createListProperties(instanceName: string, customProps:any = [], isDependentProps:boolean = false):IMetadataProperty[]{

        if((!this.instanceProperties || !this.setProperties.hasOwnProperty(instanceName)) && !isDependentProps){
            this.instanceProperties = this._metadataService.metadataTypes[instanceName];
            this.setProperties[instanceName] = this.instanceProperties;
        }
        else if(isDependentProps){  
            this.instanceProperties = this._metadataService.metadataTypes[instanceName];
            this.instanceProperties = this.instanceProperties?.concat(customProps);
        }
        else{  
            this.instanceProperties = this.setProperties[instanceName];
            this.instanceProperties = this.instanceProperties?.concat(customProps);
        } 
        //this.instanceProperties = this.instanceProperties?.concat(customProps);
        this.instanceProperties.forEach(item => this.setValueProvider(item));
        this.setProperties[instanceName] = this.instanceProperties;
  
        return  this.instanceProperties;
    }

    setPropertyValue(parent:string, prop: any, value:any, valueType:string = 'string', 
        navPriority:number = 1, newName:string = "", 
        isEditedName:boolean = false, isDeletedProp:boolean = false,
        isRequired:boolean = false){ 
        const propertyName = prop[0].name.toLowerCase();
        if(propertyName){
            
            let parentType = this.instanceDataList[parent] ?? {};
            if(prop[0]?.isParameter){
                let propName = propertyName;
                if(isEditedName && prop[0]?.isNewProp){
                    delete this.listParameters[propName];
                    propName = newName;
                }
                this.listParameters[propName] = {
                    name: propName,
                    value: value,
                    type: valueType,
                    navPriority: navPriority,
                    newName: newName,
                    isDeleted: isDeletedProp,
                    Required: isRequired
                };
                this.instanceData['parameters'] = this.listParameters;
                parentType['parameters'] = this.listParameters;
            }else{
                this.instanceData[propertyName] = {
                    name: propertyName,
                    value: value,
                    type: valueType,
                    navPriority: navPriority
                }
                parentType[propertyName] = this.instanceData[propertyName];
                this.instanceDataList[parent] = parentType;
            }
        }

    }

    sortProperties(props){
        props = props.map(item => {
            if(item.order == null) item.order = 0;
            return item;
        });
        return props.sort(function (a, b) {
            if (a.order > b.order) {
              return -1;
            }
            if (a.order < b.order) {
              return 1;
            }
            return 0;
          });
    }

    isRequiredValue(id):boolean{
        if(!document.getElementById(id)) return;
        const controls = document.getElementById(id).querySelectorAll("[required]");

        // let test = document.getElementById("OrderProductcontrols-id").querySelectorAll("[required]"); 
      //  [aria-required='true'] "[required]"
        let isValid = true;
        controls.forEach((element:HTMLInputElement) => {
            if (!element.reportValidity()) {
                isValid = false;
                return false;
            }
            if(!element.value.trim()){
                isValid = false;
                return false;
            }
        });
       
        return isValid;
    }

    resetValueProperties():void{
        this.customProperties = new Subject<any>();
        this.setProperties = {};
        this.valueUpdated = new Subject<valueUpdatedData>();
        this.instanceData = {};
        this.listParameters = {};
    }

    resetValueUpdated():void{
    }

    disposeFormProperties(){
        this.customProperties = new Subject<any>();
        this.dependentProperties = new Subject<any>();
        this.valueUpdated = new Subject<valueUpdatedData>();
        this.listCreatedField = [];
        this.instanceData = {};
        this.listParameters = {};
    }
    
    populateValue(p: IMetadataProperty ) : void {
        let v = this.getPropertyValue(p);
        p.value = v;
    }
  
  
    private getPropertyValue (p: IMetadataProperty): any {

        if (this._appStateService.instanceOfProduct) {
            let name = p.name.toLowerCase();
            let propertyValues = this._appStateService.instanceOfProduct; //this.getPropertyValues();
            let key = _.findKey(propertyValues, (v, k) => k.toLowerCase() == name);  

            //needs to improve
            let parametrType;
            if(key === undefined && propertyValues?.parameters && p.isParameter) {
                parametrType = propertyValues?.parameters.filter(el=>{        
                    if(el?.name.toLowerCase() == name.toLowerCase()) return el;
                });
            }
            
            //needs to improve
            if ((key && (propertyValues[key] !== undefined )) || parametrType?.length > 0){
                let pv = propertyValues[key];
                if(pv === undefined && parametrType?.length > 0){
                    pv = parametrType[0]["value"]; //needs to improve
                }
                // if(typeof pv == "object" &&  _.isEmpty(pv)){
                //     if(p.editor && p.editor.directive === "enum"){
                //         pv = p.defaultValue;
                //     }
                //     else if(p.originalTypeName == "boolean"){
                //         pv = false;
                //     }
                // }

               // let standartCost = 0;
                if(key === "standartCost"){
                    pv = (pv * this._appStateService.Cur_OfficialRate_EUR).toFixed(2);
                    this.standartCost = pv;
                }
                
             

                return pv;
            }
        }
  
        if (p.defaultValue) {
            let dv = p.defaultValue;
            return dv;
        }

        if(p.name.toLowerCase() === "totalprice"){
            return (this.standartCost * 1.2).toFixed(2);
        }

        if(p.name.toLowerCase() === "totalpricends"){
            return ((this.standartCost * 1.2)*1.2).toFixed(2);
        }

        if(p.name.toLowerCase() === "markup"){
            return ((this.standartCost * 1.2) - this.standartCost).toFixed(2);
        }

        return null;
    }
  
    // private getPropertyValues(){
  
    // }

    private setValueProvider(property: IMetadataProperty):void{
        if(property.editor){
            if(property.editor == "selector" || property.editor == "picklist" ){
                property.selectableItems = this._dataValueService.getSelectableItems(property,{}); 
            } 
        }
    }
}