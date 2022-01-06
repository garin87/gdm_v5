import { ChangeDetectorRef, Component, Input, IterableDiffers, OnInit } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { BehaviorSubject, from, of, Subject } from "rxjs";
import { async } from "rxjs/internal/scheduler/async";
import { debounceTime, filter, tap } from "rxjs/operators";
import { isMetaProperty } from "typescript";
import { IMetadataProperty, valueUpdatedData } from "../common/objects/common";
import { ApplicationService } from "../common/services/application.service";
import { DataAccessorsService } from "../common/services/dataAccessors.service";
import { FormEditorService } from "../common/services/formEditor.service";
import { MetadataService } from "../common/services/metadata.service";

@Component({
    selector: 'form-editor',
    templateUrl: './formEditor.component.html',
    styleUrls: ['./formEditor.component.css']
  })
export class FormEditorComponent implements OnInit {

    @Input("instanceName") instanceName: string;

    //valueUpdated = new BehaviorSubject<valueUpdatedData>(undefined); 
    // instanceData: any;
    properties = new BehaviorSubject<IMetadataProperty[]>(undefined);
    // listParameters:any;

    constructor( private _formEditorService: FormEditorService,
                 private _dataAccessorsService: DataAccessorsService,
                 private _chRef: ChangeDetectorRef ){}

    ngOnInit(){
       //.pipe(debounceTime(500))
       this._formEditorService.valueUpdated.subscribe((el:valueUpdatedData)=>{
          if(el){
            console.log("-------- ------ --  valueUpdated.subscribe");
            console.log(el);
            let prop = this._formEditorService.instanceProperties
            .filter(p => p.name.toLowerCase() == el.propertyName.toLowerCase())

            if(prop[0]){
              let contextProp = {}
              if(prop[0]?.accessor)
                this._dataAccessorsService.CallSetter(el.value, contextProp, prop[0]);
              
             
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
                console.log(customProps);
                this.applyProps(customProps);
           }
        );
        this._formEditorService.dependentProperties.subscribe( dependentProps => {
               this.applyProps(dependentProps, true);
        })
    };
    
   
    applyProps(customProps:any[], isDependentProps:boolean = false){
        let instanceProps = this._formEditorService.createListProperties(this.instanceName, customProps, isDependentProps);
        instanceProps = this._formEditorService.sortProperties(instanceProps);
        this.properties.next(instanceProps);
    }
  

}