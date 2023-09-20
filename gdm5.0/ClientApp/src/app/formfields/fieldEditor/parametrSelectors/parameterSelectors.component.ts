import { Component, Input, OnInit } from "@angular/core";
import { IDependentProperties, IParameter, MetadataProperty } from "src/app/common/objects/common";
import { ApplicationService } from "src/app/common/services/application.service";
import { AppStateService } from "src/app/common/services/appState.service";
import { FormEditorService } from "src/app/common/services/formEditor.service";

@Component({
    selector: 'selector-list-parametrs',
    templateUrl: './parameterSelectors.component.html',
    styleUrls: ['./parameterSelectors.component.css']
  })
export class ParameterSelectorsComponent implements OnInit {
    @Input("property") _property: any;
    //@Input("items") items: Observable<ISelectableItem[]>;

    typeName:string;
    isDateTimeRange: boolean;
    category:string;
    constructor(private _applicationService : ApplicationService, 
        private _appStateService : AppStateService,
        private _FormEditorService: FormEditorService) {
        
       
    }

    ngOnInit(){
        console.log(" init ParametrSelectorsComponent");
        console.log(this._property);
        this.category = this._property.category;
        this.createParametersAsSelectors(this._property, this._appStateService.selectedInstancePanel)
    }

    public  createParametersAsSelectors(property:any, selectedProduct:string){
        console.log(" ------- ------ - - -- - - --  init --- createParametersAsSelectors");
        let listParameters = [];
            this._applicationService.getProductParameters2(selectedProduct)
            .subscribe((data:IParameter[] | any[]) =>{
                if(typeof data == "object" && data.length > 0){
                    data = data.filter((item:IParameter)=> item.value.toLocaleLowerCase() !== "номер");
                    data.forEach((item:IParameter) => {
                        const p = new MetadataProperty(item.value, "string", undefined, 
                        item.id, item.value,"selector","selector","loadInstancesParameter",undefined,
                        "getGridDataByParameter",item.priority,false,item.priority,false,false,false,false,true,false, this.category);
                        listParameters.push(p);
                    })

                    delete this._FormEditorService.instanceData.parameters;    
                    this._FormEditorService.listParameters = {};
                    
                    const dependentProperties: IDependentProperties = {
                        metadataTypeName : "",
                        properties : listParameters
                    }
                     
                    this._FormEditorService.dependentProperties.next(dependentProperties);
                }
                
            })

    }
}
//   properties:IMetadataProperty[]
            //  if(typeof data == "object" && data.length > 0){
            //      data.forEach((item:IParameter) => {
            //         const p = new MetadataProperty(item.value, "string", undefined, 
            //         item.id, item.value, undefined,undefined,undefined,
            //         undefined,undefined,0,false,0,false,false,false,false,true,false);
            //         listParameters.push(p);
            //      })
            //  }
            //  console.log("--------- ------ listParameters");
            //  console.log(listParameters);
        
            //  delete this._FormEditorService.instanceData.parameters;    
            //  this._FormEditorService.listParameters = {};
            //  this._FormEditorService.dependentProperties.next(listParameters);

