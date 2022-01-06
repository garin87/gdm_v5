import { Injectable } from "@angular/core";
import { ApplicationService } from "./application.service";


@Injectable()
export class MetadataService {
    
    private alltypes:any;
    get metadataTypes():any { return this.alltypes};
    public loaded:boolean = false;

    constructor(private _applicationService: ApplicationService) {

        this._applicationService.getMetadata().subscribe(response => {
            this.alltypes = response;
            console.log("---  console.log(this.mtypes);");
            console.log(this.alltypes);
            this.loaded = true;
          },
          err => {
             console.error(err);
             this.loaded = false;
          })
    }

    getMetadataType(metadataType){
       return this.alltypes[metadataType];
    }
}