import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { ArticleService } from '../article.service';
import { User } from '../interface/user';

@Component({
  selector: 'app-toolbar',
  templateUrl: './toolbar.component.html',
  styleUrls: ['./toolbar.component.css']
})
export class ToolbarComponent implements OnInit {

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
    private router: Router,
    private snackBar: MatSnackBar,
  ) { }

  public ngOnInit(): void {
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
      this.articleService.userLogout().subscribe(
        () => {
          this.snackBar.open('登出成功', 'OK', { horizontalPosition: 'center', verticalPosition: 'bottom', duration: 3000 });
          this.router.navigate(['articles']);
        }
      );
    }
  }
}
