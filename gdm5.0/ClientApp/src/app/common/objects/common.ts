import { Observable } from "rxjs";

export interface UserForRegistrationDto {
    userName: string;
    email: string;
    password: string;
    confirmPassword: string;
    userRole: string;
}
export interface UserForLoginDto {
    username: string;
    password: string;
}


export interface RegistrationResponseDto {
    isSuccessfulRegistration: boolean;
    errros: string[];
}

export interface IExceptionDataType {
    Code: number;
    Description: string;
    FailureContext: string;
    Source: string;
}

export interface IResultStatus {
    IsSuccess: boolean;
    Message: string;
    ExceptionData?: IExceptionDataType;
}

export interface userRole {
    value: string;
    viewValue: string;
}

export interface ParameterForm {
    parameterName: string;
    parameterPriority: any;
}

export interface IMetadataProperty
{
    name:string;
    type:string;
    displayedName?:string;
    typeView?:string;
    editor?: string;
    provider?: string;
    defaultValue?:string;
    value?:string;
    accessor?:string;
    order?:number;
    navPriority?:number;
    selectableItems?: Observable<ISelectableItem[]>;
    isDeleted?:boolean,
    isParameter?:boolean,
    required?:boolean;
    readOnly?:boolean;
    hidden?:boolean;
    category?:string;
    
}

export interface IPropertyEditor
{   
    editorName:string;
    provider?:string;
    selectableItems?: Observable<ISelectableItem[]>;
}

export interface ISelectableItem
{
    name:string;
    value:any;

    type?:string;
    childCount?:number;
}
export interface IParameter
{
    id:number;
    value:string;
    nameType:string;
    priority:number;
}
export interface IPaginationAction
{  
    paginationEventName?:string;
    gridName?:string;
    pageSize?:number;
    pageNumber?:number;
}

export interface IGridColumnDefinition {
    columnDef:string;
    header:string;
    cell:any;
    isSortable:boolean,
    order:number
}

export interface ISortOption{
    name:string;
    direction:string;
    isParameter:boolean;
}

export interface IPageFilter{
    pageNumber : number,
    pageSize: number
} 

export interface IgetProductTypeInstancesRequest{
    NameProductType:string;
    SortOption:ISortOption,
    PageFilter:IPageFilter,
    Filter:IProductFilter
};

export interface IProductFilter{
    Name: string,
    ProductNumber: string,
    Manufacturer: string,
    Description: string,
    Quantity:number,
    PrimeCost:number,
    StandartCost:number,
    Parameters : ProductParameterFilter[] 
};
export interface IGetOrderInstancesRequest{
    Name:string;
    OrderId:number;
    SortOption:ISortOption,
    PageFilter:IPageFilter,
    Filter:IOrderFilter
};

export interface IOrderFilter{
    NameCompany: string,
    TotalPrice:number,
    Number:number,
    CustomerId:number,
    OrderCreatedTime:Date,
    FilterStartDate:Date,
    FilterEndtDate:Date,
}

export interface ProductParameterFilter{
    Value:string,
    ParameterName: string,
    ProductId: number,
    ParameterId: number,
}

export interface gridParameter{
    isParameter: boolean,
    value: string,
    name: string
}
export class SortOption implements ISortOption  {
    constructor() {}
    name: string;
    direction: string;
    isParameter: boolean;
}

export class ProductTypeInstancesRequest implements IgetProductTypeInstancesRequest  {
    constructor() {}
    NameProductType: string;
    SortOption: ISortOption = new SortOption();
    PageFilter: IPageFilter = new PageFilter();
    Filter: IProductFilter = new ProductFilter();
}

export class OrdersRequest implements IGetOrderInstancesRequest  {
    constructor() {}
    Name: string;
    OrderId: number;
    SortOption: ISortOption = new SortOption();
    PageFilter: IPageFilter = new PageFilter();
    Filter: IOrderFilter = new OrderFilter();
}

export class PageFilter implements IPageFilter  {
    constructor() {}
    pageNumber: number = 1;
    pageSize: number = 5;
}

export class ProductFilter implements IProductFilter  {
    constructor() {}
    Name: string;
    ProductNumber: string;
    Manufacturer: string;
    Description: string;
    Quantity: number;
    PrimeCost: number;
    StandartCost: number;
    Parameters: ProductParameterFilter[] = [];
}

export class OrderFilter implements IOrderFilter  {
    constructor() {}
    NameCompany: string;
    TotalPrice:number;
    Number:number;
    CustomerId:number;
    OrderCreatedTime:Date;
    FilterStartDate:Date;
    FilterEndtDate:Date;
}
export class OrderProducts {
    constructor() {}
    currency: string;
    description:string;
    orderCreatedByUser:string;
    orderCreatedTime:Date;
    orderId:number;
    orderNumber:number;
    parameters: OrderProductParameter[]
}

export interface OrderProductParameter{
    id: number,
    name:string,
    value: any,
    parameterId:number,
    priority:number
}

export interface CountCartProducts{
    countCartProduct:number,
    isEmptyCart:boolean
}

export class valueUpdatedData {
    constructor(public propertyName:string, public value: any, public ValueType?:string, public category?:string,
        public navPriority?:number, public propertyNewName?:string, public isEditedName?:boolean,
        public isNewProp?:boolean, public isDeletedProp?:boolean) {
        this.propertyName = this.propertyName.toLowerCase();
    }
}

export class parameterUpdatedData {
    constructor(public propertyName:string, public value: any, public propertyNewName?:string, public ValueType?:string,
        public navPriority?:number) {
        this.propertyName = this.propertyName.toLowerCase();
    }
}

export class ProductParameter  {
    constructor(public name:string, public id?:number, public value?: any) {
        this.name = this.name.toLowerCase();
    }
}

export class MetadataProperty implements IMetadataProperty  {
    constructor(public name:string, 
                public type:string, 
                public value?: any,
                public id?: number,
                public displayedName?: string,
                public typeView?: string,
                public editor?: string,
                public provider?: string,
                public defaultValue?:string,
                public accessor?:string,
                public order?:number,
                public isEditable?:boolean,
                public navPriority?:number,
                public required?:boolean,
                public readOnly?:boolean,
                public hidden?:boolean,
                public isDeleted?:boolean,
                public isParameter?:boolean,
                public isNewProp?:boolean,
                public category?:string
                ) {
        this.name = this.name.toLowerCase();
    }
}

export interface IExecuteCommand{
    command: string,
    icon:string,
    type:string,
    data: IDataExecuteCommand
}

export interface IDataExecuteCommand{
    actionName:string,
    actiontitle?:string,
    actionButton?: string,
    typeInstance?: string,
    parentType:string,
    data?:any
}

export class ExecuteCommand implements IExecuteCommand {
    constructor(public command: string, public icon:string, public type:string, public data: IDataExecuteCommand){        
    }
}

export const siteURI = "https://localhost:44335/"