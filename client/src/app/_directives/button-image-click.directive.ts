import { Directive, ElementRef, HostListener, Input } from '@angular/core';

@Directive({
  selector: '[ButtonImageClick]'
})
export class ButtonImageClickDirective {
  @Input() currentPositon: number;
  @Input() size: number;
  @Input() action: string;

  constructor(private el: ElementRef) { }

  @HostListener('click')
  nextImage() {
    if (this.action == "next" && this.currentPositon < this.size -1)
        this.currentPositon++;
    if (this.action == "previous" && this.currentPositon > 0)
        this.currentPositon--;

    var imageSlide = document.getElementsByClassName("img-slide");

    for (let i = 0; i < imageSlide.length; i++) {
      imageSlide[i].classList.remove("active");
    }

    imageSlide[ this.currentPositon].classList.add("active");
    var prev: any = document.getElementById("preview");
    prev.src = imageSlide[ this.currentPositon].getElementsByTagName("img")[0].src;
  }
}
