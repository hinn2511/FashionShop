import { Component, OnInit } from '@angular/core';
import { ChartData, ChartParams, ChartFilter, fnGetChartTimeFilterList } from 'src/app/_models/chart';
import { DashboardService } from 'src/app/_services/dashboard.service';

@Component({
  selector: 'app-admin-chart-order-rate',
  templateUrl: './admin-chart-order-rate.component.html',
  styleUrls: ['./admin-chart-order-rate.component.css']
})
export class AdminChartOrderRateComponent implements OnInit {

  data: ChartData[];

  showTimeFilter: boolean = false;

  cardColor: string = '#232837';

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
    this.dashboardService.getDashboardOrderRate(param).subscribe(result => {
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

  public formattingValue(data)
  {
    
    return `${data.value} %`;
  }



}
