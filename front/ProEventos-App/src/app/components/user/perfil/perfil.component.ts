import { NgxSpinnerService } from 'ngx-spinner';
import { UserUpdate } from './../../../models/Identity/UserUpdate';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { AccountService } from './../../../services/account.service';
import { ValidatorField } from './../../../helpers/ValidatorField';
import { FormGroup, FormBuilder, AbstractControlOptions, Validators } from '@angular/forms';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-perfil',
  templateUrl: './perfil.component.html',
  styleUrls: ['./perfil.component.scss']
})
export class PerfilComponent implements OnInit {

  form!: FormGroup;
  userUpdate = {} as UserUpdate

  get f(): any {
    return this.form.controls;
  }

  constructor(public fb: FormBuilder, public accountService: AccountService, private router: Router, private toaster: ToastrService, private spinner: NgxSpinnerService) { }

  ngOnInit() {
    this.validation();
    this.carregarUsuario();
  }

  private carregarUsuario(): void {
    this.spinner.show();
    this.accountService.getUser().subscribe(
      (userRetorno: UserUpdate) => {
        console.log(userRetorno);
        this.userUpdate = userRetorno;
        this.form.patchValue(this.userUpdate);
        this.toaster.success('Usuário Carregado', 'Sucesso');
      },
      (error) => {
        console.error(error);
        this.toaster.error('Erro ao carregar usuário', 'Erro');
        this.router.navigate(['/dashboard']);
      }
    )
    .add(() => this.spinner.hide());
  }

  public validation(): void {

    const formOptions: AbstractControlOptions = {
      validators: ValidatorField.MustMatch('password', 'confirmePassword')
    }

    this.form = this.fb.group({
      titulo: ['', Validators.required],
      primeiroNome: ['', Validators.required],
      ultimoNome: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      telefone: ['', Validators.required],
      funcao: ['', Validators.required],
      descricao: ['', Validators.required],
      password: (['', [Validators.nullValidator, Validators.minLength(4)]]),
      confirmePassword: ['', Validators.nullValidator]
    }, formOptions)
  }

  public resetForm(event: any): void {
    event.preventDefault();
    this.form.reset();
  }

}
