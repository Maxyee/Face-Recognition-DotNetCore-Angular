import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { WebcamComponent } from './components/webcam/webcam.component';

const routes: Routes = [
  { path: '', component: WebcamComponent },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }