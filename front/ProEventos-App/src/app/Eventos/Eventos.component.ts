import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-Eventos',
  templateUrl: './Eventos.component.html',
  styleUrls: ['./Eventos.component.scss']
})
export class EventosComponent implements OnInit {

  public eventos: any

  constructor(private http: HttpClient) { }

  ngOnInit() { //Esse metodo sempre eh chamado antes de iniciar a aplicacao.
    this.getEventos() //Por esse motivo passamos o getEventos para esse metodo, pois antes da pagina carregar, os valores de eventos ja tem que estar presentes no corpo da pagina.
  }

  public getEventos(): void {

    this.http.get('https://localhost:5001/api/eventos').subscribe(
      response => this.eventos = response,
      error => console.log(error),
    );

  }

}
