import { Link } from "react-router";
import { navigationItems } from "../config/navigation";
import { cn } from "../utils";
import logo from '../assets/logo.webp';

export default function AppSidebar() {
    const renderNavbarItems = () => {
        return navigationItems.map((navItem, index) => {
            const Icon = navItem.icon;

            return (
                <li key={index} className="flex items-center gap-3 p-3 cursor:pointer text-lg">
                    <span
                        className={"menu-item-icon-size menu-item-icon-active"}
                    >
                        <Icon size={20} ></Icon>
                    </span>
                    
                    <Link to={navItem.path}>
                        {navItem.title}
                    </Link>
                </li>
            )
        })
    }

    return (
        <aside
            className={cn(
                "fixed inset-s-0 top-0 z-50 flex h-screen flex-col border-e border-gray-200 bg-white px-5 py-3 text-gray-900 transition-all duration-300 ease-in-out xl:translate-x-0 xl:rtl:translate-x-0",
                "dark:border-gray-800 dark:bg-gray-900 dark:text-white",
                "w-72.5",
                "-translate-x-full rtl:translate-x-full"
            )}
        >
            <div className="font-bold text-xl mb-4 flex gap-3 items-center">
                <img src={logo} width="36px" height="36px" alt="" />
                <h1>Coworking Booking</h1>
            </div>
            <ul>
                {renderNavbarItems()}
            </ul>
        </aside>
    );
}