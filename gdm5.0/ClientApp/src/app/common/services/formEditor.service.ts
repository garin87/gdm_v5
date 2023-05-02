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
    valueUpdated = new BehaviorSubject<valueUpdatedData>(undefined); 
    instanceProperties:IMetadataProperty[];
    setProperties:any;
    instanceData:any;
    listParameters:any;

    
    constructor(private _metadataService: MetadataService,
        private _dataValueService: DataValueService,
        private _appStateService: AppStateService) {
        this.instanceData = {};
        this.listParameters = {};
        this.setProperties = {}
    }

    createListProperties(instanceName: string, customProps:any = [], isDependentProps:boolean = false):IMetadataProperty[]{
        console.log("createListProperties");
        console.log(this.instanceProperties);

        if(!this.instanceProperties || !this.setProperties.hasOwnProperty(instanceName)){
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
        console.log( this.instanceProperties);
        
        return  this.instanceProperties;
    }

    setPropertyValue(prop: any, value:any, valueType:string = 'string', 
    navPriority:number = 1, newName:string = "", isEditedName:boolean = false, isDeletedProp:boolean = false){ 
        const propertyName = prop[0].name.toLowerCase();
        if(propertyName){
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
                    isDeleted: isDeletedProp
                };
                this.instanceData['parameters'] = this.listParameters;
            }else{
                this.instanceData[propertyName] = {
                    name: propertyName,
                    value: value,
                    type: valueType,
                    navPriority: navPriority
                }
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
        this.dependentProperties = new Subject<any>();
        this.setProperties = {};
        this.valueUpdated = new BehaviorSubject<valueUpdatedData>(undefined);
        this.instanceData = {};
        this.listParameters = {};
    }

    resetValueUpdated():void{
     //   this.valueUpdated = new BehaviorSubject<valueUpdatedData>(undefined);
       // this.instanceData = {};
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
                return pv;
            }
        }
  
        if (p.defaultValue) {
            let dv = p.defaultValue;
            return dv;
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