import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { OrderOverview } from '../interface/order';
import { Response } from '../interface/response';

@Component({
  selector: 'app-order-list',
  templateUrl: './order-list.component.html',
  styleUrls: ['./order-list.component.css']
})
export class OrderListComponent implements OnInit {

  public ELEMENT_DATA: OrderOverview[] = [];
  
  public displayedColumns: string[] = ['Id', 'Buyer', 'PaymentType', 'Total', 'Address', 'CreateTime', 'OrderStatusType', 'Management'];

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.getData();
  }

  getData(): void {
    this.http.get<Response<OrderOverview[]>>('https://localhost:5001/api/order/list').subscribe(response => {
      this.ELEMENT_DATA = response.Data;
    });
  }

  updateOrderStatus(id: number): void {
    this.http.put<Response<string>>(`https://localhost:5001/api/order/status/${id}`, {'id': id}).subscribe();
    window.location.reload();
  }
}
