export interface ManagerCarouselDetail {
  imageUrl: string;
  title: string;
  description: string;
  navigationText: string;
  link: string;
  id: number;
  dateCreated: Date;
  createdByUserId: number;
  lastUpdated: Date;
  lastUpdatedByUserId: number;
  dateDeleted: Date;
  deletedByUserId: number;
  dateHidden: Date;
  hiddenByUserId: number;
  status: number;
}

export interface ManagerCarousel {
  id: number;
  status: number;
  title: string;
  description: string;

  link: string;
  imageUrl: string;
}

export class ManagerCarouselParams {
  pageNumber = 1;
  pageSize = 12;
  orderBy = 0;
  field = 'Id';
  query = '';
  carouselStatus = [0, 1];
}

export class Carousel {
  title: string;
  description: string;
  link: string;
  imageUrl: string;

  constructor(
    title: string,
    description: string,
    link: string,
    imageUrl: string
  ) {
    this.title = title;
      this.description = description;
      this.link = link;
      this.imageUrl = imageUrl;
  }
}
