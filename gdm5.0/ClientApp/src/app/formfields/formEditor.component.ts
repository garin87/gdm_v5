import { ChangeDetectorRef, Component, Input, OnInit } from "@angular/core";
import { BehaviorSubject, of } from "rxjs";
import { IMetadataProperty, valueUpdatedData } from "../common/objects/common";
import { DataAccessorsService } from "../common/services/dataAccessors.service";
import { FormEditorService } from "../common/services/formEditor.service";

@Component({
    selector: 'form-editor',
    templateUrl: './formEditor.component.html',
    styleUrls: ['./formEditor.component.css']
})

export class FormEditorComponent implements OnInit {

    @Input("instanceName") instanceName: string;
    @Input("selectedInstance") selectedInstance: string;
    
    properties = new BehaviorSubject<IMetadataProperty[]>(undefined);
    
    lodedOptionalFilter:boolean = false;
    constructor( private _formEditorService: FormEditorService,
                 private _dataAccessorsService: DataAccessorsService){}

    ngOnInit(){
       //.pipe(debounceTime(500))
       this._formEditorService.valueUpdated.subscribe((el:valueUpdatedData)=>{
          if(el){
            console.log("-------- ------ --  valueUpdated.subscribe");
            console.log(el);
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
                
              this._formEditorService.setPropertyValue(prop, el.value, el.ValueType, el.navPriority,
                                      el.propertyNewName, el.isEditedName, el.isDeletedProp);
    
              console.log("---- this.instanceData")
              console.log(this._formEditorService.instanceData);
            }
          }   
        });


        this.applyProps([]);
        this._formEditorService.customProperties.subscribe( customProps =>{
                console.log("---- call customProperties.subscribe");  
                this.applyProps(customProps);
           }
        );

        this._formEditorService.dependentProperties.subscribe( dependentProps => {
               this.applyProps(dependentProps, true);
               
               if(this.instanceName == "OptionalOfProduct"){
                  this.lodedOptionalFilter = true;
               }

              // this.lodedOptionalFilter
               //this.instanceName = undefined;
        });

        this._formEditorService.dependentPropertiesforDialog.subscribe( dependentProps => {
          this.applyProps(dependentProps, true);
          
          //this.instanceName = undefined;
          //this.instanceName == "OptionalOfProduct" && customProps[0].TypeView == undefined
        });


        
    };
    
   
    applyProps(customProps:any[], isDependentProps:boolean = false){
      if(!this.lodedOptionalFilter){
        if(this.instanceName){
          let instanceProps = this._formEditorService.createListProperties(this.instanceName, customProps, isDependentProps);
          instanceProps.forEach(p=> this._formEditorService.populateValue(p));
          console.log(" ----------- instanceProps applyProps");
          instanceProps = this._formEditorService.sortProperties(instanceProps);
          this.properties.next(instanceProps);
        }
        
      }
    }
  
}