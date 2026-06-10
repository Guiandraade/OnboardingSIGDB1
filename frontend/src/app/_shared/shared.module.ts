import { NgModule } from "@angular/core";
import { CnpjPipe } from 'src/app/_common/_pipes/cnpj.pipe';
import { DateBrPipe } from 'src/app/_common/_pipes/date-br.pipe';
import { CpfPipe }  from 'src/app/_common/_pipes/cpf.pipe';

@NgModule({
  declarations: [CnpjPipe, DateBrPipe, CpfPipe],
  imports: [],
  exports: [CnpjPipe, DateBrPipe, CpfPipe]
})

export class SharedModule {}
