import { Component, ComponentFactoryResolver, ComponentRef, EnvironmentInjector, Input, OnInit, SimpleChange, ViewChild, ViewContainerRef, createComponent } from '@angular/core';
import { AppStateService } from 'src/app/common/services/appState.service';
import { LabelsService } from 'src/app/common/services/labels.service';

import { BehaviorSubject, Subscription } from 'rxjs';
import { FormEditorComponent } from '../formEditor.component';
import { IMetadataProperty } from 'src/app/common/objects/common';

@Component({
  selector: 'baseFieldEditor',
  templateUrl: './baseFieldEditor.component.html',
  styleUrls: ['./baseFieldEditor.component.css']
})

export class BaseFieldEditorComponent implements OnInit{
  @Input("component") component: any;
  @Input("instanceName") metadataTypeName: string;
  @Input("selectedInstance") selectedInstance: string;
  @Input("priority") priority: any;
 // @Input("properties") properties: BehaviorSubject<IMetadataProperty[]>;
  
  private filter_initCreateFilterTileComponentSubscription$:Subscription;
  private createdComponentsPriorities: Array<number> = [];
  private createdComponents: Array<{ componentRef: ComponentRef<any>, priority: number }> = [];

  @ViewChild('FormEditorComponent', { read: ViewContainerRef }) formEditor: ViewContainerRef;

  constructor(private _appStateService:AppStateService,
    public _labelsService: LabelsService,
    private componentFactoryResolver: ComponentFactoryResolver) { };

  ngOnInit() {
    const comp = this.resolveComponent(this.component);
    if( comp ){
      setTimeout(()=>{this.loadComponent(comp)}, 0)
    }
    else{
        console.error("The component " + this.component + " was not resolved");
    }

    this.filter_initCreateFilterTileComponentSubscription$ =  this._appStateService.selectedSideSubPanelValue.subscribe((data)=>{
       // this.loadComponent(comp)
    })

    
  }

  private loadComponent(comp){

    const componentFactory = this.componentFactoryResolver.resolveComponentFactory(comp);
    const viewContainerRef = this.formEditor;

    if(viewContainerRef){
       const componentRef = viewContainerRef.createComponent(componentFactory);;
       const panelInstance = componentRef.instance as any;
       
       panelInstance.metadataTypeName = this.metadataTypeName;
       panelInstance.selectedInstance = this.selectedInstance;
       //panelInstance.properties = new BehaviorSubject<IMetadataProperty[]>(undefined);
       
       this.createdComponentsPriorities.push(this.priority);
       this.createdComponents.push({ componentRef, priority: this.priority });

          // you might need to trigger this too to make sure it runs change detection so the UI of the dynamic component gets updated
       //   panelInstance.changeDetectorRef.detectChanges();
    }
  }

  private destroyDynamicComponentsByPriority(priority: number): void {
    const componentsToRemove = this.createdComponents.filter(dc => dc.priority === priority);
  
    componentsToRemove.forEach(dc => {
      this.destroyDynamicComponent(dc.componentRef);
      const index = this.createdComponents.indexOf(dc);
      if (index > -1) {
        this.createdComponents.splice(index, 1);
      }

    });

    this.createdComponentsPriorities.splice(this.createdComponentsPriorities.indexOf(priority), 1);
  }


  destroyDynamicComponent(componentRef: ComponentRef<any>) {
    componentRef.destroy();
  }

  private resolveComponent(selector: string){
    const comps = {
        "form-editor" : FormEditorComponent,
    };
    return comps[selector];
  }

  ngOnChanges(change){
  };

  ngOnDestroy(){
    this.createdComponentsPriorities = [];
    this.createdComponents = [];
    this.filter_initCreateFilterTileComponentSubscription$.unsubscribe();
  };
}
