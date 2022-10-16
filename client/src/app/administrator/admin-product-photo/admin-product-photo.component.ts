import { Component, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { HideProductImageModalComponent } from 'src/app/_modals/hide-product-image-modal/hide-product-image-modal.component';
import { ProductImageModalComponent } from 'src/app/_modals/product-image-modal/product-image-modal.component';
import { IdArray } from 'src/app/_models/adminRequest';
import { ManagerProduct, ManagerProductPhoto, Product, ProductPhoto } from 'src/app/_models/product';
import { AuthenticationService } from 'src/app/_services/authentication.service';
import { ProductService } from 'src/app/_services/product.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-admin-product-photo',
  templateUrl: './admin-product-photo.component.html',
  styleUrls: ['./admin-product-photo.component.css']
})
export class AdminProductPhotoComponent implements OnInit {
  product: ManagerProduct;
  productPhotos: ManagerProductPhoto[];
  uploader: FileUploader;
  hasBaseDropzoneOver = false;
  baseUrl = environment.apiUrl;
  bsModalRef: BsModalRef;
  modalAction: string;
  multipleSelected: boolean;

  constructor(private productService: ProductService, private authenticationService: AuthenticationService, private modalService: BsModalService) { }

  ngOnInit(): void {
    this.loadProductDetail(this.productService.getSelectedProductId());
    this.initializeUploader();
    this.modalAction = "";
    this.multipleSelected = true;
  }

  loadProductDetail(productId: number) {
    this.productService.getManagerProduct(productId).subscribe(result => {
      this.product = result;
      this.productPhotos = this.product.productPhotos;
    })
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'product/add-product-photo/' + this.productService.getSelectedProductId(),
      authToken: 'Bearer ' + this.authenticationService.userValue.jwtToken,
      isHTML5: true,
      allowedFileType: ['image', 'video'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });
    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = true
    }
    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const photo: ManagerProductPhoto = JSON.parse(response);
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

  openPhotosModal(product: ManagerProduct, action: string) {
    let actionTitle = "";
    this.modalAction = action;
    switch (this.modalAction) {
      case "hide":
        actionTitle = "Hide photos of " + product.productName;
        this.multipleSelected = true;
        break;
      case "unhide":
        actionTitle = "Unhide photos of " + product.productName;
        this.multipleSelected = true;
        break;
      case "delete":
        actionTitle = "Recover photos of " + product.productName;
        this.multipleSelected = true;
        break;
      default:
        actionTitle = "Choose main photo of " + product.productName;
        this.multipleSelected = false;
        break;
    }
    const config = {
      class: 'modal-dialog-centered',
      initialState: {
        product,
        productPhotos: this.getHidePhotosArray(product),
        action: actionTitle,
        multiple: this.multipleSelected
      }
    }
    this.bsModalRef = this.modalService.show(ProductImageModalComponent, config);
    this.bsModalRef.content.updateSelectedPhotos.subscribe(values => {
      const photosToUpdate = {
        photoIds: [...values.filter(el => el.checked === true).map(el => el.id)]
      };
      console.log(photosToUpdate.photoIds);
      if (photosToUpdate) {
        let ids: IdArray = {
          ids: photosToUpdate.photoIds
        }
        switch (action) {
          case "hide":
            this.productService.hideProductImage(ids).subscribe(() => {
              this.loadProductDetail(this.productService.getSelectedProductId());
            })
            break;
          case "unhide":
            this.productService.unhideProductImage(ids).subscribe(() => {
              this.loadProductDetail(this.productService.getSelectedProductId());
            })
            break;
          case "delete":
            this.productService.deleteProductImage(ids).subscribe(() => {
              this.loadProductDetail(this.productService.getSelectedProductId());
            })
            break;
          default:
            this.productService.setMainProductImage(this.product.id, photosToUpdate.photoIds[0]).subscribe(() => {
              this.loadProductDetail(this.productService.getSelectedProductId());
            })
            break;
        }

      }
    })
  }


  private getHidePhotosArray(product) {

    const productPhotos = [];

    product.productPhotos.forEach(productPhoto => {
      switch (this.modalAction) {
        case "hide":
          if (!productPhoto.isMain && productPhoto.status == 0) {
            productPhoto.checked = false;
            productPhotos.push(productPhoto);
          }
          break;
        case "unhide":
          if (!productPhoto.isMain && productPhoto.status == 1) {
            productPhoto.checked = false;
            productPhotos.push(productPhoto);
          }
          break;
        case "delete":
          if (!productPhoto.isMain && productPhoto.status != 2) {
            productPhoto.checked = false;
            productPhotos.push(productPhoto);
          }
          break;
        default:
          if (!productPhoto.isMain && productPhoto.status == 0) {
            productPhoto.checked = false;
            productPhotos.push(productPhoto);
          }
          break;
      }

    });
    return productPhotos;
  }
}
