import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { AccountService } from './../../../services/account.service';
import { User } from '@app/models/Identity/user';
import { ValidatorField } from './../../../helpers/ValidatorField';
import { AbstractControlOptions } from '@angular/forms';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.scss']
})
export class RegistrationComponent implements OnInit {

  form!: FormGroup;
  user = {} as User;

  constructor(private fb: FormBuilder, private accountService: AccountService, private router: Router, private toaster: ToastrService) { }

  get f(): any {
    return this.form.controls;
  }

  ngOnInit(): void {
    this.validation();
  }

  public validation(): void {

    const formOptions: AbstractControlOptions = {
      validators: ValidatorField.MustMatch('password', 'confirmePassword')
    }

    this.form = this.fb.group({
      primeiroNome:['', Validators.required],
      ultimoNome: ['', Validators.required],
      email:['', [Validators.required, Validators.email]],
      userName:['', Validators.required],
      password:(['', [Validators.required, Validators.minLength(4)]]),
      confirmePassword:['', Validators.required],
    }, formOptions)
  }

  register(): void {
    this.user = {... this.form.value };
    this.accountService.register(this.user).subscribe(
      () => {this.router.navigateByUrl('/dashboard');},
      (error) => {this.toaster.error(error.error)}
    )
  }

}
