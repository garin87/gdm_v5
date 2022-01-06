import { Injectable } from "@angular/core";
import { BehaviorSubject, Subject } from "rxjs";
import { IMetadataProperty, valueUpdatedData } from "../objects/common";
import { ApplicationService } from "./application.service";
import { DataAccessorsService } from "./dataAccessors.service";
import { DataValueService } from "./dataValue.service";
import { MetadataService } from "./metadata.service";


@Injectable()
export class FormEditorService {
    
    customProperties:Subject<any> = new Subject<any>();
    dependentProperties:Subject<any> = new Subject<any>();
    valueUpdated = new BehaviorSubject<valueUpdatedData>(undefined); 
    instanceProperties:IMetadataProperty[];
    setProperties:any;
    instanceData:any;
    listParameters:any;

    
    constructor(private _metadataService: MetadataService,
        private _dataValueService: DataValueService) {
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
                    navpriority: navPriority,
                    newName: newName,
                    isDeleted: isDeletedProp
                };
                this.instanceData['parameters'] = this.listParameters;
            }else{
                this.instanceData[propertyName] = {
                    name: propertyName,
                    value: value,
                    type: valueType,
                    navpriority: navPriority
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
        this.setProperties = {};
        this.valueUpdated = new BehaviorSubject<valueUpdatedData>(undefined);
        this.instanceData = {};
        this.listParameters = {};
    }

    private setValueProvider(property: IMetadataProperty):void{
        if(property.editor){
            if(property.editor == "selector"){
                property.selectableItems = this._dataValueService.getSelectableItems(property,{}); 
            } 
        }
    }
}