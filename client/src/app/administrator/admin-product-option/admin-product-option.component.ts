import { RotateAnimation } from 'src/app/_common/animation/carousel.animations';
import { OptionService } from 'src/app/_services/option.service';
import { ManagerOption } from 'src/app/_models/productOptions';
import { Component, Input, OnInit } from '@angular/core';
import { Pagination } from 'src/app/_models/pagination';
import { ManagerOptionParams } from 'src/app/_models/productOptions';
import { Router } from '@angular/router';
import { IdArray } from 'src/app/_models/adminRequest';
import { ToastrService } from 'ngx-toastr';
import {
  fnGetObjectStateString,
  fnGetObjectStateStyle,
} from 'src/app/_common/function/style-class';
import { GenericStatus, GenericStatusList } from 'src/app/_models/generic';

@Component({
  selector: 'app-admin-product-option',
  templateUrl: './admin-product-option.component.html',
  styleUrls: ['./admin-product-option.component.css'],
  animations: [RotateAnimation],
})
export class AdminProductOptionComponent implements OnInit {
  @Input() minimize: boolean = false;
  options: ManagerOption[];
  pagination: Pagination;
  optionParams: ManagerOptionParams;
  isSelectAllOptionStatus: boolean;
  state: string = 'default';
  showStatusFilter: boolean;

  selectAllOption: boolean;

  selectedIds: number[] = [];
  query: string;
  genericStatus: GenericStatus[] = GenericStatusList;

  constructor(
    private optionService: OptionService,
    private router: Router,
    private toastr: ToastrService
  ) {
    this.optionParams = this.optionService.getOptionParams();
  }

  ngOnInit(): void {
    this.optionParams.productIds = [];
    this.selectAllOption = false;
    this.optionParams.field = 'Id';
    this.optionParams.orderBy = 0;
    this.optionParams.productOptionStatus = [0, 1];
    this.isSelectAllOptionStatus = false;
    this.showStatusFilter = false;
    this.loadOptions();
  }

  rotate() {
    this.state = this.state === 'default' ? 'rotated' : 'default';
  }

  loadOptions() {
    this.optionService.setOptionParams(this.optionParams);

    this.optionService
      .getManagerOptions(this.optionParams)
      .subscribe((response) => {
        this.options = response.result;
        this.pagination = response.pagination;
      });
  }

  filterByProductIds(productIds: number[]) {
    this.optionParams.productIds = productIds;
    this.loadOptions();
  }

  resetSelectedIds() {
    this.selectedIds = [];
  }

  viewDetail(productId: number) {
    this.optionService.setSelectedOptionId(productId);
    this.router.navigateByUrl(
      '/administrator/option-manager/detail/' + productId
    );
  }

  editOption() {
    if (!this.isSingleSelected()) return;
    this.optionService.setSelectedOptionId(this.selectedIds[0]);
    this.router.navigateByUrl(
      '/administrator/option-manager/edit/' + this.selectedIds[0]
    );
  }

  hideOptions() {
    if (!this.isMultipleSelected()) return;
    let ids: IdArray = {
      ids: this.selectedIds,
    };

    this.optionService.hideOptions(ids).subscribe(
      (result) => {
        this.loadOptions();
        this.resetSelectedIds();
        this.toastr.success(result.message, 'Success');
      },
      (error) => {
        this.toastr.error(error, 'Error');
      }
    );
  }

  activateOptions() {
    if (!this.isMultipleSelected()) return;
    let ids: IdArray = {
      ids: this.selectedIds,
    };

    this.optionService.activateOptions(ids).subscribe(
      (result) => {
        this.loadOptions();
        this.resetSelectedIds();
        this.toastr.success(result.message, 'Success');
      },
      (error) => {
        this.toastr.error(error, 'Error');
      }
    );
  }

  deleteOptions() {
    if (!this.isMultipleSelected()) return;
    this.optionService.deleteOption(this.selectedIds).subscribe(
      (result) => {
        this.loadOptions();
        this.resetSelectedIds();
        this.toastr.success('Product options have been deleted', 'Success');
      },
      (error) => {
        this.toastr.error('Something wrong happen!', 'Error');
      }
    );
  }

  pageChanged(event: any) {
    if (this.optionParams.pageNumber !== event.page) {
      this.optionParams.pageNumber = event.page;
      this.optionService.setOptionParams(this.optionParams);
      this.loadOptions();
    }
  }

  sort(type: number) {
    this.optionParams.orderBy = type;
    this.loadOptions();
  }

  filter(params: ManagerOptionParams) {
    this.optionParams = params;
    this.loadOptions();
  }

  resetFilter() {
    this.optionParams = this.optionService.resetOptionParams();
    this.loadOptions();
  }

  orderBy(field: string) {
    this.optionParams.field = field;
    if (this.optionParams.orderBy == 0) this.optionParams.orderBy = 1;
    else this.optionParams.orderBy = 0;
    this.rotate();
    this.loadOptions();
  }

  selectAllStatus() {
    if (this.isSelectAllOptionStatus) {
      this.selectedIds = [];
    } else {
      this.selectedIds = this.options.map(({ id }) => id);
    }
    this.isSelectAllOptionStatus = !this.isSelectAllOptionStatus;
  }

  selectOption(id: number) {
    if (this.selectedIds.includes(id)) {
      this.selectedIds.splice(this.selectedIds.indexOf(id), 1);
    } else {
      this.selectedIds.push(id);
    }
  }

  selectAllOptions() {
    if (this.selectAllOption) {
      this.selectedIds = [];
    } else {
      this.selectedIds = this.options.map(({ id }) => id);
    }
    this.selectAllOption = !this.selectAllOption;
  }

  isOptionSelected(id: number) {
    return this.selectedIds.indexOf(id) >= 0;
  }

  getOptionState(option: ManagerOption) {
    return fnGetObjectStateString(option.status);
  }

  getStateStyle(option: ManagerOption) {
    return fnGetObjectStateStyle(option.status);
  }

  getColor(option: ManagerOption) {
    return 'color: ' + option.colorCode;
  }

  isAllStatusIncluded() {
    return this.optionParams.productOptionStatus.length == 3;
  }

  isStatusIncluded(status: number) {
    return this.optionParams.productOptionStatus.indexOf(status) > -1;
  }

  selectStatus(status: number) {
    if (this.isStatusIncluded(status))
      this.optionParams.productOptionStatus =
        this.optionParams.productOptionStatus.filter((x) => x !== status);
    else this.optionParams.productOptionStatus.push(status);
    this.optionParams.productOptionStatus = [
      ...this.optionParams.productOptionStatus,
    ].sort((a, b) => a - b);
    this.loadOptions();
  }

  selectAllOptionStatus() {
    if (this.isAllStatusIncluded()) this.optionParams.productOptionStatus = [];
    else this.optionParams.productOptionStatus = [0, 1, 2];
    this.loadOptions();
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
}
