import { Component, OnInit } from '@angular/core';
import { ChartData, ChartParams, ChartFilter, fnGetChartTimeFilterList } from 'src/app/_models/chart';
import { DashboardService } from 'src/app/_services/dashboard.service';

@Component({
  selector: 'app-admin-chart-popular-product',
  templateUrl: './admin-chart-popular-product.component.html',
  styleUrls: ['./admin-chart-popular-product.component.css']
})
export class AdminChartPopularProductComponent implements OnInit {

  data: ChartData[];

  limit = 10;

  showTimeFilter: boolean = false;
  showXAxis = true;
  showYAxis = true;
  gradient = false;
  showLegend = false;
  showXAxisLabel = true;
  xAxisLabel = 'Product';
  showYAxisLabel = true;
  yAxisLabel = 'Sold';

  timeFilters: ChartFilter[];
  selectedTimeFilter: ChartFilter;
  

  colorScheme = {
    domain: ['#83e6c0', '#3751c7', '#d9e655', '#29ace9', '#e929af']
  };
  
  constructor(private dashboardService: DashboardService) { }

  ngOnInit(): void {
    this.timeFilters = fnGetChartTimeFilterList();
    this.selectedTimeFilter = this.timeFilters[0];
    this.loadChartData();   
  }


  loadChartData() {
    let param = new ChartParams(this.selectedTimeFilter.from, this.selectedTimeFilter.to, this.limit);
    this.dashboardService.getDashboardPopularProduct(param).subscribe(result => {
      this.data = result;
    })
  }

  selectTimeFilter(time: ChartFilter)
  {
    this.selectedTimeFilter = time;
    this.timeFilterToggle();
    this.loadChartData();
  }

  timeFilterToggle()
  {
    this.showTimeFilter = !this.showTimeFilter;
  }

}
