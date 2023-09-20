import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { gridParameter, IGetOrderInstancesRequest, IgetProductTypeInstancesRequest, ISortOption, siteURI, UserForRegistrationDto } from "../objects/common";

@Injectable()
export class ApplicationService {
    constructor(private http: HttpClient) {}

    getMetadata():Observable<any>{
        return  this.http.get(siteURI + "api/metadata/getmetadatatypes");
    }

    deleteProductInstance(id):Observable<any>{
      return  this.http.delete(siteURI + "api/product/deleteProduct/" + `${id}`);
    }

    refreshToken(token:string):Promise<any>{
        return  this.http.post(siteURI + "api/token/refresh", token, {
         headers: new HttpHeaders({
           "Content-Type": "application/json"
         }),observe:'response'
       }).toPromise();
    }
   
    login(credentials:string):Observable<any> {
        return this.http.post(siteURI + "api/auth/login", credentials, {
            headers: new HttpHeaders ({
              "Content-Type": "application/json"
            })
          })
    }
    
    regUser (body: UserForRegistrationDto):Observable<any> {
        return this.http.post<UserForRegistrationDto> (siteURI + "api/auth/registration", body);
    }

    addNewProduct(body:any):Observable<any> {
        return this.http.post<UserForRegistrationDto> (siteURI + "api/product/AddNewProducts", body,{
          headers: new HttpHeaders ({
            "Content-Type": "application/json"
          })
        });
    }

    addOrderProduct(body:any):Observable<any> {
      return this.http.post<UserForRegistrationDto> (siteURI + "api/Orders/addOrderProduct", body,{
        headers: new HttpHeaders ({
          "Content-Type": "application/json"
        })
      });
    }

    addToCartOrder(body:any):Observable<any> {
      return this.http.post<UserForRegistrationDto> (siteURI + "api/Orders/addToCartOrder", body,{
        headers: new HttpHeaders ({
          "Content-Type": "application/json"
        })
      });
    }

    addCompany(body:any):Observable<any> {
      return this.http.post<UserForRegistrationDto> (siteURI + "api/customer/AddNewCustomer", body,{
        headers: new HttpHeaders ({
          "Content-Type": "application/json"
        })
      });
    }

    updateCompany(body:any):Observable<any> {
      return this.http.post<UserForRegistrationDto> (siteURI + "api/customer/UpdateCustomer", body,{
        headers: new HttpHeaders ({
          "Content-Type": "application/json"
          })
      });
    }

    deleteCompany(id):Observable<any>{
      return this.http.delete(siteURI + "api/customer/DeleteCustomer/" + `${id}`);
    }

    getCompanies():Observable<any>{
      return  this.http.get(siteURI + "api/customer/GetAll");
    }

    getCompaniesNames():Observable<any>{
      return  this.http.get(siteURI + "api/customer/GetNamesCustomeres");
    }

    addWareHouse(body:any):Observable<any> {
      return this.http.post<UserForRegistrationDto> (siteURI + "api/warehouse/AddNewWareHouse", body,{
        headers: new HttpHeaders ({
          "Content-Type": "application/json"
        })
      });
    }

    updateWareHouse(body:any):Observable<any> {
      return this.http.post<UserForRegistrationDto> (siteURI + "api/warehouse/UpdateWareHouse", body,{
        headers: new HttpHeaders ({
          "Content-Type": "application/json"
          })
      });
    }

    deleteWareHouse(id):Observable<any>{
      return this.http.delete(siteURI + "api/warehouse/DeleteWareHouse/" + `${id}`);
    }

    getWareHouses():Observable<any>{
      return  this.http.get(siteURI + "api/warehouse/GetAll");
    }

    GetNamesWareHouses():Observable<any>{
      return  this.http.get(siteURI + "api/warehouse/GetNamesWareHouses");
    }

    getWareHouseByName(name):Observable<any>{
      return  this.http.get(siteURI + "api/warehouse/GetWareHouseByName/" + name);
    }

    getCurrencies():Observable<any>{
      return  this.http.get(siteURI + "api/сurrency/GetAll");
    }

    getNamesCurrencies():Observable<any>{
      return  this.http.get(siteURI + "api/сurrency/GetNamesCurrencies");
    }
    
    getCurrencyByName(name):Observable<any>{
      return  this.http.get(siteURI + "api/сurrency/getCurrencyByNameCurrency/" + name);
    }

