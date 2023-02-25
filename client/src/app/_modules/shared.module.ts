import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ModalModule } from 'ngx-bootstrap/modal';
import { PopoverModule } from 'ngx-bootstrap/popover';
import { defineLocale } from 'ngx-bootstrap/chronos'
import { AccordionModule } from 'ngx-bootstrap/accordion';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { viLocale } from 'ngx-bootstrap/locale';
import { RatingModule } from 'ngx-bootstrap/rating'; 
import { FileUploadModule } from 'ng2-file-upload';
import { ToastrModule } from 'ngx-toastr';
import { ColorPickerModule } from '@iplab/ngx-color-picker';
defineLocale('vi', viLocale);

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    BsDropdownModule.forRoot(),
    BsDatepickerModule.forRoot(),
    PaginationModule.forRoot(),
    ModalModule.forRoot(),
    PopoverModule.forRoot(),
    AccordionModule.forRoot(),
    TabsModule.forRoot(),
    RatingModule.forRoot(),
    FileUploadModule,
    ToastrModule.forRoot({
      timeOut: 3000,
      positionClass: 'toast-bottom-left',
      preventDuplicates: false,
    }),
    ColorPickerModule
  ], 
  exports: [
    BsDropdownModule,
    PaginationModule,
    PopoverModule,
    AccordionModule,
    TabsModule,
    RatingModule,
    BsDatepickerModule,
    ModalModule,
    FileUploadModule,
    ColorPickerModule
  ]
})

export class SharedModule { }
