import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { OrderDetail } from '../interface/order';
import { Response } from '../interface/response';


@Component({
  selector: 'app-order-detail',
  templateUrl: './order-detail.component.html',
  styleUrls: ['./order-detail.component.css']
})
export class OrderDetailComponent implements OnInit {

  public ELEMENT_DATA: OrderDetail = {
    Id: 0,
    Buyer: '',
    Seller: '',
    Products: [],
    PaymentType: '',
    Address: '',
    CreateTime: '',
    OrderStatusType: '',
    Total: 0,
    Cost: 0
  };

  public displayedColumns: string[] = ['Name', 'Price', 'Quantity', 'Total'];


  constructor(private http: HttpClient, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.getData();
  }

  getData(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.http.get<Response<OrderDetail>>(`https://localhost:5001/api/order/detail/${id}`).subscribe(response => {
      this.ELEMENT_DATA = response.Data;
    });
  }
}
