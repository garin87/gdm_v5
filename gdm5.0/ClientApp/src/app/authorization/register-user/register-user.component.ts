

import { AuthorizeService, } from '../authorize.service';
import { Component, OnInit } from '@angular/core';
import { UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { UserForRegistrationDto, userRole } from 'src/app/common/objects/common';
import { AlertService } from 'src/app/alert/alert.service';
import { ApplicationService } from 'src/app/common/services/application.service';
import { LabelsService } from 'src/app/common/services/labels.service';

@Component({
  selector: 'app-register-user',
  templateUrl: './register-user.component.html',
  styleUrls: ['./register-user.component.css']
})

export class RegisterUserComponent implements OnInit {
  public registerForm: UntypedFormGroup;
  public hidePassword = true;
  public hidepasswordConfirm = true;
  public isPasswordMatch:boolean = true;

  username  = new UntypedFormControl( '', [Validators.required, Validators.minLength(4)]);
  email  = new UntypedFormControl( '', [Validators.required, Validators.email]);
  password = new UntypedFormControl( '', [Validators.required, Validators.minLength(4)]);
  passwordConfirm = new UntypedFormControl( '', [Validators.required, Validators.minLength(4)]);
  userRole = new UntypedFormControl( '', [Validators.required]);

  userRoles: userRole[] = [
    {value: 'Administrator', viewValue: 'Administrator'},
    {value: 'User', viewValue: 'User'},
  ];
  constructor(private _applicationService: ApplicationService,
              private alertService:AlertService,
              public _labelsService: LabelsService ) { }

  ngOnInit(): void {
    this._labelsService.labels.userRegistration_LableButton

    this.registerForm = new UntypedFormGroup({
      username: this.username,
      email: this.email,
      password: this.password,
      passwordConfirm: this.passwordConfirm,
      userRole: this.userRole
    }, this.passwordMatchValidator);
  }

  public passwordMatchValidator(g: UntypedFormGroup) {
    let password = g.get('password').value;
    let passwordConfirm = g.get('passwordConfirm').value;
    
    if(password === passwordConfirm){
      g.get('passwordConfirm').setErrors(null)
      return null
    }
    else{
      g.get('passwordConfirm').setErrors({ notMatch: true })
      return {'mismatch': true}
    }
  }

  public registerUser = (registerFormValue) => {
    const formValues = { ...registerFormValue };

    const user: UserForRegistrationDto = {
      userName: formValues.username,
      email: formValues.email,
      password: formValues.password,
      confirmPassword: formValues.passwordConfirm,
      userRole: formValues.userRole
    };

    this._applicationService.regUser(user)
    .subscribe(response => {
          this.alertService.success(response.message);
          console.log("Successful registration");
          console.log(response);
        },
        err => {
          this.alertService.error(err.error.message);
          console.log("----  console.log(error) registration ");
          console.log(err);
        })
    }

}

