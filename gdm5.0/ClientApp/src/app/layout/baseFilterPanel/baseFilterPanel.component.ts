import { Component, ComponentFactoryResolver, ComponentRef, Input, OnInit, SimpleChange, ViewChild, ViewContainerRef } from '@angular/core';
import { AppStateService } from 'src/app/common/services/appState.service';
import { LabelsService } from 'src/app/common/services/labels.service';
import { FilterTileComponent } from '../filterTile/filterTile.component';
import { Subscription } from 'rxjs';

@Component({
  selector: 'baseFilterPanel',
  templateUrl: './baseFilterPanel.component.html',
  styleUrls: ['./baseFilterPanel.component.css']
})

export class BaseFilterPanelComponent implements OnInit{
  @Input("selectedProduct") selectedProduct: any;
  @Input("dataTile") dataTile: any;
  @Input("component") component: any;
  @Input("priority") priority: any;

  private filter_initCreateFilterTileComponentSubscription$:Subscription;
  private createdComponentsPriorities: Array<number> = [];
  private createdComponents: Array<{ componentRef: ComponentRef<any>, priority: number }> = [];

  @ViewChild('filterPanelContainer', { read: ViewContainerRef }) panelFilter: ViewContainerRef;

  constructor(private _appStateService:AppStateService,
    public _labelsService: LabelsService,
    private componentFactoryResolver: ComponentFactoryResolver) { };

  ngOnInit() {
    const comp = this.resolveComponent(this.component);
    if( comp ){
        this.loadComponent(comp);
    }
    else{
        console.error("The component " + this.component + " was not resolved");
    }

    this.filter_initCreateFilterTileComponentSubscription$ =  this._appStateService.filter_initCreateFilterTileComponent.subscribe((data)=>{
      if(data && data?.ParameterValues){
        this.dataTile = data?.ParameterValues;
        this.priority = data?.Priority;
        if(this.priority){
          const createdComponents = this.createdComponentsPriorities.filter((item)=> item <= this.priority);

          createdComponents.forEach(item =>{
            this.destroyDynamicComponentsByPriority(item);
          })
          
        } 
        
        const comp = this.resolveComponent(this.component);
        if( comp ){
            this.loadComponent(comp);
        }
      }
    })
  }

  private loadComponent(comp){
    const componentFactory = this.componentFactoryResolver.resolveComponentFactory(comp);
    const viewContainerRef = this.panelFilter;

    if(viewContainerRef){
       const componentRef = viewContainerRef.createComponent(componentFactory);
       const panelInstance = componentRef.instance as any;
       panelInstance.selectedProduct = this.selectedProduct;
       panelInstance.dateTile = this.dataTile;
       panelInstance.titleTile = this.dataTile[0].name.toUpperCase();
       panelInstance.priority = this.priority;
       
       this.createdComponentsPriorities.push(this.priority);
       this.createdComponents.push({ componentRef, priority: this.priority });
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
        "app-filterTile" : FilterTileComponent,
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
