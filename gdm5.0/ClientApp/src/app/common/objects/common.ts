import { Observable } from "rxjs";

export interface IGdmLabels{
    // Global menu
    globalMenu_Home:string;
    globalMenu_Product:string;
    globalMenu_Modeling:string;
    globalMenu_Order:string;
    globalMenu_Account:string;
    globalMenu_SingUP:string;
    globalMenu_Logout:string;
    
    // Product command
    productCommand_Product: string;
    productCommand_AddProduct: string;
    productCommand_AddNewProduct: string;
    productCommand_EditProduct: string;

    // Title product cart
    titleProductCart_Product:string;
    subTitleProductCart_AddProduct:string;
    subTitleProductCart_AddNewProduct:string;
    subTitleProductCart_EditParametersProduct:string;
    
    // Lables of product buttons
    productButton_SaveInstanceProduct:string;
    productButton_SaveNewProduct:string;
    productButton_SaveChangesOfProduct:string;

    // Filter panel
    filterPanel_Title:string;
    filterPanel_Reset:string;

    // Side panel
    sidePanel_Search:string;
    
    // Product action menu
    productActionMenu_Order:string;
    productActionMenu_AddToCart:string;
    productActionMenu_Edit:string;
    productActionMenu_Delete:string;

    // Modeling title
    modelingTitle_Modeling:string;
    modelingTitle_AddCompany:string;
    modelingTitle_AddCurrency:string;
    modelingTitle_AddPriceList:string;
    modelingTitle_AddWarehouse:string;
    modelingTitle_Add:string;

    // Modeling labels of buttons
    modelingLabelsOfButtons_Modeling:string;
    modelingLabelsOfButtons_Create:string;
    modelingLabelsOfButtons_SaveCompany:string;
    modelingLabelsOfButtons_SaveCurrency:string;
    modelingLabelsOfButtons_SavePriceList:string;
    modelingLabelsOfButtons_SaveWarehouse:string;
    modelingLabelsOfButtons_Save:string;

    // Order
    orderComandButton_Cart:string;
    orderComandButton_OrderHistory:string;
    
    // Order title
    orderTitleCart_InfoTitle:string;
    orderTitleCart_NameCompany:string;
    orderTitleCart_TotaPrice:string;
    orderTitleCart_OrderCreatedByUser:string;
    orderTitleCart_OrderCurency:string;
    orderTitleCart_CartIsEmpty:string;

    // Order labels of buttons
    orderLablesOfButton_CancelOrder:string;
    orderLablesOfButton_SaveOrder:string;
    
    // Product popup title
    productActionPopUp_OrderProduct:string;
    productActionPopUp_AddToCart:string;
    productActionPopUp_UpdateProduct:string;
    productActionPopUp_DeleteProduct:string;
    
    // Product popup lables of button 
    productActionPopUpButton_QuickOrder:string;
    productActionPopUpButton_AddToCart:string;
    productActionPopUpButton_Update:string;
    productActionPopUpButton_Delete:string;
    productActionPopUpButton_Cancel:string;
    
    // Modeling popup title
    modelingActionPopUp_UpdateCompany:string;
    modelingActionPopUp_DeleteCompany:string;
    modelingActionPopUp_UpdateCurrency:string;
    modelingActionPopUp_DeleteCurrency:string;
    modelingActionPopUp_UpdateWarehouse:string;
    modelingActionPopUp_DeleteWarehouse:string;

    // Order popup title
    modelingActionPopUp_DeleteProductFromCart:string;
    modelingActionPopUp_DeleteOrder:string;

    // User registration
    userRegistration_UserRegistrationTitle:string;
    userRegistration_UserName:string;
    userRegistration_UserNameError:string;
    userRegistration_UserNameErrorLinght:string;
    userRegistration_Email:string;
    userRegistration_EmailError:string;
    userRegistration_Password:string;
    userRegistration_PasswordError:string;
    userRegistration_PasswordLenght:string;
    userRegistration_ConfirmPassword:string;
    userRegistration_ConfirmPasswordError:string;
    userRegistration_UserRole:string;
    userRegistration_UserRoleError:string;
    userRegistration_LableButton:string;

    // Log In
    logIn_LogInTitle:string;
    logIn_UserName:string;
    logIn_UserNameError:string;
    logIn_UserNameLenght:string;
    logIn_Password:string;
    logIn_PasswordError:string;
    logIn_UserNameErrorLenght:string;
    logIn_LableButton:string;
}

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
    parentName?:string;
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
    name:string
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
    Priority:number
}

export interface gridParameter{
    isParameter: boolean,
    value: string,
    name: string,
    priority:number
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
        public isNewProp?:boolean, public isDeletedProp?:boolean, public parentName?:string) {
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
                public category?:string,
                public parentName?:string
                ) {
        this.name = this.name.toLowerCase();
    }
}

export interface IExecuteCommand{
    command: string,
    displayName:string,
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

// DependProps
export interface IDependentProperties{
    metadataTypeName?:string,
    properties:Array<any>
}

export class ExecuteCommand implements IExecuteCommand {
    constructor(public command: string, public displayName:string, public icon:string, public type:string, public data: IDataExecuteCommand){        
    }
}

// Filter
export interface FilterParameters {
    IsParameter: boolean,
    ParameterName: string, 
    ParameterValue: string,
    Priority:number
};  

export interface ISelectorValue {
    name: string,
    value: string
};

export interface IOptionParameterValues {
    NameType: string,
    NameParameter: string,
    IsParameter: boolean,
    Priority:number,
    FilterParameters: Array<any>,
    Parameters:Array<IParameter>,
    ParameterValues:Array<ISelectorValue>,
};

export interface IParameterValues {
    NameType: string,
    NameParameter: string,
    Priority:number,
    ParameterValues:Array<ISelectorValue>,
    Parameters:Array<IParameter>
};

export interface ILocalState {
    UserProfile: IUserProfile;
    MetaData: any;
    CurrenciesRateToday: any;
    CurrenciesRateLast: any;
}

export interface IUserProfile {
    Name: string;
    Dictionary: string;
    Password: IEncryptedField;
    SessionID: IEncryptedField;
    UTCOffset: string;
}

export interface IEncryptedField {
    IsEncrypted: boolean;
    Value: string;
}

export interface IParameterSelectionValue extends  ISelectorValue{
    typeProductName: string,
};

export interface IDictionaryArray {
    key: string;
    valueArray: any[];
}





//export const siteURI = "https://garin87-001-site1.btempurl.com/"
export const siteURI = "https://localhost:44335/"
