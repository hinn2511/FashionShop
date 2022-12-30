export class GenericObject {
  id: number;
  name: string;

  constructor(id: number, name: string) {
    this.id = id;
    this.name = name;
  }
}

export class ResponseMessage
{

  constructor(isSuccess: boolean, httpStatusCode: number,  message: string)
    {
        this.isSuccess = isSuccess;
        this.status = httpStatusCode;
        this.message = message ;
    }

    status: number;
    message: string;
    isSuccess : boolean; 
}

export class GenericStatus extends GenericObject {
  constructor(id: number, name: string) {
    super(id, name);
  }
}

export const GenericStatusList: GenericStatus[] = [
  new GenericStatus(0, 'Active'),
  new GenericStatus(1, 'Hidden'),
  new GenericStatus(2, 'Deleted'),
];
