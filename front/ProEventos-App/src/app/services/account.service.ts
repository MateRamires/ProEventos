import { environment } from './../../environments/environment';
import { map, Observable, take } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { User } from '@app/models/Identity/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseUrl = environment.apiURL + 'api/account/';

  constructor(private http: HttpClient) { }


  public login(model: any): Observable<void> {
    return this.http.post<User>(this.baseUrl + 'login', model).pipe(
      take(1),
      map((response: User) => {
        const user = response;
        if(user) {
          
        }
      })
    );
  }

}
