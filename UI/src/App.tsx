import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";

import LoginPage from "./pages/LoginPage";
import DashboardPage from "./pages/DashboardPage";
import ProtectedRoute from "./components/ProtectedRoute";
import CreateRequestPage from "./pages/CreateRequestPage";
import MyRequestListPage from "./pages/MyRequestListPage";
import ReviewRequestsPage from "./pages/ReviewRequestsPage";

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        {/* Public */}
        <Route path="/login" element={<LoginPage />} />

        {/* All authenticated users */}
        <Route element={<ProtectedRoute />}>
          <Route path="/dashboard" element={<DashboardPage />} />
        </Route>

        {/* Employee */}
        <Route element={<ProtectedRoute allowedRoles={["Employee"]} />}>
          <Route path="/requests/create" element={<CreateRequestPage />} />
          <Route path="/requests/my" element={<MyRequestListPage />} />
        </Route>

        {/* Manager and Finance */}
        <Route
          element={<ProtectedRoute allowedRoles={["Manager", "Finance"]} />}
        >
          <Route
            path="/requests/assigned"
            element={<MyRequestListPage assigned />}
          />
          <Route path="/requests/review" element={<ReviewRequestsPage />} />
        </Route>

        {/* Fallback */}
        <Route path="*" element={<Navigate to="/dashboard" replace />} />
      </Routes>
    </BrowserRouter>
  );
}
