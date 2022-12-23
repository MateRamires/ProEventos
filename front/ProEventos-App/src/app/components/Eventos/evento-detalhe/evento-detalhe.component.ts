import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { LoteService } from './../../../services/lote.service';
import { Lote } from './../../../models/Lote';
import { Evento } from './../../../models/Evento';
import { EventoService } from './../../../services/evento.service';

import { Component, OnInit, TemplateRef } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators, AbstractControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import { ToastrService } from 'ngx-toastr';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-evento-detalhe',
  templateUrl: './evento-detalhe.component.html',
  styleUrls: ['./evento-detalhe.component.scss']
})
export class EventoDetalheComponent implements OnInit {

  modalRef: BsModalRef;
  eventoId: number;
  form!: FormGroup;
  evento = {} as Evento;
  estadoSalvar = 'post';
  loteAtual = {id: 0, nome: "", indice: 0};
  imagemURL = 'assets/upload.png'

  get modoEditar(): boolean {
    return this.estadoSalvar === "put" //Quando a tela estiver em modo PUT, a variavel modoEditar vai retornar TRUE, mostrando assim o formulario de lotes. Pois quando a tela for de criacao de um novo Evento, nao deve aparecer o form de Lotes.
  }

  get f(): any {
    return this.form.controls;
  }

  get bsConfig(): any {
    return {
      isAnimated: true,
      adaptivePosition: true,
      dateInputFormat: 'DD/MM/YYYY hh:mm a',
      containerClass: 'theme-default',
      showWeekNumbers: false
    }
  }

  get bsConfigLote(): any {
    return {
      isAnimated: true,
      adaptivePosition: true,
      dateInputFormat: 'DD/MM/YYYY',
      containerClass: 'theme-default',
      showWeekNumbers: false
    }
  }

  get lotes(): FormArray {
    return this.form.get('lotes') as FormArray
  }

  constructor(private fb: FormBuilder,
    private localeService: BsLocaleService,
    private activeRoute: ActivatedRoute,
    private eventoService: EventoService,
    private spinner: NgxSpinnerService,
    private modalService: BsModalService,
    private toastr: ToastrService,
    private router: Router,
    private loteService: LoteService) {
    this.localeService.use('pt-br')
  }


  ngOnInit(): void {
    this.carregarEvento();
    this.validation();
  }

  public validation(): void {
    this.form = this.fb.group({
      tema: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(50)]],
      local: ['', Validators.required],
      dataEvento: ['', Validators.required],
      qtdPessoas: ['', [Validators.required, Validators.max(12000)]],
      telefone: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      imageURL: ['', Validators.required],
      lotes: this.fb.array([])
    })
  }


  public carregarEvento(): void {
    this.eventoId = +this.activeRoute.snapshot.paramMap.get('id');

    if (this.eventoId !== null && this.eventoId !== 0) {
      this.spinner.show();

      this.estadoSalvar = 'put';

      this.eventoService.getEventoById(this.eventoId).subscribe(  //+ ira converter a string para number.
        (evento: Evento) => {
          this.evento = { ...evento }; //Se eu utilizasse apenas o this.evento = evento, ele iria apenas atribuir e memoria seria perdida, com esse spread operator isso nao ocorre, portanto eh a melhor forma.
          this.form.patchValue(this.evento);
          this.evento.lotes.forEach(lote => {
            this.lotes.push(this.criarLote(lote))
          });

          //this.carregarLotes();
        },
        (error: any) => {
          this.toastr.error('Erro ao tentar carregar evento.', 'Erro!')
          console.error(error);
        }
      ).add(() => this.spinner.hide());
    }
  }

  //Nao esta sendo utilizado, mas eh util usar dessa maneira quando a classe principal nao possui essa classe secundaria imbutida, sendo necessario realizar outra busca no banco.
  public carregarLotes(): void {
    this.loteService.getLotesByEventoId(this.eventoId).subscribe(
      (lotesRetorno: Lote[]) => {
        lotesRetorno.forEach(lote => {
          this.lotes.push(this.criarLote(lote)) //Quando fazemos this.lotes.push, estamos fazendo a mesma coisa da funcao adicionarLotes, so que nesse caso,estamos adicionando cada um dos lotes que esta vindo do banco de dados.
        })
      },
      (error) => {
        this.toastr.error("Erro ao tentar carregar lotes", "Erro");
        console.error(error);
      }
    ).add(() => this.spinner.hide());
  }


  adicionarLote(): void {
    this.lotes.push(this.criarLote({ id: 0 } as Lote));
  }

  criarLote(lote: Lote): FormGroup {
    return this.fb.group({
      id: [lote.id],
      nome: [lote.nome, Validators.required],
      quantidade: [lote.quantidade, Validators.required],
      preco: [lote.preco, Validators.required],
      dataInicio: [lote.dataInicio],
      dataFim: [lote.dataFim]
    })
  }

  public retornaTituloLote(nome: string): string {
    return nome === null || nome === '' ? 'Nome do lote' : nome
  }

  public resetForm(): void {
    this.form.reset();
  }

  public cssValidator(campoForm: FormControl | AbstractControl): any {
    return { 'is-invalid': campoForm.errors && campoForm.touched }
  }

  public salvarEvento(): void {
    this.spinner.show();
    if (this.form.valid) {

      this.evento = (this.estadoSalvar === 'post')
        ? { ... this.form.value }
        : { id: this.evento.id, ... this.form.value };


      this.eventoService[this.estadoSalvar](this.evento).subscribe(
        (eventoRetorno: Evento) => {

          this.toastr.success('Evento Salvo com Sucesso.', 'Sucesso!');
          this.router.navigate([`eventos/detalhe/${eventoRetorno.id}`])
        },
        (error: any) => {
          console.error(error);
          this.spinner.hide();
          this.toastr.error('Erro ao Salvar Evento.', 'Erro!')
        },
        () => this.spinner.hide()
      );


    }
  }

  public salvarLotes(): void {
    if (this.form.controls['lotes'].valid) {
      this.spinner.show();
      this.loteService.saveLote(this.eventoId, this.form.value.lotes).subscribe(
        () => {
          this.toastr.success('Lotes salvos com Sucesso!', 'Sucesso!');
          //this.lotes.reset();
        },
        (error: any) => {
          this.toastr.error('Erro ao tentar salvar lotes.', 'Erro');
          console.error(error);
        },
      ).add(() => this.spinner.hide())
    }
  }

  public removerLote(template: TemplateRef<any>, indice: number): void {

    this.loteAtual.id = this.lotes.get(indice + '.id').value;
    this.loteAtual.nome = this.lotes.get(indice + '.nome').value;
    this.loteAtual.indice = indice;


    this.modalRef = this.modalService.show(template, {class: 'modal-sm'})
  }

  confirmDeleteLote(): void {
    this.modalRef.hide();
    this.spinner.show();

    this.loteService.deleteLote(this.eventoId, this.loteAtual.id).subscribe(
      () => {
        this.toastr.success("Lote deletado com sucesso!", "Sucesso");
        this.lotes.removeAt(this.loteAtual.indice);
      },
      (error: any) => {
        this.toastr.error(`Erro ao tentar deletar o Lote ${this.loteAtual.id}.`, 'Erro');
        console.error(error);
      }
    ).add(() => this.spinner.hide());
  }

  declineDeleteLote(): void {
    this.modalRef.hide();
  }

}
