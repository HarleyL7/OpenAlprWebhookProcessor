import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { DayCounts } from "./plateCountResponse";

@Injectable({ providedIn: 'root' })
export class HomeService {
    private plateCountsUrl = 'licenseplates/counts'

    constructor(private http: HttpClient) { }

    getPlatesCount(numberOfDays: number): Observable<any> {
        return this.http.get<any>(`/${this.plateCountsUrl}/${numberOfDays}`);
    }
}
