import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class RoleGuard implements CanActivate {
  constructor(private router: Router) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    const userRole = localStorage.getItem('userRole') || ''; // Utiliser une chaîne vide par défaut
    const requiredRoles: string[] = route.data['roles'];

    if (requiredRoles && requiredRoles.includes(userRole)) {
      return true;
    }

    // Rediriger vers une page ou l'accueil si non autorisé
    this.router.navigate(['/home']);
    return false;
  }
}
