import { Box, Button, Paper, Typography } from "@mui/material";
import { useNavigate } from "react-router-dom";
import AppLayout from "../components/AppLayout";

type PlaceholderPageProps = {
  title: string;
};

export default function PlaceholderPage({ title }: PlaceholderPageProps) {
  const navigate = useNavigate();

  return (
    <AppLayout>
      <Box sx={{ maxWidth: 1000, mx: "auto" }}>
        <Typography variant="h4" sx={{ fontWeight: 700, mb: 3 }}>
          {title}
        </Typography>

        <Paper variant="outlined" sx={{ p: 4, borderRadius: 2 }}>
          <Typography color="text.secondary" sx={{ mb: 3 }}>
            This page will be implemented in the next stage.
          </Typography>

          <Button variant="contained" onClick={() => navigate("/dashboard")}>
            Back to Dashboard
          </Button>
        </Paper>
      </Box>
    </AppLayout>
  );
}
