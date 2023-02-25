import { PhotoService } from './../../_services/photo.service';
import { Component, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Pagination } from 'src/app/_models/pagination';
import { Params } from 'src/app/_models/params';
import { SelectPhoto } from 'src/app/_models/photo';

@Component({
  selector: 'app-image-selector',
  templateUrl: './image-selector.component.html',
  styleUrls: ['./image-selector.component.css'],
})
export class ImageSelectorComponent implements OnInit {
  @Input() action: string;
  @Input() multiple: boolean;
  @Input() selectedIds: number[];
  pagination: Pagination;
  photoParams: Params;
  photos: SelectPhoto[] = [];
  selectAllPhoto: boolean;

  constructor(
    public bsModalRef: BsModalRef,
    private photoService: PhotoService
  ) {
    this.photoParams = this.photoService.getPhotoParams();
  }

  ngOnInit(): void {
    this.photoParams.pageSize = 6;
    this.loadPhotos();
  }

  loadPhotos() {
    this.photoService.setPhotoParams(this.photoParams);

    this.photoService
      .getManagerPhotos(this.photoParams)
      .subscribe((response) => {
        this.photos = response.result.map(x => new SelectPhoto(x.id, x.url, false));
        this.pagination = response.pagination;
      });
  }

  confirm() {
    this.bsModalRef.hide();
    this.selectAllPhoto = false;
  }

  select(index: number) {
    if (!this.multiple) {
      this.photos.forEach((x) => {
        x.isSelected = false;
      });
    }
    this.photos[index].isSelected = !this.photos[index].isSelected;
  }

  cancel() {
    this.photos = [];
    this.bsModalRef.hide();
  }

  selectAll() {
    if (this.selectAllPhoto) {
      this.photos.forEach((x) => {
        x.isSelected = false;
      });
    } else {
      this.photos.forEach((x) => {
        x.isSelected = true;
      });
    }

    this.selectAllPhoto = !this.selectAllPhoto;
  }

  pageChanged(event: any) {
    if (this.photoParams.pageNumber !== event.page) {
      this.photoParams.pageNumber = event.page;
      this.photoService.setPhotoParams(this.photoParams);
      this.loadPhotos();
    }
  }
}