    getUniqueNameParameters():Observable<any>{
      return  this.http.get(siteURI + "api/parameter/getUniqueNameParameters");
    }
    
    addСurrency(body:any):Observable<any> {
      return this.http.post<UserForRegistrationDto> (siteURI + "api/сurrency/AddNewCurrency", body,{
        headers: new HttpHeaders ({
          "Content-Type": "application/json"
        })
      });
    }

    updateСurrency(body:any):Observable<any> {
      return this.http.post<UserForRegistrationDto> (siteURI + "api/сurrency/UpdateCurrency", body,{
        headers: new HttpHeaders ({
          "Content-Type": "application/json"
          })
      });
    }

    deleteСurrency(id):Observable<any>{
      return this.http.delete(siteURI + "api/сurrency/DeleteСurrency/" + `${id}`);
    }


    getPriceLists():Observable<any>{
      return  this.http.get(siteURI + "api/pricelist/GetAll");
    }

    getPriceListWithValues(id):Observable<any>{
      return  this.http.get(siteURI + "api/pricelist/getPriceListWithValues" + `${id}`);
    }
    
    getPriceListWithValuesByName(name):Observable<any>{
      return  this.http.get(siteURI + "api/pricelist/getPriceListWithValuesByName/" + `${name}`);
    }

    addPriceList(body:any):Observable<any> {
      return this.http.post<UserForRegistrationDto> (siteURI + "api/pricelist/addPriceList", body,{
        headers: new HttpHeaders ({
          "Content-Type": "application/json"
        })
      });
    }

    updatePriceList(body:any):Observable<any> {
      return this.http.post<UserForRegistrationDto> (siteURI + "api/pricelist/updatePriceList", body,{
        headers: new HttpHeaders ({
          "Content-Type": "application/json"
          })
      });
    }

    deletePriceList(id):Observable<any>{
      return this.http.delete(siteURI + "api/pricelist/deletePriceList/" + `${id}`);
    }

    addInstanceProduct(body:any):Observable<any> {
        return this.http.post<UserForRegistrationDto> (siteURI + "api/product/AddInstanceProduct", body,{
            headers: new HttpHeaders ({
              "Content-Type": "application/json"
            })
        });
    }
    
    updateProductInstance(body:any):Observable<any> {
      return this.http.post<UserForRegistrationDto> (siteURI + "api/product/UpdateProductInstance", body,{
        headers: new HttpHeaders ({
          "Content-Type": "application/json"
          })
      });
    }

    getProductManufacturers():Observable<any>{
      return  this.http.get(siteURI + "api/product/getProductManufacturers");
    }

    loadProductManufacturersByProductName(nameProduct):Observable<any>{
      return  this.http.get(siteURI + "api/product/getManufacturersByProductName/" + nameProduct);
    }

    getProductSuppliers():Observable<any>{
      return  this.http.get(siteURI + "api/product/getProductSuppliers");
    }

    getProductSuppliersByProductName(nameProduct):Observable<any>{
      return  this.http.get(siteURI + "api/product/getProductSuppliersByProductName/" + nameProduct);
    }

    getProductTypes():Observable<any>{
        return  this.http.get(siteURI + "api/productTypes/getProductTypeNames");
    }

    getCustomerByNameCompany(name):Observable<any>{
      return  this.http.get(siteURI + "api/customer/getCustomerByNameCompany/" + name);
    }
    
    getProductTypeInstances(nameProduct:string, pageNumber:number = 1, pageSize:number = 10,
                            sortOptions:ISortOption = null):Observable<any>{
     const param = nameProduct;
     const paginationParam = `?pageNumber=${pageNumber}&pageSize=${pageSize}`;
     const sortOption = sortOptions != null? `&direction=${sortOptions.direction.trim()}
           &name=${sortOptions.name.trim()}&isParameter=${sortOptions.isParameter}`: "";


      return  this.http.get(
            siteURI + "api/productTypes/getProductTypeInstances/" + param + paginationParam + sortOption
        );
    }

    getProductTypeInstances2(getProductTypeInstancesRequest:IgetProductTypeInstancesRequest):Observable<any>{ 
     
      return this.http.post<UserForRegistrationDto> 
      (siteURI + "api/productTypes/getProductTypeInstances2", getProductTypeInstancesRequest,{
         headers: new HttpHeaders ({
           "Content-Type": "application/json"
          })
       });
      
    }

