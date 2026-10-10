import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";

import LoginPage from "./pages/LoginPage";
import DashboardPage from "./pages/DashboardPage";
import ProtectedRoute from "./components/ProtectedRoute";
import PlaceholderPage from "./pages/PlaceholderPage";

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
          <Route
            path="/requests/create"
            element={<PlaceholderPage title="Create Request" />}
          />
          <Route
            path="/requests/my"
            element={<PlaceholderPage title="My Requests" />}
          />
        </Route>

        {/* Manager and Finance */}
        <Route
          element={<ProtectedRoute allowedRoles={["Manager", "Finance"]} />}
        >
          <Route
            path="/requests/assigned"
            element={<PlaceholderPage title="Assigned Requests" />}
          />
          <Route
            path="/requests/review"
            element={<PlaceholderPage title="Approve / Reject" />}
          />
        </Route>

        {/* Fallback */}
        <Route path="*" element={<Navigate to="/dashboard" replace />} />
      </Routes>
    </BrowserRouter>
  );
}
