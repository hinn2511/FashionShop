import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Gender, GenderList } from 'src/app/_models/category';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.css']
})
export class FooterComponent implements OnInit {
  genders: Gender[] = GenderList;
  constructor(private router: Router) { }

  ngOnInit(): void {
  }

  private navigateToGenderSummary(
    gender: number
  ) {
    this.router.navigate(['/categories'], {
      queryParams: { gender: gender },
    });
  }

}
