import {
  Directive,
  Input,
  OnInit,
  TemplateRef,
  ViewContainerRef,
} from '@angular/core';
import { User } from '../_models/user';
import { AuthenticationService } from '../_services/authentication.service';

@Directive({
  selector: '[notInRoles]',
})
export class NotInRolesDirective implements OnInit {
  @Input() notInRoles: string[];
  user: User;

  constructor(
    private viewContainerRef: ViewContainerRef,
    private templateRef: TemplateRef<any>,
    private authenticationService: AuthenticationService
  ) {}

  ngOnInit(): void {
    this.user = this.authenticationService.userValue;
    if (this.user?.roles.some((r) => this.notInRoles.includes(r)))
      this.viewContainerRef.clear();
    else this.viewContainerRef.createEmbeddedView(this.templateRef);
  }
}
