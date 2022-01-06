import { Component } from '@angular/core';
import { AlertService } from '../alert/alert.service';
import { ApplicationService } from '../common/services/application.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {

  constructor(private alertService : AlertService, 
              private _applicationService: ApplicationService){}
  success(){
    this.alertService.success("test alertService - success");
  }
  error(){
    this.alertService.error("test alertService - ERROR");
  }

  getMetadata(){
    this._applicationService.getMetadata().subscribe(el => console.log(el))
  }

  test(){
    this.alertService.confirmModal("test alertService - ERROR","Make a confirm");
  }
  test2(){
    this.alertService.choiceModal("tttttttttt", "Make a choice");
  }
  test3(){
    this.alertService.warningModal("eeeeeeeddcc", "Warning");
  }
}    
