

import { AuthorizeService, } from '../authorize.service';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { UserForRegistrationDto, userRole } from 'src/app/common/objects/common';
import { AlertService } from 'src/app/alert/alert.service';
import { ApplicationService } from 'src/app/common/services/application.service';

@Component({
  selector: 'app-register-user',
  templateUrl: './register-user.component.html',
  styleUrls: ['./register-user.component.css']
})

export class RegisterUserComponent implements OnInit {
  public registerForm: FormGroup;
  public hidePassword = true;
  public hidepasswordConfirm = true;
  public isPasswordMatch:boolean = true;

  username  = new FormControl( '', [Validators.required, Validators.minLength(4)]);
  email  = new FormControl( '', [Validators.required, Validators.email]);
  password = new FormControl( '', [Validators.required, Validators.minLength(4)]);
  passwordConfirm = new FormControl( '', [Validators.required, Validators.minLength(4)]);
  userRole = new FormControl( '', [Validators.required]);

  userRoles: userRole[] = [
    {value: 'Administrator', viewValue: 'Administrator'},
    {value: 'User', viewValue: 'User'},
  ];
  constructor(private _applicationService: ApplicationService,
              private alertService:AlertService ) { }

  ngOnInit(): void {
    this.registerForm = new FormGroup({
      username: this.username,
      email: this.email,
      password: this.password,
      passwordConfirm: this.passwordConfirm,
      userRole: this.userRole
    }, this.passwordMatchValidator);
  }

  public passwordMatchValidator(g: FormGroup) {
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

