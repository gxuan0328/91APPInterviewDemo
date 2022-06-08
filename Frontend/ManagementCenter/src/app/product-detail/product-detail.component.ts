import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProductDetail } from '../interface/product';
import { Response } from '../interface/response';

@Component({
  selector: 'app-product-detail',
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.css']
})
export class ProductDetailComponent implements OnInit {

  public ELEMENT_DATA: ProductDetail = {
    Id: 0,
    Name: '',
    Description: '',
    CreateTime: '',
    Price: 0,
    Cost: 0
  };

  public displayedColumns: string[] = ['Name', 'Price', 'Quantity', 'Total'];


  constructor(private http: HttpClient, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.getData();
  }

  getData(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.http.get<Response<ProductDetail>>(`https://localhost:5001/api/product/detail/${id}`).subscribe(response => {
      this.ELEMENT_DATA = response.Data;
      console.log(this.ELEMENT_DATA);
    });
  }

}
