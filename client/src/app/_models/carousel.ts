export interface ManagerCarouselDetail {
    imageUrl: string;
    title: string;
    description: string;
    navigationText: string;
    link: string;
    textPosition: number;
    textPaddingLeft: number;
    textPaddingRight: number;
    textPaddingTop: number;
    textPaddingBottom: number;
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
    navigationText: string;
    link: string;
    imageUrl: string;
    textPosition: number;
    textPaddingLeft: number;
    textPaddingRight: number;
    textPaddingTop: number;
    textPaddingBottom: number;
}

export class ManagerCarouselParams {
    pageNumber = 1;
    pageSize = 12;
    orderBy = 0;
    field = "Id";
    query = "";
    carouselStatus = [0, 1];
}

export interface CustomerCarousel {
    title: string;
    description: string;
    navigationText: string;
    link: string;
    imageUrl: string;
    textPosition: number;
    textPaddingLeft: number;
    textPaddingRight: number;
    textPaddingTop: number;
    textPaddingBottom: number;
}

export interface TextPosition
{
  value: number;
  position: string;
}

export const AvailableTextPositions: TextPosition[] = [
  {
    value: 0,
    position: 'Top left',
  },
  {
    value: 1,
    position: 'Top center',
  },
  {
    value: 2,
    position: 'Top right',
  },
  {
    value: 3,
    position: 'Middle left',
  },
  {
    value: 4,
    position: 'Middle center',
  },
  {
    value: 5,
    position: 'Middle right',
  },
  {
    value: 6,
    position: 'Bottom left',
  },
  {
    value: 7,
    position: 'Bottom center',
  },
  {
    value: 8,
    position: 'Bottom right',
  },
];
