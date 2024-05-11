import { IGdmLabels } from "./common";


export class gdmLabelsRU implements IGdmLabels  {
    constructor() {}
    // Global menu
    globalMenu_Home:string = "Главная";
    globalMenu_Product:string = "Продукты";
    globalMenu_Modeling:string = "Моделинг";
    globalMenu_Order:string = "Заказы";
    globalMenu_Report:string = "Отчеты";
    globalMenu_Account:string = "Аккаунт";
    globalMenu_SingUP:string = "Регистрация";
    globalMenu_Logout:string = "Выйти";

    // Product command
    productCommand_Product: string = "Продукты";
    productCommand_AddProduct: string = "Добавить продукт";
    productCommand_ReportProduct: string = "Статистика продуктов";
    productCommand_AddNewProduct: string = "Добавить новый продукт";
    productCommand_EditProduct: string = "Редактировать параметры продукта";

    // Title product cart
    titleProductCart_Product:string = "Продукты";
    subTitleProductCart_AddProduct:string = "Добавить экземпляр продукта";
    subTitleProductCart_AddNewProduct:string = "Добавить новый тип продукта";
    subTitleProductCart_EditParametersProduct:string = "Вы можете добавлять, удалять или редактировать параметры продукта";
    
    // Lables of product buttons
    productButton_SaveInstanceProduct= "Сохранить экземпляр продукта";
    productButton_SaveNewProduct= "Сохранить новый тип продукта";
    productButton_SaveChangesOfProduct= "Сохранить изменинения";

    // Filter panel
    filterPanel_Title:string = "ФИЛЬТРОВАТЬ";// "Фильтровать";

    filterPanel_Reset:string = "Сбросить";

    // Side panel
    sidePanel_Search:string = "Искать";
    
    // Product action menu
    productActionMenu_Order:string = "Быстрый заказ";
    productActionMenu_AddToCart:string = "Добавить в корзину";
    productActionMenu_Edit:string = "Изменить";
    productActionMenu_Delete:string = "Удалить";

    // Modeling title
    modelingTitle_Modeling:string = "Моделинг";
    modelingTitle_AddCompany:string = "Добавить компанию";
    modelingTitle_AddCurrency:string = "Добавить валюту";
    modelingTitle_AddPriceList:string = "Добавить прайс лист";
    modelingTitle_AddWarehouse:string = "Добавить склад";
    modelingTitle_Add:string = "Добавить ";
 
    // Modeling labels of buttons
    modelingLabelsOfButtons_Modeling:string = "Модели";
    modelingLabelsOfButtons_Create:string = "Создать";
    modelingLabelsOfButtons_SaveCompany:string = "Сохранить компанию";
    modelingLabelsOfButtons_SaveCurrency:string = "Сохранить валюту";
    modelingLabelsOfButtons_SavePriceList:string = "Сохранить прайс лист";
    modelingLabelsOfButtons_SaveWarehouse:string = "Сохранить склад";
    modelingLabelsOfButtons_Save:string = "Сохранить ";

    // Order
    orderComandButton_Cart:string = "Корзина";
    orderComandButton_OrderHistory:string = "История заказов";
    orderComandButton_OrderReport:string = "Статистика заказов";

    // Order title
    orderTitleCart_InfoTitle:string = "Информация о текущем заказе";
    orderTitleCart_NameCompany:string = "Имя компани";
    orderTitleCart_TotaPrice:string = "Общая стоймость";
    orderTitleCart_OrderCreatedByUser:string = "Заказ создан пользователем";
    orderTitleCart_OrderCurency:string = "Валюта";
    orderTitleCart_CartIsEmpty:string = "Корзина пуста";

    // Order labels of buttons
    orderLablesOfButton_CancelOrder:string = "Отменить заказ";
    orderLablesOfButton_SaveOrder:string = "Сохранить заказ";

    // Product popup title
    productActionPopUp_OrderProduct:string = "Заказ продукта";
    productActionPopUp_AddToCart:string = "Добавить в корзину";
    productActionPopUp_UpdateProduct:string = "Обновить информацию о продукте";
    productActionPopUp_DeleteProduct:string = "Удалить продукт";
    
    // Product popup lables of button 
    productActionPopUpButton_QuickOrder:string = "Быстрый заказ";
    productActionPopUpButton_AddToCart:string = "Добавить в корзину";
    productActionPopUpButton_Update:string = "Обновить";
    productActionPopUpButton_Delete:string = "Удалить";
    productActionPopUpButton_Cancel:string = "Отменить";
    
    // Modeling popup title
    modelingActionPopUp_UpdateCompany:string = "Обновить информацию о компании";
    modelingActionPopUp_DeleteCompany:string = "Удалить компанию";
    modelingActionPopUp_UpdateCurrency:string = "Обновить информацию о валюте";
    modelingActionPopUp_DeleteCurrency:string = "Удалить валюту";
    modelingActionPopUp_UpdateWarehouse:string = "Обновить информацию о складе";
    modelingActionPopUp_DeleteWarehouse:string = "Удалить скалад";

    // Order popup title
    modelingActionPopUp_DeleteProductFromCart:string = "Удалить продук с корзины";
    modelingActionPopUp_DeleteOrder:string = "Удалить заказ";
    
    // User registration
    userRegistration_UserRegistrationTitle:string = "Регистрация пользователя";
    userRegistration_UserName:string = "Имя пользователя";
    userRegistration_UserNameError:string = "Пожалуйста введите имя пользователя.";
    userRegistration_UserNameErrorLinght:string = "Имя пользователя должно содержать более 4 символов.";
    userRegistration_Email:string = "Email";
    userRegistration_EmailError:string = "Пожалуйста введите email.";
    userRegistration_Password:string = "Пароль";
    userRegistration_PasswordError:string = "Пожалуйста введите коректный пароль.";
    userRegistration_PasswordLenght:string = "Пароль должен содержать более 4 символов.";
    userRegistration_ConfirmPassword:string = "Подтверждение пароля";
    userRegistration_ConfirmPasswordError:string = "Пожалуйста введите коректный пароль.";
    userRegistration_UserRole:string = "Выберите роль пользователя";
    userRegistration_UserRoleError:string = "Пожалуйста выберите роль пользователя.";
    userRegistration_LableButton:string = "Зарегистрировать";

    // Log In
    logIn_LogInTitle:string = "Войти";
    logIn_UserName:string = "Имя пользователя";
    logIn_UserNameError:string = "Пожалуйста введите корректное имя пользователя.";
    logIn_UserNameLenght:string = "Имя пользователя должно содержать более 4 символов.";
    logIn_Password:string = "Пароль";
    logIn_PasswordError:string = "Пожалуйста введите коректный пароль.";
    logIn_UserNameErrorLenght:string = "Пароль должен содержать более 4 символов.";
    logIn_LableButton:string = "Войти";
}
