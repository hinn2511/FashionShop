export class SelectPhoto {
  id: number;
  url: string;
  isSelected: boolean;

  constructor(id: number, url: string, isSelected: boolean) {
    this.id = id;
    this.url = url;
    this.isSelected = isSelected;
  }
}

export interface Photo {
  id: number;
  url: string;
}
