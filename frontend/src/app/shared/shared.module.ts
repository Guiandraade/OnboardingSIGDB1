import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { CnpjPipe } from 'src/app/shared/pipes/cnpj.pipe';
import { DateBrPipe } from 'src/app/shared/pipes/date-br.pipe';
import { CpfPipe }  from 'src/app/shared/pipes/cpf.pipe';
import { ConfirmDeleteComponent } from 'src/app/shared/components/confirm-delete/confirm-delete.component';
import { PaginationComponent } from 'src/app/shared/components/pagination/pagination.component';
import { FormErrorBannerComponent } from 'src/app/shared/components/form-error-banner/form-error-banner.component';
import { FormSubmitActionsComponent } from 'src/app/shared/components/form-submit-actions/form-submit-actions.component';
import { ValidationRulesComponent } from 'src/app/shared/components/validation-rules/validation-rules.component';
import { SearchSelectComponent } from 'src/app/shared/components/search-select/search-select.component';

@NgModule({
  declarations: [
    CnpjPipe,
    DateBrPipe,
    CpfPipe,
    ConfirmDeleteComponent,
    PaginationComponent,
    FormErrorBannerComponent,
    FormSubmitActionsComponent,
    ValidationRulesComponent,
    SearchSelectComponent
  ],
  imports: [CommonModule],
  exports: [
    CnpjPipe,
    DateBrPipe,
    CpfPipe,
    ConfirmDeleteComponent,
    PaginationComponent,
    FormErrorBannerComponent,
    FormSubmitActionsComponent,
    ValidationRulesComponent,
    SearchSelectComponent
  ]
})

export class SharedModule {}
