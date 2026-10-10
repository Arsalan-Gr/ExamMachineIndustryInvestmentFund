import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { Typography, Box } from "@mui/material";
import LoginPage from "./pages/LoginPage";

function Dashboard() {
  return (
    <Box sx={{ p: 4 }}>
      <Typography variant="h4">Dashboard</Typography>
      <Typography>Login successful!</Typography>
    </Box>
  );
}

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="*" element={<Navigate to="/login" replace />} />
      </Routes>
    </BrowserRouter>
  );
}
