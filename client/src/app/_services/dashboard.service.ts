import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { ChartData, ChartMultipleData, ChartParams } from '../_models/chart';

@Injectable({
  providedIn: 'root',
})
export class DashboardService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {
  } 

  getDashboardOrderStatusSummary(chartParams: ChartParams) {
    let params = new HttpParams();
    
    params = params.append('from', chartParams.from.toISOString());
    params = params.append('to', chartParams.to.toISOString());
    
    return this.http.get<ChartData[]>(
      this.baseUrl + 'dashboard/order-status', {params: params});
  }

  getDashboardOrderRate(chartParams: ChartParams) {
    let params = new HttpParams();
    
    params = params.append('from', chartParams.from.toISOString());
    params = params.append('to', chartParams.to.toISOString());
    
    return this.http.get<ChartData[]>(
      this.baseUrl + 'dashboard/order-rate', {params: params});
  }

  getDashboardPopularProduct(chartParams: ChartParams) {
    let params = new HttpParams();
    
    params = params.append('from', chartParams.from.toISOString());
    params = params.append('to', chartParams.to.toISOString());
    params = params.append('limit', chartParams.limit);
    
    return this.http.get<ChartData[]>(
      this.baseUrl + 'dashboard/popular-product', {params: params});
  }

  getDashboardRevenue(chartParams: ChartParams) {
    let params = new HttpParams();
    
    params = params.append('metric', chartParams.metric);
    
    return this.http.get<ChartData[]>(
      this.baseUrl + 'dashboard/revenue', {params: params});
  }

 

  
}
