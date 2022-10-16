import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { User } from '../_models/user';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
    private userSubject: BehaviorSubject<User>;
    public user: Observable<User>;
    baseUrl = environment.apiUrl;

    constructor(
        private router: Router,
        private http: HttpClient,
    ) {
        this.userSubject = new BehaviorSubject<User>(null);
        this.user = this.userSubject.asObservable();
        ///
        
        ///
    }

    public setUser() {
        const user: User = JSON.parse(localStorage.getItem('user'));
        this.userSubject.next(user);
    }

    public get userValue(): User {
        return this.userSubject.value;
    }

    login(username: string, password: string) {
        return this.http.post<any>(`${environment.apiUrl}account/authenticate`, { username, password }, { withCredentials: true })
            .pipe(map(user => {
                user.roles = [];
                const roles = this.getDecodedToken(user.jwtToken).role;
                Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);
                this.userSubject.next(user);
                ///
                localStorage.setItem('user', JSON.stringify(user));
                ///
                this.startRefreshTokenTimer();
                return user;
            }));
    }

    logout() {
        this.http.post<any>(`${environment.apiUrl}account/revoke-token`, {}, { withCredentials: true }).subscribe();
        
        let bussinessRole: string[] = [  "Admin", "Manager" ];
        let logoutRoute = "/login"
        if (this.userValue?.roles.some(role => bussinessRole.includes(role))) {
            logoutRoute = "/administrator/login";
        }

        this.stopRefreshTokenTimer();
        ///
        localStorage.removeItem('user');
        ///
        this.userSubject.next(undefined);
        this.user = new Observable<User>();
        this.router.navigate([logoutRoute]);
    }

    refreshToken() {
        return this.http.post<any>(`${environment.apiUrl}account/refresh-token`, {}, { withCredentials: true })
            .pipe(map((user) => {
                this.userSubject.next(user);
                ///
                localStorage.setItem('user', JSON.stringify(user));
                ///
                this.startRefreshTokenTimer();
                return user;
            }));
    }

    private refreshTokenTimeout;

    private startRefreshTokenTimer() {
        const jwtToken = JSON.parse(atob(this.userValue.jwtToken.split('.')[1]));

        const expires = new Date(jwtToken.exp * 1000);
        const timeout = expires.getTime() - Date.now() - (60 * 1000);
        this.refreshTokenTimeout = setTimeout(() => this.refreshToken().subscribe(), timeout);
    }

    private stopRefreshTokenTimer() {
        clearTimeout(this.refreshTokenTimeout);
    }

    private getDecodedToken(token) {
        return JSON.parse(atob(token.split('.')[1]));
    }
}