import { useKeycloak } from "@react-keycloak/web";
import { Navigate, Outlet } from "react-router";

export default function PrivateRoute() {
  const { keycloak, initialized } = useKeycloak();

  if (!initialized) {
    return <p>Loading...</p>;
  }

  if (!keycloak.authenticated) {
    return <Navigate replace to="/login" />
  }

  return <Outlet />;
}