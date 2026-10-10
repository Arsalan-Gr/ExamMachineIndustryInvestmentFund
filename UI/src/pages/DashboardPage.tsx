import {
  Box,
  Card,
  CardActionArea,
  CardContent,
  Grid,
  Typography,
} from "@mui/material";
import { useNavigate } from "react-router-dom";
import AppLayout from "../components/AppLayout";

type DashboardItem = {
  title: string;
  description: string;
  path: string;
};

export default function DashboardPage() {
  const navigate = useNavigate();

  let role = "Employee";

  try {
    const auth = JSON.parse(localStorage.getItem("auth") || "null");
    role = auth?.roles?.[0] ?? "Employee";
  } catch {
    role = "Employee";
  }

  const employeeItems: DashboardItem[] = [
    {
      title: "Create Request",
      description: "Submit a new request for approval.",
      path: "/requests/create",
    },
    {
      title: "My Requests",
      description: "View and track your submitted requests.",
      path: "/requests/my",
    },
  ];

  const reviewerItems: DashboardItem[] = [
    {
      title: "Assigned Requests",
      description: "View requests assigned to your role.",
      path: "/requests/assigned",
    },
    {
      title: "Approve / Reject",
      description: "Review pending requests and take action.",
      path: "/requests/review",
    },
  ];

  const items =
    role === "Employee"
      ? employeeItems
      : role === "Manager" || role === "Finance"
        ? reviewerItems
        : [];

  return (
    <AppLayout>
      <Box sx={{ maxWidth: 1100, mx: "auto" }}>
        <Typography variant="h4" sx={{ fontWeight: 700, mb: 1 }}>
          Welcome to Investment Fund
        </Typography>

        <Typography variant="body1" color="text.secondary" sx={{ mb: 4 }}>
          Manage your requests and approval workflow.
        </Typography>

        <Grid container spacing={3}>
          {items.map((item) => (
            <Grid key={item.path} size={{ xs: 12, md: 6 }}>
              <Card
                variant="outlined"
                sx={{
                  height: "100%",
                  borderRadius: 2,
                }}
              >
                <CardActionArea
                  onClick={() => navigate(item.path)}
                  sx={{ height: "100%" }}
                >
                  <CardContent sx={{ p: 3 }}>
                    <Typography variant="h6" sx={{ fontWeight: 600, mb: 1 }}>
                      {item.title}
                    </Typography>

                    <Typography color="text.secondary">
                      {item.description}
                    </Typography>
                  </CardContent>
                </CardActionArea>
              </Card>
            </Grid>
          ))}
        </Grid>
      </Box>
    </AppLayout>
  );
}
