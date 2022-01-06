export class Alert{
  time:number;
  modal:boolean;
  title:string;

  constructor(public type : AlertTypes, public message: string, data?:any ){
      this.time = Date.now();
  }

  get class():string {
     return ' alert alert-' + AlertTypes[this.type] + ' alert-dismissible';
  }
}

export enum AlertTypes {info, danger, warning, success, choice}