    getOrders(getOrdersInstancesRequest:IGetOrderInstancesRequest):Observable<any>{ 
     
      const body = getOrdersInstancesRequest; //JSON.stringify(getOrdersInstancesRequest);
      return this.http.post<UserForRegistrationDto> 
      (siteURI + "api/orders/getOrderProduct", body,{
         headers: new HttpHeaders ({
           "Content-Type": "application/json"
          })
       });
      
    }

    getCartOrderProducts():Observable<any>{ 
      const body = {};
      return this.http.get<UserForRegistrationDto> 
      (siteURI + "api/orders/getCartOrderProducts",{
         headers: new HttpHeaders ({
           "Content-Type": "application/json"
          })
       });
      
    }

    getOrderProductList(getOrdersInstancesRequest:IGetOrderInstancesRequest):Observable<any>{ 
     
      const body = getOrdersInstancesRequest; //JSON.stringify(getOrdersInstancesRequest);
      return this.http.post<UserForRegistrationDto> 
      (siteURI + "api/orders/getOrderProductList", body,{
         headers: new HttpHeaders ({
           "Content-Type": "application/json"
          })
       });
      
    }

    getOrderNameCompanies():Observable<any>{
        return  this.http.get(siteURI + "api/orders/getOrderNameCompanies");
    }

    saveOrderCart():Observable<any>{
      return  this.http.get(siteURI + "api/orders/saveOrderCart");
    }
    
    cancelOrderCart():Observable<any>{
      return  this.http.get(siteURI + "api/orders/cancelOrderCart");
    }



    getNamesProduct():Observable<any>{
      return  this.http.get(siteURI + "api/orders/getNamesProduct");
    }

    getProductParameters(nameProduct:string):Observable<any>{
      console.log("--[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[ getProductParameters ]]]]]]]]]]]]]]]");
       const param = nameProduct;
       return  this.http.get(
              siteURI + "api/productTypes/getProductTypeParameters/" + param
         );
    }

    getProductParameters2(nameProduct:string):Observable<any>{
      console.log("--[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[ getProductParameters 2]]]]]]]]]]]]]]]");
       const param = nameProduct;
       return  this.http.get(
              siteURI + "api/productTypes/getProductTypeParameters/" + param
         );
    }

    getProductParameters3(nameProduct:string):Observable<any>{
      console.log("--[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[ getProductParameters 3]]]]]]]]]]]]]]]");
       const param = nameProduct;
       return  this.http.get(
              siteURI + "api/productTypes/getProductTypeParameters/" + param
         );
    }

    getInstancesParameter(parameters):Observable<any>{

      const body = JSON.stringify(parameters); //JSON.stringify(getOrdersInstancesRequest);
      return this.http.post<UserForRegistrationDto> 
      (siteURI + "api/product/getInstancesOfProductParameter", body,{
         headers: new HttpHeaders ({
           "Content-Type": "application/json"
          })
       });

    }

    updateProductParameters(body:any):Observable<any> {
      return this.http.post<UserForRegistrationDto> (siteURI + "api/parameter/UpdateProductTypeParameters", body,{
        headers: new HttpHeaders ({
          "Content-Type": "application/json"
          })
      });
    }

    deleteCartProduct(id):Observable<any>{
      return this.http.delete(siteURI + "api/orders/deleteCartProduct/" + `${id}`);
    }

    deleteOrder(id):Observable<any>{
      return  this.http.delete(siteURI + "api/orders/deleteOrderProduct/"+ `${id}`);
    }

    GetCartOrderCount():Observable<any>{
      return  this.http.get(siteURI + "api/orders/GetCartOrderCount");
    }

    getNBRBCurrencies(date = 0, period = 0):Observable<any>{
      //https://api.nbrb.by/exrates/rates?periodicity=0
      //exrates/rates?periodicity=0
      const periodicity = `periodicity=${period}`;
      const onDate = `ondate=${date}`;
      const parameters = date == 0 ? `?${periodicity}` : `?${onDate}&${periodicity}`;
      const uriNbrb = "/exrates/rates" + parameters;
      console.log(uriNbrb);
      return  this.http.get(uriNbrb);
    }


  getPeport(): Observable<any> {
    return this.http.get(
      siteURI + "api/product/loadReport"
    );
  }

}

