import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { ArticleService } from './article.service';
import { User } from './interface/user';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  private _title: string = 'Article-List';

  public get title(): string {
    return this._title;
  }

  private set title(title: string) {
    this._title = title;
  }

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

  constructor(
    private articleService: ArticleService,
    private jwt: JwtHelperService,
    private router: Router,
    private snackBar: MatSnackBar,
  ) { }

  public ngOnInit() {
    const token = localStorage.getItem('TOKEN')?.toString()!;
    if ((Boolean(token)) && (!this.jwt.isTokenExpired(token))) {
      this.articleService.login(this.jwt.decodeToken(token));
      this.articleService.setAuthorization(token);
    }
    this.getUserStatus();
  }

  private getUserStatus(): void {
    this.articleService.getUserStatus()
      .subscribe(status => {
        this.status = status;
      });
  }

  public logout(): void {
    if (confirm('確定要登出帳號嗎?')) {
      this.articleService.logout();
      this.getUserStatus();
      this.snackBar.open('登出成功', 'OK', { horizontalPosition: 'center', verticalPosition: 'bottom', duration: 3000 });
      this.router.navigate(['articles']);
    }
  }

}
