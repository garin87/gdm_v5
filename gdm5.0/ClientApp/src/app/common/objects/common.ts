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
    
    required?:boolean;
    readOnly?:boolean;
    hidden?:boolean;
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
}
export interface IPaginationAction
{  
    paginationEventName?:string;
    gridName?:string;
    pageSize?:number;
    pageNumber?:number;
}

export interface IGridColumnDefinition {
    columnDef: string;
    header: string;
    cell:any;
    isSortable: boolean
}

export interface SortOptions{
    name:string;
    direction:string;
    isParameter:boolean;
}

export class valueUpdatedData {
    constructor(public propertyName:string, public value: any, public ValueType?:string,
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
                ) {
        this.name = this.name.toLowerCase();
    }
}

export const siteURI = "https://localhost:44335/"