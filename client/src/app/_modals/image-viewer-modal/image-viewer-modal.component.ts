import { viLocale } from 'ngx-bootstrap/locale';
import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { PhotoViewerItem } from 'src/app/_models/product';

@Component({
  selector: 'app-image-viewer-modal',
  templateUrl: './image-viewer-modal.component.html',
  styleUrls: ['./image-viewer-modal.component.css'],
})
export class ImageViewerModalComponent implements OnInit {
  // @Input() updateSelectedPhotos = new EventEmitter();
  // @Input() action: string;
  // url: string = "";
  // constructor(public bsModalRef: BsModalRef) { }

  // ngOnInit(): void {
  // }

  @Input() ImageList = new EventEmitter();
  @Input() action: string;
  viewerItems: PhotoViewerItem[] = [];
  previewItems: PhotoViewerItem[] = [];
  currentItem: PhotoViewerItem;
  maxPreviewItem: number;
  leftOffset: number;
  rightOffset: number;
  previewItemWidth: string;

  constructor(public bsModalRef: BsModalRef) {}

  ngOnInit(): void {
    this.preparePreview();
    console.log(this.currentItem);
    
  }

  private preparePreview() {
    this.previewItems = this.viewerItems.slice(
      this.leftOffset,
      this.rightOffset
    );
  }

  selectPhoto(item: PhotoViewerItem) {
    this.action = item.url;
    this.currentItem = item;
  }

  isSelected(item: PhotoViewerItem) {
    return item.url === this.currentItem.url;
  }

  getPreviewItemWidth() {
    return 'width: calc(' + 100 / this.maxPreviewItem + '% - 1px);';
  }

  onPreviousClick() {
    let curIndex = this.viewerItems.indexOf(this.currentItem);
    if ( curIndex > 0) {
      this.currentItem = this.viewerItems[--curIndex];
      if (curIndex > this.maxPreviewItem)
      {
        this.leftOffset--;
        this.rightOffset--;
      }
    }
    this.preparePreview();
  }

  onNextClick() {
    let curIndex = this.viewerItems.indexOf(this.currentItem);
    if ( curIndex < this.viewerItems.length - 1) {
      this.currentItem = this.viewerItems[++curIndex];
      if (this.viewerItems.length > this.maxPreviewItem)
      {
        this.leftOffset++;
        this.rightOffset++;
      }
    }
    this.preparePreview();
  }
}
