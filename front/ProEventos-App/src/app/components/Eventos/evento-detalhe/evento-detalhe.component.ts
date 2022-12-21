import { LoteService } from './../../../services/lote.service';
import { Lote } from './../../../models/Lote';
import { Evento } from './../../../models/Evento';
import { EventoService } from './../../../services/evento.service';

import { Component, OnInit } from '@angular/core';
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

  eventoId: number;
  form!: FormGroup;
  evento = {} as Evento;
  estadoSalvar = 'post';

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

  get lotes(): FormArray {
    return this.form.get('lotes') as FormArray
  }

  constructor(private fb: FormBuilder,
    private localeService: BsLocaleService,
    private activeRoute: ActivatedRoute,
    private eventoService: EventoService,
    private spinner: NgxSpinnerService,
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

    if (this.eventoId !== null || this.eventoId === 0) {
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
    this.spinner.show();
    if(this.form.controls['lotes'].valid){
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

}
