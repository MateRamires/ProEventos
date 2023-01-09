import { environment } from './../../../../environments/environment';
import { Router } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { EventoService } from './../../../services/evento.service';
import { Evento } from './../../../models/Evento';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Component, OnInit, TemplateRef } from '@angular/core';
import { PaginatedResult, Pagination } from '@app/models/Pagination';

@Component({
  selector: 'app-evento-lista',
  templateUrl: './evento-lista.component.html',
  styleUrls: ['./evento-lista.component.scss']
})
export class EventoListaComponent implements OnInit {

  modalRef?: BsModalRef;

  public eventos: Evento[] = [];
  public eventosFiltrados: Evento[] = [];
  public eventoId = 0;
  public pagination = {} as Pagination;

  public mostrarImagem: boolean = true;

  private _filtroLista: string = "";

  public get filtroLista(): string {
    return this._filtroLista;
  }

  public set filtroLista(value: string) {
    this._filtroLista = value;
    this.eventosFiltrados = this.filtroLista ? this.filtrarEventos(this.filtroLista) : this.eventos;
  }

  public filtrarEventos(filtrarPor: string): Evento[] {
    filtrarPor = filtrarPor.toLocaleLowerCase();

    return this.eventos.filter(
      (evento: any) => evento.tema.toLocaleLowerCase().indexOf(filtrarPor) !== -1 ||
        evento.local.toLocaleLowerCase().indexOf(filtrarPor) !== -1
    );
  }

  constructor(
    private eventoService: EventoService,
    private modalService: BsModalService,
    private toastr: ToastrService,
    private spinner: NgxSpinnerService,
    private router: Router
  ) { }

  public ngOnInit() { //Esse metodo sempre eh chamado antes de iniciar a aplicacao.
    this.pagination = {currentPage: 1, itemsPerPage: 3, totalItems: 1} as Pagination;
    this.carregarEventos() //Por esse motivo passamos o carregarEventos para esse metodo, pois antes da pagina carregar, os valores de eventos ja tem que estar presentes no corpo da pagina.
  }

  public carregarEventos(): void {

    this.spinner.show();

    /*const observer = {
      next: (eventosResp: Evento[]) =>{
        this.eventos = eventosResp;
        this.eventosFiltrados = this.eventos
      },
      error: (error: any) => console.log(error),
      complete: () => {}
    }*/   //Modelo de um observer

    this.eventoService.getEventos(this.pagination.currentPage, this.pagination.itemsPerPage).subscribe({
      next: (paginatedResult: PaginatedResult<Evento[]>) => {
        this.eventos = paginatedResult.result;
        this.eventosFiltrados = this.eventos;
        this.pagination = paginatedResult.pagination;
      },
      error: (error: any) => {
        this.spinner.hide(),
          this.toastr.error('Errro ao Carregar os Eventos.', 'Erro!');
      },
      complete: () => this.spinner.hide()
    });

  }

  public alterarEstadoImagem(): void {
    this.mostrarImagem = !this.mostrarImagem;
  }

  public mostraImagem(imagemURL: string): string {
    return (imagemURL !== '')
    ? `${environment.apiURL}resources/images/${imagemURL}`
    : 'assets/semImagem.png';
  }

  openModal(event: any, template: TemplateRef<any>, eventoId: number) {
    event.stopPropagation(); //Evita que outros eventos sejam acionados ao clicar no botao de excluir, nesse caso, ele nao ira para a tela de detalhes de eventos ao clicar no excluir.
    this.eventoId = eventoId;
    this.modalRef = this.modalService.show(template, { class: 'modal-sm' });
  }

  pageChanged($evento): void {

  }

  confirm(): void {
    this.modalRef?.hide();
    this.spinner.show();

    this.eventoService.deleteEvento(this.eventoId).subscribe(
      (result: any) => {
        if (result.message === "Deletado") {
          this.toastr.success('O Evento foi deletado com sucesso.', 'Deletado!');
          this.carregarEventos();
        }
      },
      (error: any) => {
        console.error(error);
        this.toastr.error(`Erro ao tentar deletar o Evento ${this.eventoId}`, 'Erro!');
      }
    ).add(() => this.spinner.hide());

  }

  decline(): void {
    this.modalRef?.hide();
  }

  detalheEvento(id: number): void {
    this.router.navigate([`eventos/detalhe/${id}`]);
  }

}
