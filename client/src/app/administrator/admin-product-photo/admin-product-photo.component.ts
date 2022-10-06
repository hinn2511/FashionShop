import { Component, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { Product, ProductPhoto } from 'src/app/_models/product';
import { ProductService } from 'src/app/_services/product.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-admin-product-photo',
  templateUrl: './admin-product-photo.component.html',
  styleUrls: ['./admin-product-photo.component.css']
})
export class AdminProductPhotoComponent implements OnInit {
  product: Product;
  productPhotos: ProductPhoto[];
  uploader: FileUploader;
  hasBaseDropzoneOver = false;
  baseUrl = environment.apiUrl;
  
  constructor(private productService: ProductService) { }

  ngOnInit(): void {
    this.loadProductDetail(this.productService.getSelectedProductId());
    this.initializeUploader();
  }

  loadProductDetail(productId: number) {
    this.productService.getProduct(productId).subscribe(result => {
      this.product = result;
      this.productPhotos = this.product.productPhotos;
    })
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'product/add-product-photo/' +  this.product.id,
      authToken: 'Bearer ',
      isHTML5: true,
      allowedFileType: ['image', 'video'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });
    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false
    }
    this.uploader.onSuccessItem = (item, response, status, headers ) => {
      if (response) {
         const photo: ProductPhoto = JSON.parse(response);
        // console.log(photo);
        this.productPhotos.push(photo);
        this.productService.removeProductCache(this.product.id);
        // const photo: ProductPhoto = JSON.parse(response);
        // this.member.photos.push(photo);
        // if (photo.isMain) {
        //   this.user.photoUrl = photo.url;
        //   this.member.photoUrl = photo.url;
        //   this.accountService.setCurrentUser(this.user);

        // }
      }
    }

  }
  
  fileOverBase(e: any) {
    this.hasBaseDropzoneOver = e;
  }

}
