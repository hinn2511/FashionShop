import { FadeInAndOut, SlideRightToLeft } from './../../_common/animation/common.animation';
import { CustomerCatalogue, GenderList } from '../../_models/category';
import { Subscription } from 'rxjs';
import {
  Component,
  OnInit,
  OnDestroy,
  ElementRef,
  QueryList,
  ViewChildren,
  AfterViewInit,
} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CategoryService } from 'src/app/_services/category.service';
import { fnGetGenderName } from 'src/app/_models/category';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-category-summary',
  templateUrl: './category-summary.component.html',
  styleUrls: ['./category-summary.component.css'],
  animations: [FadeInAndOut, SlideRightToLeft]
})
export class CategorySummaryComponent
  implements OnInit, OnDestroy, AfterViewInit
{
  querySubscribe$: Subscription;
  selectedGender: number;
  selectedGenderString: string;

  @ViewChildren('genderTitle') items: QueryList<ElementRef>;
  genderTitles: ElementRef[] = [];

  categoryGroups: CustomerCatalogue[];
  constructor(
    private categoryService: CategoryService,
    private route: ActivatedRoute
  ) {}
  ngAfterViewInit(): void {
    this.items.changes.pipe(take(1)).subscribe((i) => {
      this.genderTitles = i.toArray();
    });
  }

  ngOnDestroy(): void {
    this.querySubscribe$.unsubscribe();
  }

  ngOnInit(): void {
    this.querySubscribe$ = this.route.queryParamMap.subscribe(
      (queryParamMap) => {
        this.selectedGender = +queryParamMap.get('gender');
        this.selectedGenderString = fnGetGenderName(this.selectedGender);
        this.loadCategoryGroup();
      }
    );
  }
  loadCategoryGroup() {
    if (this.categoryGroups != undefined) {
      this.scrollToGender();
    } else {
      this.categoryService.getCatalogue().subscribe((result) => {
        this.categoryGroups = result;
        this.scrollToGender();
      });
    }
  }

  scrollToGender() {
    let genderIndex = GenderList.findIndex((x) => x.id == this.selectedGender);
    setTimeout(() => {
      this.genderTitles[genderIndex].nativeElement.scrollIntoView({
        behavior: 'smooth',
        block: 'center',
        // inline: 'top',
      });
    }, 300);
  }
}
