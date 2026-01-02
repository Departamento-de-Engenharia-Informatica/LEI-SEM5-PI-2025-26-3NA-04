import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule, RouterModule, TranslateModule],
  templateUrl: './footer.html',
  styleUrls: ['./footer.css']
})
export class Footer {
  currentYear: number = new Date().getFullYear();

  constructor(public translate: TranslateService) {}

  switchLanguage(lang: string): void {
    this.translate.use(lang);
    console.log('Language switched to:', lang);
  }

  isCurrentLang(lang: string): boolean {
    return this.translate.currentLang === lang;
  }
}