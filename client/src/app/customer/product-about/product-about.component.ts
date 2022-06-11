import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-product-about',
  templateUrl: './product-about.component.html',
  styleUrls: ['./product-about.component.css']
})
export class ProductAboutComponent implements OnInit {

  comment: string;
  constructor() { }
  
  ngOnInit(): void {
    this.comment = "<div class='row'><div class='col-12 col-md-6'><h5 class='mt-5'>SAY HELLO TO INCREDIBLE ENERGY RETURN</h5><p>These Ultraboost running shoes serve up comfort and responsiveness. You'll be riding on a BOOST midsole for endless energy, with a Linear Energy Push system and a Continental™ Rubber outsole. This shoe's upper is made with a high-performance yarn which contains at least 50% Parley Ocean Plastic —&nbsp; reimagined plastic waste, intercepted on remote islands, beaches, coastal communities and shorelines, preventing it from polluting our ocean.</p></div><div class='col-12 col-md-6'><img class='w-100' src='https://assets.adidas.com/images/w_600,f_auto,q_auto/160b11df40af4233a40fad9100ef48b1_9366/Ultraboost_22_Shoes_White_GX5459.jpg' alt=''></div></div>";
  }

}
