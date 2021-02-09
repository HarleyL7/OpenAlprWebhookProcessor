﻿import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { JwtInterceptor, ErrorInterceptor } from './_helpers';
import { AppComponent } from './app.component';
import { AlertComponent } from './_components';
import { HomeComponent } from './home';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { LightboxModule } from 'ngx-lightbox';
import { MatButtonModule } from '@angular/material/button';
import { FastSearchComponent } from './fast-search/fast-search.component';
import { AccountService } from './_services';
import { appInitializer } from './_helpers/app.initializer';
import { MatTabsModule } from '@angular/material/tabs';
import { MatIconModule } from '@angular/material/icon';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatCardModule } from '@angular/material/card';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDividerModule } from '@angular/material/divider';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatSnackBarModule } from '@angular/material/snack-bar';;
import { SnackbarComponent } from './snackbar/snackbar.component'
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@NgModule({
    imports: [
        BrowserModule,
        AppRoutingModule,
        FormsModule,
        ReactiveFormsModule,
        HttpClientModule,
        BrowserAnimationsModule,
        MatAutocompleteModule,
        LightboxModule,
        MatButtonModule,
        MatTabsModule,
        MatIconModule,
        MatCardModule,
        FlexLayoutModule,
        MatDatepickerModule,
        MatInputModule,
        MatFormFieldModule,
        MatCheckboxModule,
        MatDividerModule,
        MatSlideToggleModule,
        MatSnackBarModule,
        MatProgressSpinnerModule
    ],
    declarations: [
        AppComponent,
        AlertComponent,
        HomeComponent,
        FastSearchComponent,
        SnackbarComponent
    ],
    providers: [
        { provide: APP_INITIALIZER, useFactory: appInitializer, multi: true, deps: [AccountService] },
        { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
        { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    ],
    bootstrap: [AppComponent]
})
export class AppModule { };