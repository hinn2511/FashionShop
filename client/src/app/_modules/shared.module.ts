import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { PaginationModule } from 'ngx-bootstrap/pagination';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    BsDropdownModule.forRoot(),
    PaginationModule.forRoot(),
  ], 
  exports: [
    BsDropdownModule,
    PaginationModule
  ]
})

export class SharedModule { }
