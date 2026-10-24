import { BrowserRouter, Route, Routes } from "react-router"
import Dashboard from "./screens/Dashboard"
import ProtectedRoute from "./components/ProtectedRoute"
import AppLayout from "./layout/AppLayout"
import Login from "./screens/Login"

const routes: React.FC = () => {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/login" element={<Login />} />

                <Route element={<ProtectedRoute />}>
                    <Route path="/" element={<AppLayout />}>    
                        <Route index element={<Dashboard />} />
                    </Route>
                </Route>
            </Routes>
        </BrowserRouter>
    )    
}

export default routes