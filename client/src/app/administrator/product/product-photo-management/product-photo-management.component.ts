import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FileUploader } from 'ng2-file-upload';
import { Product, ProductPhoto } from 'src/app/_models/product';
import { PhotoService } from 'src/app/_services/photo.service';
import { ProductService } from 'src/app/_services/product.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-product-photo-management',
  templateUrl: './product-photo-management.component.html',
  styleUrls: ['./product-photo-management.component.css']
})
export class ProductPhotoManagementComponent implements OnInit {
  photos: ProductPhoto[];
  product: Product;
  uploader: FileUploader;
  hasBaseDropzoneOver = false;
  baseUrl = environment.apiUrl;
  
  constructor(private productService: ProductService, private photoService: PhotoService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.loadProduct(+(this.route.snapshot.paramMap.get('id')));
    this.loadPhotos(this.product.id);
    this.initializeUploader();
  }

  loadPhotos(id: number) {
    this.productService.getProduct(id).subscribe(result => {
      this.photos = result.productPhotos;
    })
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'product/add-product-photo/' + this.product.id,
      authToken: 'Bearer ',
      isHTML5: true,
      allowedFileType: ['image'],
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
        this.photos.push(photo);
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

  loadProduct(id: number) {
    this.productService.getProduct(id).subscribe(response => {
      this.product = response;
    })
  }

  setMainPhoto(photoId: number) {
    this.photoService.setMainPhoto(this.product.id, photoId).subscribe(result => {
      this.productService.removeProductCache(this.product.id);
    });
  }

  deletePhoto(photoId: number) {
    this.photoService.deletePhoto(this.product.id, photoId).subscribe(result => {
      this.productService.removeProductCache(this.product.id);
      this.photos = this.photos.filter(pp => pp.id !== photoId);
    });
  }
}
