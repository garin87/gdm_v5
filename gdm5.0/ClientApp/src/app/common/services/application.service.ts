import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { siteURI, SortOptions, UserForRegistrationDto } from "../objects/common";

@Injectable()
export class ApplicationService {
    constructor(private http: HttpClient) {}

    getMetadata():Observable<any>{
        return  this.http.get(siteURI + "api/metadata/getmetadatatypes");
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
     addInstanceProduct(body:any):Observable<any> {
      return this.http.post<UserForRegistrationDto> (siteURI + "api/product/AddInstanceProduct", body,{
         headers: new HttpHeaders ({
           "Content-Type": "application/json"
         })
      });
     }
    getProductTypes():Observable<any>{
       return  this.http.get(siteURI + "api/productTypes/getProductTypeNames");
    }

    getProductTypeInstances(nameProduct:string, pageNumber:number = 1, pageSize:number = 10,
                            sortOptions:SortOptions = null):Observable<any>{
     const param = nameProduct;
     const paginationParam = `?pageNumber=${pageNumber}&pageSize=${pageSize}`;
     const sortOption = sortOptions != null? `&direction=${sortOptions.direction.trim()}
           &name=${sortOptions.name.trim()}&isParameter=${sortOptions.isParameter}`: "";

     return  this.http.get(
           siteURI + "api/productTypes/getProductTypeInstances/" + param + paginationParam + sortOption
      );
    }

    getProductParameters(nameProduct:string):Observable<any>{
       const param = nameProduct;
       return  this.http.get(
              siteURI + "api/productTypes/getProductTypeParameters/" + param
         );
    }


   updateProductParameters(body:any):Observable<any> {
    return this.http.post<UserForRegistrationDto> (siteURI + "api/parameter/UpdateProductTypeParameters", body,{
       headers: new HttpHeaders ({
         "Content-Type": "application/json"
        })
     });
   }
}