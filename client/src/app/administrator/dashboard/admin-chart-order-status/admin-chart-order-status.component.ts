import { Component, OnInit } from '@angular/core';
import { ChartData, ChartFilter, fnGetChartTimeFilterList, ChartParams } from 'src/app/_models/chart';
import { DashboardService } from 'src/app/_services/dashboard.service';

@Component({
  selector: 'app-admin-chart-order-status',
  templateUrl: './admin-chart-order-status.component.html',
  styleUrls: ['./admin-chart-order-status.component.css']
})
export class AdminChartOrderStatusComponent implements OnInit {

  data: ChartData[];

  showTimeFilter: boolean = false;
  showLegend: boolean = true;
  showLabels: boolean = true;
  isDoughnut: boolean = false;
  legendPosition: string = 'right';
  legendTitle: string = "Status";

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
    let param = new ChartParams(this.selectedTimeFilter.from, this.selectedTimeFilter.to);
    this.dashboardService.getDashboardOrderStatusSummary(param).subscribe(result => {
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
