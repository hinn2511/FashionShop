import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { User } from '../_models/user';
import { AuthenticationService } from '../_services/authentication.service';

@Directive({
  selector: '[appHasRole]'
})
export class HasRoleDirective implements OnInit {
  @Input() appHasRole: string[];
  user: User;

  constructor(private viewContainerRef: ViewContainerRef,
    private templateRef: TemplateRef<any>,
    private authenticationService: AuthenticationService) {
    this.user = this.authenticationService.userValue;
  }

  ngOnInit(): void {
    
    if (!this.user?.roles || this.user == null) {
      if (this.appHasRole.includes("Anonymous")) 
      {
        
        this.viewContainerRef.createEmbeddedView(this.templateRef);
        return;
      }
      else {
        

        this.viewContainerRef.clear();
        return;
      }
    }

    if (this.user?.roles.some(r => this.appHasRole.includes(r))) {
      this.viewContainerRef.createEmbeddedView(this.templateRef);
      
    }
    else {
      
      this.viewContainerRef.clear();
    }
  }

}
