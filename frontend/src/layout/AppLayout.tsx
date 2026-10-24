import { cn } from "../utils";
import { Outlet } from "react-router";
import AppSidebar from "./AppSidebar";

const LayoutContent: React.FC = () => {

  return (
    <div className="min-h-screen flex-row">
      <AppSidebar />
      <main className="p-4 md:p-2">
        <Outlet />
      </main>
    </div>
  );
};

const AppLayout: React.FC = () => {
  return (
    <LayoutContent />
  );
};

export default AppLayout;
