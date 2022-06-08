import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { catchError, filter, map, switchMap   } from 'rxjs/operators';

import { Response } from './interface/response';
import { User } from './interface/user';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Router } from '@angular/router';


@Injectable({
  providedIn: 'root'
})
export class ArticleService {

  private articleUrl = 'api/article';
  private userUrl = 'api/user';

  private httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
  };

  private _status: User = {
    Id: 0,
    Name: '',
    Status: 0,
    exp: 0,
    iat: 0
  };

  public get status(): User {
    return this._status;
  }

  private set status(status: User) {
    this._status = status;
  }

  private _userStatus = new BehaviorSubject<User>(this.status);

  public get userStatus(): BehaviorSubject<User> {
    return this._userStatus;
  }

  private set userStatus(userStatus: BehaviorSubject<User>) {
    this._userStatus = userStatus;
  }

  constructor(
    private http: HttpClient,
    private router: Router,
    private jwt: JwtHelperService,
  ) { }

  public login(user: User): void {
    this.userStatus.next(user);
  }

  public logout(): void {
    const guest = {
      Id: 0,
      Name: '',
      Status: 0,
      exp: 0,
      iat: 0
    };
    this.userStatus.next(guest);
    localStorage.removeItem('TOKEN');
  }

  public getUserStatus(): Observable<User> {
    return this.userStatus;
  }

  public setAuthorization(token: string): void {
    this.httpOptions.headers = this.httpOptions.headers.set('Authorization', `Bearer ${token}`);
  }

  public userLogin(name: string, password: string): Observable<Response<string>> {
    return this.http.post<Response<string>>(`${this.userUrl}/login`, { UserName: name, Password: password }).pipe(
      filter(article => {
        if (article.StatusCode === 200) {
          this.setAuthorization(article.Data);
          this.login(this.jwt.decodeToken(article.Data));
          localStorage.setItem('TOKEN', article.Data);
          return true;
        }
        else if (article.StatusCode === 400) {
          alert('輸入欄位不可為空');
          return false;
        }
        else if (article.StatusCode === 10) {
          alert('帳號或密碼錯誤');
          return false;
        }
        else {
          alert('系統錯誤');
          return false;
        }
      }),
      catchError(this.handleError<Response<string>>(`userLogin name=${name}`))
    );
  }

  public userSignUp(name: string, password: string): Observable<Response<User>> {
    return this.http.post<Response<User>>(`${this.userUrl}/sign`, { UserName: name, Password: password }).pipe(
      filter(article => {
        if (article.StatusCode === 200) {
          return true;
        }
        else if (article.StatusCode === 400) {
          alert('輸入欄位不可為空');
          return false;
        }
        else if (article.StatusCode === 11) {
          alert('使用者名稱已存在');
          return false;
        }
        else {
          alert('系統錯誤');
          return false;
        }
      }),
      catchError(this.handleError<Response<User>>(`userSignUp name=${name}`))
    );
  }

  public userLogout(): Observable<Response<string>> {
    return this.http.put<Response<string>>(`${this.userUrl}/logout`, null, this.httpOptions).pipe(
      filter(article => {
        if (article.StatusCode === 200) {
          this.logout();
          return true;
        }
        else if (article.StatusCode === 1) {
          alert('無效的憑證，請重新登入');
          this.logout();
          this.router.navigate(['login']);
          return false;
        }
        else if (article.StatusCode === 2) {
          alert('憑證已過期，請重新登入');
          this.logout();
          this.router.navigate(['login']);
          return false;
        }
        else if (article.StatusCode === 3) {
          alert('憑證不存在，請重新登入');
          this.logout();
          this.router.navigate(['login']);
          return false;
        }
        else if (article.StatusCode === 4) {
          alert('帳號正在其他裝置使用，請重新登入');
          this.logout();
          this.router.navigate(['login']);
          return false;
        }
        else {
          alert('系統錯誤');
          this.logout();
          return false;
        }
      }),
      catchError(this.handleError<Response<string>>('userLogout'))
    );
  }

  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      console.log(error);
      console.log(`${operation} failed: ${error.message}`);
      return of(result as T);
    };
  }
}