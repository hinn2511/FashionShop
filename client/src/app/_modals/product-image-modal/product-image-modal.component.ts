import { Component, EventEmitter, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ManagerProduct, ManagerProductPhoto, SelectedProductPhoto } from 'src/app/_models/product';


@Component({
  selector: 'app-product-image-modal',
  templateUrl: './product-image-modal.component.html',
  styleUrls: ['./product-image-modal.component.css']
})
export class ProductImageModalComponent implements OnInit {

  @Input() updateSelectedPhotos = new EventEmitter();
  @Input() action: string;
  @Input() multiple: boolean;
  product: ManagerProduct;
  productPhotos: SelectedProductPhoto[];
  selectAllPhoto: boolean;

  constructor(public bsModalRef: BsModalRef) { }

  ngOnInit(): void {
  }

  update() {
    this.updateSelectedPhotos.emit(this.productPhotos);
    this.bsModalRef.hide();
    this.selectAllPhoto = false;
  }

  select(index: number) {
    if (!this.multiple) {
      this.productPhotos.forEach(x => {
        x.checked = false;
      })
      
    }
    this.productPhotos[index].checked = !this.productPhotos[index].checked;

  }

  selectAll() {
    if (this.selectAllPhoto) {
     this.productPhotos.forEach(x => {
        x.checked = false;
      })
    }
    else
    {
      this.productPhotos.forEach(x => {
        x.checked = true;
      }) 
    }

    this.selectAllPhoto = !this.selectAllPhoto;
   
  }

}
