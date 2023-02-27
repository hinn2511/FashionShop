import { Component, OnInit } from '@angular/core';
import { ChartParams, ChartMultipleData, ChartData } from 'src/app/_models/chart';
import { DashboardService } from 'src/app/_services/dashboard.service';

@Component({
  selector: 'app-admin-chart-revenue',
  templateUrl: './admin-chart-revenue.component.html',
  styleUrls: ['./admin-chart-revenue.component.css']
})
export class AdminChartRevenueComponent implements OnInit {

  data: ChartData[];

  now: Date;

  showMetricFilter: boolean = false;
  showXAxis = true;
  showYAxis = true;
  gradient = false;
  showLegend = false;
  showXAxisLabel = true;
  xAxisLabel = 'Day';
  showYAxisLabel = true;
  yAxisLabel = 'Revenue';


  metricFilters: string[] = ["day", "month", "quarter"];
  selectedMetricFilter: string;
  

  colorScheme = {
    domain: ['#83e6c0', '#3751c7', '#d9e655', '#29ace9', '#e929af']
  };
  
  constructor(private dashboardService: DashboardService) { }

  ngOnInit(): void {
    this.now = new Date;
    this.selectedMetricFilter = this.metricFilters[0];
    this.loadChartData();   
  }


  loadChartData() {
    let param = new ChartParams(new Date(), new Date(), 0, this.selectedMetricFilter);
    this.dashboardService.getDashboardRevenue(param).subscribe(result => {
      this.data = result;
    })
  }

  selectMetricFilter(metric: string)
  {
    this.selectedMetricFilter = metric;
    this.xAxisLabel = metric.charAt(0).toUpperCase() + metric.slice(1);
    this.metricFilterToggle();
    this.loadChartData();
  }

  metricFilterToggle()
  {
    this.showMetricFilter = !this.showMetricFilter;
  }

}
