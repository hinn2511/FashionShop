import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ProgressbarModule } from 'ngx-bootstrap/progressbar';
import { CarouselModule } from 'ngx-bootstrap/carousel';
import { viLocale } from 'ngx-bootstrap/locale';
import { defineLocale } from 'ngx-bootstrap/chronos';
import { FileUploadModule } from 'ng2-file-upload';
defineLocale('vi', viLocale);

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    BsDropdownModule.forRoot(),
    BsDatepickerModule.forRoot(),
    PaginationModule.forRoot(),
    ProgressbarModule.forRoot(),
    CarouselModule.forRoot(),
    FileUploadModule
  ], 
  exports: [
    BsDropdownModule,
    PaginationModule,
    BsDatepickerModule,
    ProgressbarModule,
    CarouselModule,
    FileUploadModule
  ]
})

export class SharedModule { }
