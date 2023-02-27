import { ToastrService } from 'ngx-toastr';
import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthenticationService } from '../_services/authentication.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    constructor(private authenticationService: AuthenticationService, private toarst: ToastrService) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(request).pipe(catchError(err => {
            if ([401].includes(err.status) && this.authenticationService.userValue) {
                if(localStorage.getItem('user') != null || localStorage.getItem('user') != undefined)
                    localStorage.removeItem('user');
                this.authenticationService.logout();
            }

            if ([403].includes(err.status) && this.authenticationService.userValue) {
                this.toarst.error("Access denied", "Error");
            }            
            const error = (err && err.error && err.error.message) || err.statusText;
            return throwError(error);
        }))
    }
}