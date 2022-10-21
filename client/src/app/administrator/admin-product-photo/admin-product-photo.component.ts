import { PhotoViewerItem } from './../../_models/product';
import { ImageViewerModalComponent } from './../../_modals/image-viewer-modal/image-viewer-modal.component';
import { Component, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ProductImageModalComponent } from 'src/app/_modals/product-image-modal/product-image-modal.component';
import { IdArray } from 'src/app/_models/adminRequest';
import {
  ManagerProduct,
  ManagerProductPhoto,
  Product,
  ProductPhoto,
} from 'src/app/_models/product';
import { AuthenticationService } from 'src/app/_services/authentication.service';
import { ProductService } from 'src/app/_services/product.service';
import { environment } from 'src/environments/environment';
import { calculatePreviewOffset } from 'src/app/_common/function/global';

@Component({
  selector: 'app-admin-product-photo',
  templateUrl: './admin-product-photo.component.html',
  styleUrls: ['./admin-product-photo.component.css'],
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

  maxPreviewItem: number = 6;

  leftOffset: number;

  rightOffset: number;

  constructor(
    private productService: ProductService,
    private authenticationService: AuthenticationService,
    private modalService: BsModalService
  ) {}

  ngOnInit(): void {
    this.loadProductDetail(this.productService.getSelectedProductId());
    this.initializeUploader();
    this.modalAction = '';
    this.leftOffset = 0;
    this.rightOffset = 0;
    this.multipleSelected = true;
  }

  loadProductDetail(productId: number) {
    this.productService.getManagerProduct(productId).subscribe((result) => {
      this.product = result;
      this.productPhotos = this.product.productPhotos;
    });
  }

  initializeUploader() {
    let uploadUrl =
      this.baseUrl +
      'product/add-product-photo/' +
      this.productService.getSelectedProductId();
    this.uploader = new FileUploader({
      url: uploadUrl,
      authToken: 'Bearer ' + this.authenticationService.userValue.jwtToken,
      isHTML5: true,
      allowedFileType: ['image', 'video'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 30 * 1024 * 1024,
    });

    this.uploader.onBeforeUploadItem = (file) => {
      if (file.file.type == 'video/mp4')
        this.uploader.setOptions({
          url: uploadUrl.replace('add-product-photo', 'add-product-video'),
        });
    };

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = true;
    };
    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const photo: ManagerProductPhoto = JSON.parse(response);
        this.productPhotos.push(photo);

        //remove in production
        this.productService.removeProductCache(this.product.id);
        //
      }
    };
  }

  fileOverBase(e: any) {
    this.hasBaseDropzoneOver = e;
  }

  openPhotosModal(product: ManagerProduct, action: string) {
    let actionTitle = '';
    this.modalAction = action;
    switch (this.modalAction) {
      case 'hide':
        actionTitle = 'Hide photos of ' + product.productName;
        this.multipleSelected = true;
        break;
      case 'unhide':
        actionTitle = 'Unhide photos of ' + product.productName;
        this.multipleSelected = true;
        break;
      case 'delete':
        actionTitle = 'Recover photos of ' + product.productName;
        this.multipleSelected = true;
        break;
      default:
        actionTitle = 'Choose main photo of ' + product.productName;
        this.multipleSelected = false;
        break;
    }
    const config = {
      class: 'modal-dialog-centered',
      initialState: {
        product,
        productPhotos: this.getHidePhotosArray(product),
        action: actionTitle,
        multiple: this.multipleSelected,
      },
    };
    this.bsModalRef = this.modalService.show(
      ProductImageModalComponent,
      config
    );
    this.bsModalRef.setClass('modal-lg');
    this.bsModalRef.content.updateSelectedPhotos.subscribe((values) => {
      const photosToUpdate = {
        photoIds: [
          ...values.filter((el) => el.checked === true).map((el) => el.id),
        ],
      };
      if (photosToUpdate) {
        let selectedIds: IdArray = {
          ids: photosToUpdate.photoIds,
        };
        switch (action) {
          case 'hide':
            this.productService.hideProductImage(selectedIds).subscribe(() => {
              this.loadProductDetail(
                this.productService.getSelectedProductId()
              );
            });
            break;
          case 'unhide':
            this.productService
              .unhideProductImage(selectedIds)
              .subscribe(() => {
                this.loadProductDetail(
                  this.productService.getSelectedProductId()
                );
              });
            break;
          case 'delete':
            this.productService
              .deleteProductImage(selectedIds.ids)
              .subscribe(() => {
                this.loadProductDetail(
                  this.productService.getSelectedProductId()
                );
              });
            break;
          default:
            this.productService
              .setMainProductImage(this.product.id, photosToUpdate.photoIds[0])
              .subscribe(() => {
                this.loadProductDetail(
                  this.productService.getSelectedProductId()
                );
              });
            break;
        }
      }
    });
  }

  viewPhoto(productPhoto: ManagerProductPhoto) {
    let index = this.productPhotos
      .map((el) => el.url)
      .indexOf(productPhoto.url);
    var previewOffset = calculatePreviewOffset(
      this.productPhotos.length,
      this.maxPreviewItem,
      index
    );

    const photoItems = [];
    this.product.productPhotos.forEach((productPhoto) => {
      photoItems.push(productPhoto);
    });
    
    const config = {
      class: 'modal-dialog-centered',
      initialState: {
        viewerItems: photoItems,
        action: productPhoto.url.split('/').pop(),
        currentItem: photoItems[index],
        maxPreviewItem: this.maxPreviewItem,
        leftOffset: previewOffset[0],
        rightOffset: previewOffset[1],
      },
    };
    this.bsModalRef = this.modalService.show(ImageViewerModalComponent, config);
    this.bsModalRef.setClass('modal-lg');
  }

  private getHidePhotosArray(product) {
    const productPhotos = [];

    product.productPhotos.forEach((productPhoto) => {
      switch (this.modalAction) {
        case 'hide':
          if (!productPhoto.isMain && productPhoto.status == 0) {
            productPhoto.checked = false;
            productPhotos.push(productPhoto);
          }
          break;
        case 'unhide':
          if (!productPhoto.isMain && productPhoto.status == 1) {
            productPhoto.checked = false;
            productPhotos.push(productPhoto);
          }
          break;
        case 'delete':
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

  getStateStyle() {
    switch (this.product.status) {
      case 0:
        return 'width: 100px ;background-color: rgb(51, 155, 51)';
      case 1:
        return 'width: 100px ;background-color: rgb(124, 124, 124)';
      default:
        return 'width: 100px ;background-color: rgb(155, 51, 51)';
    }
  }

  getProductState() {
    switch (this.product.status) {
      case 0:
        return 'Active';
      case 1:
        return 'Hidden';
      default:
        return 'Deleted';
    }
  }

  // private calculatePreviewThumbnail(index: number) {
  //   this.leftOffset = 0;
  //   this.rightOffset = 0;
  //   index++;

  //   if (this.productPhotos.length < this.maxPreviewItem) {
  //     this.leftOffset = 0;
  //     this.rightOffset = this.maxPreviewItem;
  //     return;
  //   }

  //   if (index <= this.maxPreviewItem) {
  //     this.leftOffset = 0;
  //     this.rightOffset = this.maxPreviewItem;
  //     return;
  //   }

  //   if (index > this.productPhotos.length - this.maxPreviewItem) {
  //     this.leftOffset = this.productPhotos.length - this.maxPreviewItem;
  //     this.rightOffset = this.productPhotos.length;
  //     return;
  //   }

  //   if (this.maxPreviewItem % 2 == 0) {
  //     this.leftOffset = index - this.maxPreviewItem / 2;
  //     this.rightOffset = this.leftOffset + this.maxPreviewItem + 1;
  //   } else {
  //     this.leftOffset = index - this.maxPreviewItem / 2;
  //     this.rightOffset = this.leftOffset + this.maxPreviewItem;
  //   }
  // }
}
