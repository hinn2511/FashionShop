import { HostListener, Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class DeviceService {
  private width = new BehaviorSubject<number>(0);
  deviceWidth$ = this.width.asObservable();

  private height = new BehaviorSubject<number>(0);
  deviceHeight$ = this.height.asObservable();

  public setWidth(value: number) {
    this.width.next(value);
  }
  public get deviceWidth(): number {
    return this.width.getValue();
  }

  public setHeight(value: number) {
    this.height.next(value);
  }
  public get deviceHeight(): number {
    return this.height.getValue();
  }

  constructor() {
    //Not implemented
  }

  getDeviceType()
  {
    if (this.deviceWidth <= 501)
    {
      if (this.deviceWidth < this.deviceHeight)
        return 'mobile';
      if (this.deviceWidth >= this.deviceHeight)
        return 'tablet';
    }   
    if (this.deviceWidth > 501 && this.deviceWidth < 992)
      return 'tablet';
    if (this.deviceWidth >= 992)
      return 'desktop';
  }

  isDeviceIncluded(deviceList: string[])
  {
    if (deviceList.indexOf(this.getDeviceType()) > -1)
      return true;
    return false;
  }

 
}
