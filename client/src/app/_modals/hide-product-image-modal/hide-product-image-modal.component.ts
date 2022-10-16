import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ManagerProduct, ManagerProductPhoto } from 'src/app/_models/product';

@Component({
  selector: 'app-hide-product-image-modal',
  templateUrl: './hide-product-image-modal.component.html',
  styleUrls: ['./hide-product-image-modal.component.css']
})
export class HideProductImageModalComponent implements OnInit {
  @Input() updateSelectedPhotos = new EventEmitter();
  product: ManagerProduct;
  productPhotos: ManagerProductPhoto[];

  constructor(public bsModalRef: BsModalRef) { }

  ngOnInit(): void {
  }

  update() {
    this.updateSelectedPhotos.emit(this.productPhotos);
    this.bsModalRef.hide();
  }

}
