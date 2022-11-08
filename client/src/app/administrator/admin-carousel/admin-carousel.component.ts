import { CarouselPreviewComponent } from '../../_common/carousel-preview/carousel-preview.component';
import { ContentService } from 'src/app/_services/content.service';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticationService } from 'src/app/_services/authentication.service';
import { environment } from 'src/environments/environment';
import { FileUploader } from 'ng2-file-upload';
import { Pagination } from 'src/app/_models/pagination';
import { ManagerCarousel, ManagerCarouselParams } from 'src/app/_models/carousel';
import { IdArray } from 'src/app/_models/adminRequest';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-admin-carousel',
  templateUrl: './admin-carousel.component.html',
  styleUrls: ['./admin-carousel.component.css'],
  animations: [
    trigger('rotatedState', [
        state('default', style({ transform: 'rotate(0)' })),
        state('rotated', style({ transform: 'rotate(-180deg)' })),
        transition('rotated => default', animate('500ms ease-out')),
        transition('default => rotated', animate('500ms ease-in'))
      ])
  ]
})
export class AdminCarouselComponent implements OnInit {

  carousels: ManagerCarousel[];
  pagination: Pagination;
  carouselParams: ManagerCarouselParams;
  selectAllCarousel: boolean;
  state: string = 'default';
  showStatusFilter: boolean;
  bsModalRef: BsModalRef;

  selectedIds: number[] = [];
  query: string;
  uploader: FileUploader;
  baseUrl = environment.apiUrl;

  constructor(
    private contentService: ContentService,
    private router: Router,
    private authenticationService: AuthenticationService,
    private modalService: BsModalService
  ) {
    this.carouselParams = this.contentService.getManagerCarouselParams();
  }

  ngOnInit(): void {
    this.carouselParams.field = 'Id';
    this.carouselParams.orderBy = 0;
    this.carouselParams.carouselStatus = [0, 1];
    this.selectAllCarousel = false;
    this.showStatusFilter = false;
    this.loadCarousels();
  }

  rotate() {
    this.state = (this.state === 'default' ? 'rotated' : 'default');
  }

  loadCarousels() {
    this.contentService.setManagerCarouselParams(this.carouselParams);

    this.contentService
      .getManagerCarousels(this.carouselParams)
      .subscribe((response) => {
        this.carousels = response.result;
        this.pagination = response.pagination;
        
      });
  }

  resetSelectedIds() {
    this.selectedIds = [];
  }

  // viewDetail(carouselId: number) {
  //   // this.contentService.setSelectedCarouselId(carouselId);
  //   this.router.navigateByUrl(
  //     '/administrator/carousel-manager/detail/' + carouselId
  //   );
  // }

  editCarousel() {
    if (!this.isSingleSelected()) return;
    // this.contentService.setSelectedCarouselId(this.selectedIds[0]);
    this.router.navigateByUrl(
      '/administrator/carousel-manager/edit/' + this.selectedIds[0]
    );
  }

  hideCarousels() {
    if (!this.isMultipleSelected()) return;
    let ids: IdArray = {
      ids: this.selectedIds,
    };

    this.contentService.hideCarousels(ids).subscribe((result) => {
      this.loadCarousels();
      this.resetSelectedIds();
    });
  }

  deleteCarousels() {
    if (!this.isMultipleSelected()) return;
    this.contentService.deleteCarousel(this.selectedIds).subscribe((result) => {
      this.loadCarousels();
      this.resetSelectedIds();
    });
  }

  pageChanged(event: any) {
    if (this.carouselParams.pageNumber !== event.page) {
      this.carouselParams.pageNumber = event.page;
      this.contentService.setManagerCarouselParams(this.carouselParams);
      this.loadCarousels();
    }
  }

  sort(type: number) {
    this.carouselParams.orderBy = type;
    this.loadCarousels();
  }

  filter(params: ManagerCarouselParams) {
    this.carouselParams = params;
    this.loadCarousels();
  }

  resetFilter() {
    this.carouselParams = this.contentService.resetManagerCarouselParams();
    this.loadCarousels();
  }

  orderBy(field: string) {
    switch (field) {
      case 'id':
        this.carouselParams.field = 'Id';
        break;
      case 'status':
        this.carouselParams.field = 'Status';
        break;
        case 'title':
          this.carouselParams.field = 'Title';
          break;
          case 'link':
        this.carouselParams.field = 'Link';
        break;
      default:
        this.carouselParams.field = 'Date';
        break;
    }
    if (this.carouselParams.orderBy == 0) this.carouselParams.orderBy = 1;
    else this.carouselParams.orderBy = 0;
    this.rotate();
    this.loadCarousels();
  }

  selectAllCarousels() {
    if (this.selectAllCarousel) {
      this.selectedIds = [];
    } else {
      this.selectedIds = this.carousels.map(({ id }) => id);
    }
    this.selectAllCarousel = !this.selectAllCarousel;
  }

  selectCarousel(id: number) {
    if (this.selectedIds.includes(id)) {
      this.selectedIds.splice(this.selectedIds.indexOf(id), 1);
    } else {
      this.selectedIds.push(id);
    }
  }

  isCarouselSelected(id: number) {
    if (this.selectedIds.indexOf(id) >= 0) return true;
    return false;
  }

  getCarouselState(carousel: ManagerCarousel) {
    switch (carousel.status) {
      case 0:
        return 'Active';
      case 1:
        return 'Hidden';
      default:
        return 'Deleted';
    }
  }

  getStateStyle(carousel: ManagerCarousel) {
    switch (carousel.status) {
      case 0:
        return 'width: 100px ;background-color: rgb(51, 155, 51)';
      case 1:
        return 'width: 100px ;background-color: rgb(124, 124, 124)';
      default:
        return 'width: 100px ;background-color: rgb(155, 51, 51)';
    }
  }

  // isStatusIncluded(status: number) {
  //   return this.carouselParams.carouselStatus.indexOf(status) > -1;
  // }

  isAllStatusIncluded() {
    return this.carouselParams.carouselStatus.length == 3;
  }

  isStatusIncluded(status: number) {
    return this.carouselParams.carouselStatus.indexOf(status) > -1;
  }

  selectStatus(status: number) {
    if (this.isStatusIncluded(status))
      this.carouselParams.carouselStatus =
        this.carouselParams.carouselStatus.filter((x) => x !== status);
    else this.carouselParams.carouselStatus.push(status);
    this.carouselParams.carouselStatus = this.carouselParams.carouselStatus.sort();
    this.loadCarousels();
  }

  selectAllCarouselStatus() {
    if(this.isAllStatusIncluded())
      this.carouselParams.carouselStatus = [];
    else
      this.carouselParams.carouselStatus = [0, 1, 2];
    this.loadCarousels();
  }

  statusFilterToggle() {
    this.showStatusFilter = !this.showStatusFilter;
  }

  isSingleSelected() {
    return this.selectedIds.length == 1;
  }

  isMultipleSelected() {
    return this.selectedIds.length >= 1;
  }

  showPreview(carousel: ManagerCarousel) {
    const config = {
      class: 'modal-dialog-centered',
      initialState: {
        carousel: carousel
      },
    };
    this.bsModalRef = this.modalService.show(CarouselPreviewComponent, config);
    this.bsModalRef.setClass('modal-xl');
  }
}
