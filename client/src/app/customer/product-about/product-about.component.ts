import { Component, Input, OnInit } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Component({
  selector: 'app-product-about',
  templateUrl: './product-about.component.html',
  styleUrls: ['./product-about.component.css']
})
export class ProductAboutComponent implements OnInit {
  @Input() description: string;

  comment: SafeHtml;
  constructor(private sanitizer: DomSanitizer) { }
  
  ngOnInit(): void {
    this.comment = this.sanitizer.bypassSecurityTrustHtml(this.description);
  }

}
