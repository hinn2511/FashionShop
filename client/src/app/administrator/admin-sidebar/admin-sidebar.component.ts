import { Component, Input, OnInit, Output, EventEmitter, HostListener } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-admin-sidebar',
  templateUrl: './admin-sidebar.component.html',
  styleUrls: ['./admin-sidebar.component.css']
})
export class AdminSidebarComponent implements OnInit {
  @Input() currentState: string = "in";
  @Output() newState = new EventEmitter<string>();


  @HostListener('click', ['$event'])
  clickInside($event) {
    $event.stopPropagation();
  }

  @HostListener('document:click', ['$event'])
  clickOutside() {
    this.collapse();
  }

  constructor(private router: Router) { }

  ngOnInit(): void {
    //Not implemented
  }

  hasRoute(route: string) {
    return this.router.url.includes(route);
  }

  toggle()
  {
    let output = this.currentState === 'in' ? "out" : "in";
    this.newState.emit(output)
  }

  expand()
  {
    this.newState.emit('out')
  }

  collapse()
  {
    this.newState.emit('in')
  }


}
