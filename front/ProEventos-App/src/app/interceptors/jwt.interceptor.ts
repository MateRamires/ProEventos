import { take } from 'rxjs/operators';
import { AccountService } from './../services/account.service';
import { User } from '@app/models/Identity/user';
import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private accountService: AccountService) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    let currentUser: User;

    currentUser = JSON.parse(localStorage.getItem('user'));
    if (currentUser) {
      request = request.clone(
        {
          setHeaders: {
            Authorization: `Bearer ${currentUser.token}`
          }
        }
      );
    }



    return next.handle(request);
  }
}
