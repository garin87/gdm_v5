import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from "@angular/core";
import { BehaviorSubject, Subject, Subscription, of } from "rxjs";
import { IDependentProperties, IMetadataProperty, valueUpdatedData } from "../common/objects/common";
import { DataAccessorsService } from "../common/services/dataAccessors.service";
import { FormEditorService } from "../common/services/formEditor.service";

@Component({
    selector: 'form-editor',
    templateUrl: './formEditor.component.html',
    styleUrls: ['./formEditor.component.css']
})

export class FormEditorComponent implements OnInit, OnDestroy {

    @Input("instanceName") metadataTypeName: string;
    @Input("selectedInstance") selectedInstance: string;
    //@Input("properties") properties: BehaviorSubject<IMetadataProperty[]>;
    properties: BehaviorSubject<IMetadataProperty[]> = new BehaviorSubject<IMetadataProperty[]>(undefined);
    valueUpdated: Subject<valueUpdatedData> = new Subject<valueUpdatedData>();
    private valueUpdatedSubscription$:Subscription;
    private customPropertiesSubscription$:Subscription;
    private dependentPropertiesSubscription$:Subscription;
    private dependentPropertiesforDialog$:Subscription;
   // public properties: Subject<IMetadataProperty[]>;
    

    lodedOptionalFilter:boolean = false;
    constructor( private _formEditorService: FormEditorService,
                 private _dataAccessorsService: DataAccessorsService){
                 
    }

    ngOnInit(){
       //.pipe(debounceTime(500))
       // this.valueUpdatedSubscription$ = this._formEditorService.valueUpdated.subscribe((el:valueUpdatedData)=>{
        this.valueUpdatedSubscription$ = this.valueUpdated.subscribe((el:valueUpdatedData)=>{ 
          if(el){
              let prop = this._formEditorService.instanceProperties
              .filter(p => p.name.toLowerCase() == el.propertyName.toLowerCase());

              if(el.category && this._formEditorService?.setProperties[el.category]){
                prop = this._formEditorService?.setProperties[el.category]
                .filter(p => p.name.toLowerCase() == el.propertyName.toLowerCase())  
              }
            
              if(prop[0]){
                let contextProp = {category: el.category}
                const accessor = prop[0]?.accessor;
                if(accessor){
                  this._dataAccessorsService.CallSetter(el.value, contextProp, prop[0]);
                }
                
                this._formEditorService.setPropertyValue(this.metadataTypeName, prop, el.value, el.ValueType,
                  el.navPriority, el.propertyNewName, el.isEditedName, el.isDeletedProp, el.isRequired);

              }
            }   
        });

        this.applyProps([]);
        this.customPropertiesSubscription$ = this._formEditorService.customProperties.subscribe( customProps =>{ 
                this.applyProps(customProps);
           }
        );

        this.dependentPropertiesSubscription$ = this._formEditorService.dependentProperties
        .subscribe( (dependentProps) => {   
              //  if(dependentProps.metadataTypeName){
              //     this.metadataTypeName = dependentProps.metadataTypeName
              //  }

               if(dependentProps.metadataTypeName == "" || this.metadataTypeName == dependentProps.metadataTypeName){
                 // this.metadataTypeName = dependentProps.metadataTypeName;
                  this.applyProps(dependentProps.properties, true);
                  if(this.metadataTypeName == "OptionalOfProduct"){
                     this.lodedOptionalFilter = true;
                  }
               }
              
        });

        this.dependentPropertiesforDialog$ = this._formEditorService.dependentPropertiesforDialog.subscribe( dependentProps => {
          this.applyProps(dependentProps, true);
          
          //this.instanceName = undefined;
          //this.instanceName == "OptionalOfProduct" && customProps[0].TypeView == undefined
        });
    };
    
   
    applyProps(customProps:any[], isDependentProps:boolean = false){
      if(!this.lodedOptionalFilter){
        if(this.metadataTypeName){
          let instanceProps = this._formEditorService.createListProperties(this.metadataTypeName, customProps, isDependentProps);
          instanceProps.forEach(p=> this._formEditorService.populateValue(p));
          instanceProps = this._formEditorService.sortProperties(instanceProps);
          this.properties.next(instanceProps);
        }
      }
    }

    ngOnDestroy(){
      this.valueUpdatedSubscription$.unsubscribe();
      this.customPropertiesSubscription$.unsubscribe();
      this.dependentPropertiesSubscription$.unsubscribe();
      this.dependentPropertiesforDialog$.unsubscribe();
    }
  
}