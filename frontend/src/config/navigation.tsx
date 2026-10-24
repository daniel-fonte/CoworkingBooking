import { LayoutDashboard, BuildingComplex, type LucideIcon } from "lucide-react";

export interface NavigationItem {
  title: string;
  path: string;
  icon: LucideIcon;
  roles?: string[];
}

export const navigationItems: NavigationItem[] = [
    {
        title: 'Dashboard',
        path: '/',
        icon: LayoutDashboard
    },
    {
        title: 'Coworkings',
        path: '/coworkings',
        icon: BuildingComplex
    }
]
