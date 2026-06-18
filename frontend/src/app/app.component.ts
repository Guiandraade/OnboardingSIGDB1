import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd, NavigationStart, ActivatedRoute } from '@angular/router';
import { filter, map } from 'rxjs/operators';
import { UserService } from './core/services/user.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'OnboardingSIGDB1-Front';
  currentPageTitle: string = 'Dashboard';
  isLoading = false;
  sidebarOpen = false;

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    readonly user: UserService
  ) {}

  toggleSidebar(): void {
    this.sidebarOpen = !this.sidebarOpen;
  }

  closeSidebar(): void {
    this.sidebarOpen = false;
  }

  ngOnInit(): void {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationStart) {
        this.isLoading = true;
        this.sidebarOpen = false;
      } else if (event instanceof NavigationEnd) {
        setTimeout(() => this.isLoading = false, 400);
      }
    });

    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd),
      map(() => {
        let route = this.activatedRoute.root;
        while (route.firstChild) {
          route = route.firstChild;
        }
        return route.snapshot.data['breadcrumb'] || 'Dashboard';
      })
    ).subscribe((title: string) => {
      this.currentPageTitle = title;
    });
  }
}
