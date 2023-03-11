import { Subscription } from 'rxjs';
import { RouteService } from 'src/app/_services/route.service';
import {
  Component,
  Input,
  OnInit,
  Output,
  EventEmitter,
  HostListener,
  OnDestroy,
} from '@angular/core';

@Component({
  selector: 'app-admin-sidebar',
  templateUrl: './admin-sidebar.component.html',
  styleUrls: ['./admin-sidebar.component.css']
})
export class AdminSidebarComponent implements OnInit, OnDestroy {
  @Input() currentState: string = 'in';
  @Output() newState = new EventEmitter<string>();

  state: string;

  routeSubscription$: Subscription;
  currentRoute: string = '';
  isLocked: boolean = false;

  @HostListener('click', ['$event'])
  clickInside($event) {
    $event.stopPropagation();
  }

  @HostListener('document:click', ['$event'])
  clickOutside() {
    this.collapse();
  }

  constructor(private routeService: RouteService) {}

  ngOnDestroy(): void {
    this.routeSubscription$.unsubscribe();
  }

  ngOnInit(): void {
    this.state = 'in';
    this.routeSubscribe();
  }

  private routeSubscribe() {
    this.routeSubscription$ = this.routeService.route$.subscribe((result) => {
      this.currentRoute = result;
    });
  }

  toggle() {
    if(this.isLocked)
      return;
    this.state = this.state === 'in' ? 'out' : 'in';
    this.newState.emit(this.state);
  }

  expand() {
    if(this.isLocked)
      return;
    this.newState.emit('out');
  }

  collapse() {
    if(this.isLocked)
      return;
    this.newState.emit('in');
  }

  lockToggle()
  {
    this.isLocked = !this.isLocked;
  }
}
