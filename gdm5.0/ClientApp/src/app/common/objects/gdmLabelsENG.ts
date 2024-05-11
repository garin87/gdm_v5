import { IGdmLabels } from "./common";

export class gdmLabelsENG implements IGdmLabels  {
    constructor() {}
    // Global menu
    globalMenu_Home:string = "Home";
    globalMenu_Product:string = "Products";
    globalMenu_Modeling:string = "Modeling";
    globalMenu_Order:string = "Orders";
    globalMenu_Report:string = "Reports";
    globalMenu_Account:string = "Account";
    globalMenu_SingUP:string = "Sing up";
    globalMenu_Logout:string = "Logout";

    // Product command
    productCommand_Product: string = "Products";
    productCommand_AddProduct: string = "Add a product";
    productCommand_ReportProduct: string = "Product statistics";
    productCommand_AddNewProduct: string = "Add a new product";
    productCommand_EditProduct: string = "Edit product parameters";

    // Title product cart
    titleProductCart_Product:string = "Products";
    subTitleProductCart_AddProduct:string = "Add a instance of product";
    subTitleProductCart_AddNewProduct:string = "Add parameters for a new product";
    subTitleProductCart_EditParametersProduct:string = "You can add, delete or edit product parameters";
    
    // Lables of product buttons
    productButton_SaveInstanceProduct= "Save a instance product";
    productButton_SaveNewProduct= "Save the product";
    productButton_SaveChangesOfProduct= "Save changes";

    // Filter panel
    filterPanel_Title:string = "Filter data";
    filterPanel_Reset:string = "Reset";

    // Side panel
    sidePanel_Search:string = "Search";
    
    // Product action menu
    productActionMenu_Order:string = "Order";
    productActionMenu_AddToCart:string = "Add to cart";
    productActionMenu_Edit:string = "Edit";
    productActionMenu_Delete:string = "Delete";

    // Product popup title
    productActionPopUp_OrderProduct:string = "Order product";
    productActionPopUp_AddToCart:string = "Add to Cart";
    productActionPopUp_UpdateProduct:string = "Update the product";
    productActionPopUp_DeleteProduct:string = "Delete the product";
    
    // Product popup lables of button 
    productActionPopUpButton_QuickOrder:string = "Quick Order";
    productActionPopUpButton_AddToCart:string = "Add to Cart";
    productActionPopUpButton_Update:string = "Update";
    productActionPopUpButton_Delete:string = "Delete";
    productActionPopUpButton_Cancel:string = "Cancel";
    
    // Modeling popup title
    modelingActionPopUp_UpdateCompany:string = "Update company instance";
    modelingActionPopUp_DeleteCompany:string = "Delete the company";
    modelingActionPopUp_UpdateCurrency:string = "Update currency instance";
    modelingActionPopUp_DeleteCurrency:string = "Delete the currency";
    modelingActionPopUp_UpdateWarehouse:string = "Update warehouse instance";
    modelingActionPopUp_DeleteWarehouse:string = "Delete the warehouse";

    // Order popup title
    modelingActionPopUp_DeleteProductFromCart:string = "Delete product from cart";
    modelingActionPopUp_DeleteOrder:string = "Delete order";
    

    // Modeling title
    modelingTitle_Modeling:string = "Modeling";
    modelingTitle_AddCompany:string = "Add a company";
    modelingTitle_AddCurrency:string = "Add a currency";
    modelingTitle_AddPriceList:string = "Add a pricelist";
    modelingTitle_AddWarehouse:string = "Add a warehouse";
    modelingTitle_Add:string = "Add a";

    // Modeling labels of buttons
    modelingLabelsOfButtons_Modeling:string = "Modeling";
    modelingLabelsOfButtons_ModelingSub:string = "List";
    modelingLabelsOfButtons_Create:string = "Create";
    modelingLabelsOfButtons_SaveCompany:string = "Save a new company";
    modelingLabelsOfButtons_SaveCurrency:string = "Save a new currency";
    modelingLabelsOfButtons_SavePriceList:string = "Save a new pricelist";
    modelingLabelsOfButtons_SaveWarehouse:string = "Save a new warehouse";
    modelingLabelsOfButtons_Save:string = "Save a new ";

    // Order
    orderComandButton_Cart:string = "Cart";
    orderComandButton_OrderHistory:string = "Order history";
    orderComandButton_OrderReport:string = "Order statistics";
    
    // Order title
    orderTitleCart_InfoTitle:string = "Current order info";
    orderTitleCart_NameCompany:string = "Name Company";
    orderTitleCart_TotaPrice:string = "Total Price";
    orderTitleCart_OrderCreatedByUser:string = "Order Created By User";
    orderTitleCart_OrderCurency:string = "Currency";
    orderTitleCart_CartIsEmpty:string = "Cart is empty";
    // Order labels of buttons
    orderLablesOfButton_CancelOrder:string = "Cancel order";
    orderLablesOfButton_SaveOrder:string = "Save Order";

    // User registration
    userRegistration_UserRegistrationTitle:string = "User registration";
    userRegistration_UserName:string = "User Name";
    userRegistration_UserNameError:string = "Please provide a valid Name.";
    userRegistration_UserNameErrorLinght:string = "Name must be at least 4 characters long.";
    userRegistration_Email:string = "Email";
    userRegistration_EmailError:string = "Please provide a valid email.";
    userRegistration_Password:string = "Password";
    userRegistration_PasswordError:string = "Please provide a valid password.";
    userRegistration_PasswordLenght:string = "Password must be at least 4 characters long.";
    userRegistration_ConfirmPassword:string = "Confirm Password";
    userRegistration_ConfirmPasswordError:string = "Password and Confirm password is not matched.";
    userRegistration_UserRole:string = "Choose an user role";
    userRegistration_UserRoleError:string = "Please choose an user role.";
    userRegistration_LableButton:string = "Register";

    // Log In
    logIn_LogInTitle:string = "Log In";
    logIn_UserName:string = "User Name";
    logIn_UserNameError:string = "Please provide a valid user name.";
    logIn_UserNameLenght:string = "Name must be at least 4 characters long.";
    logIn_Password:string = "Password";
    logIn_PasswordError:string = "Please provide a valid password.";
    logIn_UserNameErrorLenght:string = "Password must be at least 4 characters long.";
    logIn_LableButton:string = "Login";
    
}