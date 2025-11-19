import { Component, inject } from '@angular/core';
import { RouterOutlet, RouterLink } from '@angular/router';
import { Auth } from './services/auth';
import {
  TranslateService,
  TranslatePipe,
  TranslateDirective
} from "@ngx-translate/core";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, TranslatePipe, TranslateDirective],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class AppComponent {
  private translate = inject(TranslateService);

  constructor(
    private authService: Auth
  ) {
    this.translate.addLangs(['pt', 'en']);
    this.translate.setFallbackLang('en');

    const browserLang = this.translate.getBrowserLang();
    const langToUse = browserLang?.match(/en|pt/) ? browserLang : 'en';
    this.translate.use('pt');
  }
}