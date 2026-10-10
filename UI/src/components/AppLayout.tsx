import { useState, type ReactNode } from "react";
import {
  AppBar,
  Box,
  Button,
  Chip,
  Divider,
  Drawer,
  IconButton,
  List,
  ListItemButton,
  ListItemText,
  Toolbar,
  Typography,
} from "@mui/material";
import MenuIcon from "@mui/icons-material/Menu";
import { useLocation, useNavigate } from "react-router-dom";

const drawerWidth = 240;

type AppLayoutProps = {
  children: ReactNode;
};

type StoredAuth = {
  email: string;
  roles: string[];
};

function getStoredAuth(): StoredAuth | null {
  try {
    const storedAuth = localStorage.getItem("auth");

    if (!storedAuth) return null;

    return JSON.parse(storedAuth) as StoredAuth;
  } catch {
    return null;
  }
}

export default function AppLayout({ children }: AppLayoutProps) {
  const [mobileOpen, setMobileOpen] = useState(false);
  const navigate = useNavigate();

  const location = useLocation();

  const auth = getStoredAuth();

  const email = auth?.email ?? "Unknown user";
  const role = auth?.roles?.[0] ?? "Unknown role";
  const navigationItems = [
    {
      label: "Dashboard",
      path: "/dashboard",
      roles: ["Employee", "Manager", "Finance"],
    },
    {
      label: "Create Request",
      path: "/requests/create",
      roles: ["Employee"],
    },
    {
      label: "My Requests",
      path: "/requests/my",
      roles: ["Employee"],
    },
    {
      label: "Assigned Requests",
      path: "/requests/assigned",
      roles: ["Manager", "Finance"],
    },
    {
      label: "Approve / Reject",
      path: "/requests/review",
      roles: ["Manager", "Finance"],
    },
  ];

  const visibleNavigationItems = navigationItems.filter((item) =>
    item.roles.includes(role),
  );

  const handleDrawerToggle = () => {
    setMobileOpen((previous) => !previous);
  };

  const handleLogout = () => {
    localStorage.removeItem("auth");
    navigate("/login", { replace: true });
  };

  const drawerContent = (
    <Box>
      <Toolbar>
        <Typography variant="h6" sx={{ fontWeight: 700 }}>
          Investment Fund
        </Typography>
      </Toolbar>

      <Divider />

      <List>
        {visibleNavigationItems.map((item) => (
          <ListItemButton
            key={item.path}
            selected={location.pathname === item.path}
            onClick={() => {
              navigate(item.path);
              setMobileOpen(false);
            }}
            sx={{
              mx: 1,
              my: 0.5,
              borderRadius: 1,
              "&.Mui-selected": {
                bgcolor: "action.selected",
                color: "primary.main",
                fontWeight: 600,
              },
            }}
          >
            <ListItemText primary={item.label} />
          </ListItemButton>
        ))}
      </List>
    </Box>
  );

  return (
    <Box
      sx={{
        display: "flex",
        minHeight: "100vh",
        bgcolor: "background.default",
      }}
    >
      {/* Header */}
      <AppBar
        position="fixed"
        sx={{
          width: {
            sm: `calc(100% - ${drawerWidth}px)`,
          },
          ml: {
            sm: `${drawerWidth}px`,
          },
        }}
      >
        <Toolbar>
          <IconButton
            color="inherit"
            edge="start"
            onClick={handleDrawerToggle}
            sx={{
              mr: 2,
              display: { sm: "none" },
            }}
          >
            <MenuIcon />
          </IconButton>

          <Typography variant="h6" noWrap sx={{ flexGrow: 1 }}>
            Dashboard
          </Typography>

          {/* User information */}
          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              gap: { xs: 1, sm: 2 },
            }}
          >
            <Typography
              variant="body2"
              sx={{
                display: { xs: "none", md: "block" },
              }}
            >
              {email}
            </Typography>

            <Chip
              label={role}
              size="small"
              sx={{
                bgcolor: "white",
                color: "primary.main",
                fontWeight: 600,
              }}
            />

            <Button
              variant="contained"
              color="error"
              size="small"
              onClick={handleLogout}
            >
              Logout
            </Button>
          </Box>
        </Toolbar>
      </AppBar>

      {/* Sidebar */}
      <Box
        component="nav"
        sx={{
          width: { sm: drawerWidth },
          flexShrink: { sm: 0 },
        }}
      >
        {/* Mobile sidebar */}
        <Drawer
          variant="temporary"
          open={mobileOpen}
          onClose={handleDrawerToggle}
          ModalProps={{
            keepMounted: true,
          }}
          sx={{
            display: { xs: "block", sm: "none" },
            "& .MuiDrawer-paper": {
              width: drawerWidth,
              boxSizing: "border-box",
            },
          }}
        >
          {drawerContent}
        </Drawer>

        {/* Desktop sidebar */}
        <Drawer
          variant="permanent"
          open
          sx={{
            display: { xs: "none", sm: "block" },
            "& .MuiDrawer-paper": {
              width: drawerWidth,
              boxSizing: "border-box",
            },
          }}
        >
          {drawerContent}
        </Drawer>
      </Box>

      {/* Main content */}
      <Box
        component="main"
        sx={{
          flexGrow: 1,
          minWidth: 0,
          width: {
            sm: `calc(100% - ${drawerWidth}px)`,
          },
          p: { xs: 2, md: 3 },
        }}
      >
        <Toolbar />
        {children}
      </Box>
    </Box>
  );
}
