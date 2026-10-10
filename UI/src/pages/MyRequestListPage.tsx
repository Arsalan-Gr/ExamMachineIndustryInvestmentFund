import { useEffect, useState } from "react";
import {
  Alert,
  Box,
  Button,
  Chip,
  CircularProgress,
  FormControl,
  InputLabel,
  MenuItem,
  Paper,
  Select,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TablePagination,
  TableRow,
  TextField,
  Typography,
} from "@mui/material";

import { getRequests, type RequestDto } from "../api/requestApi";

function getStatusLabel(status: number | string) {
  const labels: Record<string, string> = {
    "0": "Unknown",
    "1": "Created",
    "2": "Pending",
    "3": "Approved",
    "4": "Rejected",
  };

  const value = String(status);
  return labels[value] ?? value;
}

function getStatusColor(status: number | string) {
  const value = String(status).toLowerCase();

  if (value === "3" || value === "approved") return "success" as const;

  if (value === "4" || value === "rejected") return "error" as const;

  if (value === "2" || value === "pending") return "warning" as const;

  return "default" as const;
}

function getRoleLabel(role: number | string) {
  const labels: Record<string, string> = {
    "0": "Unknown",
    "1": "Finance",
    "2": "Manager",
  };

  const value = String(role);
  return labels[value] ?? value;
}

export default function MyRequestsPage({
  assigned = false,
}: {
  assigned?: boolean;
}) {
  const [requests, setRequests] = useState<RequestDto[]>([]);
  const [totalCount, setTotalCount] = useState(0);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  // MUI uses zero-based pages; backend uses one-based pages.
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);

  const [searchTerm, setSearchTerm] = useState("");
  const [status, setStatus] = useState("");
  const [assignedRole, setAssignedRole] = useState("");

  const [sortBy, setSortBy] = useState("CreatedAt");
  const [sortDescending, setSortDescending] = useState(true);

  const [reloadKey, setReloadKey] = useState(0);

  useEffect(() => {
    let active = true;

    async function loadRequests() {
      try {
        setLoading(true);
        setError("");

        const result = await getRequests({
          PageNumber: page + 1,
          PageSize: pageSize,
          SearchTerm: searchTerm || undefined,
          Status: status === "" ? undefined : Number(status),
          AssignedRole: assignedRole === "" ? undefined : Number(assignedRole),
          SortBy: sortBy,
          SortDescending: sortDescending,
        });

        if (!active) return;

        setRequests(result.items);
        setTotalCount(result.totalCount);
      } catch (err) {
        if (!active) return;

        console.error(err);
        setError("Failed to load requests.");
        setRequests([]);
      } finally {
        if (active) setLoading(false);
      }
    }

    void loadRequests();

    return () => {
      active = false;
    };
  }, [
    page,
    pageSize,
    searchTerm,
    status,
    assignedRole,
    sortBy,
    sortDescending,
    reloadKey,
  ]);

  return (
    <Box sx={{ p: { xs: 2, md: 3 } }}>
      <Stack
        direction="row"
        sx={{
          mb: 3,
          justifyContent: "space-between",
          alignItems: "center",
        }}
      >
        <Box>
          <Typography variant="h5" sx={{ fontWeight: 700 }}>
            {assigned ? "Assigned Requests" : "My Requests"}
          </Typography>

          <Typography variant="body2" color="text.secondary">
            Total matching requests: {totalCount}
          </Typography>
        </Box>

        <Button
          variant="outlined"
          onClick={() => setReloadKey((x) => x + 1)}
          disabled={loading}
        >
          Refresh
        </Button>
      </Stack>

      {/* Filters */}
      <Paper sx={{ p: 2, mb: 2, borderRadius: 2 }}>
        <Stack direction={{ xs: "column", md: "row" }} spacing={2}>
          <TextField
            label="Search"
            size="small"
            value={searchTerm}
            placeholder="Title or description"
            onChange={(e) => {
              setSearchTerm(e.target.value);
              setPage(0);
            }}
            sx={{ minWidth: 200, flex: 2 }}
          />

          <FormControl size="small" sx={{ minWidth: 140 }}>
            <InputLabel>Status</InputLabel>
            <Select
              label="Status"
              value={status}
              onChange={(e) => {
                setStatus(e.target.value);
                setPage(0);
              }}
            >
              <MenuItem value="">All Statuses</MenuItem>
              <MenuItem value="1">Created</MenuItem>
              <MenuItem value="2">Pending</MenuItem>
              <MenuItem value="3">Approved</MenuItem>
              <MenuItem value="4">Rejected</MenuItem>
            </Select>
          </FormControl>

          <FormControl size="small" sx={{ minWidth: 140 }}>
            <InputLabel>Assigned To</InputLabel>
            <Select
              label="Assigned To"
              value={assignedRole}
              onChange={(e) => {
                setAssignedRole(e.target.value);
                setPage(0);
              }}
            >
              <MenuItem value="">All Roles</MenuItem>
              <MenuItem value="1">Finance</MenuItem>
              <MenuItem value="2">Manager</MenuItem>
            </Select>
          </FormControl>

          <FormControl size="small" sx={{ minWidth: 150 }}>
            <InputLabel>Sort By</InputLabel>
            <Select
              label="Sort By"
              value={sortBy}
              onChange={(e) => {
                setSortBy(e.target.value);
                setPage(0);
              }}
            >
              <MenuItem value="CreatedAt">Created Date</MenuItem>
              <MenuItem value="Title">Title</MenuItem>
              <MenuItem value="Amount">Amount</MenuItem>
              <MenuItem value="Status">Status</MenuItem>
            </Select>
          </FormControl>

          <Button
            variant="outlined"
            onClick={() => {
              setSortDescending((current) => !current);
              setPage(0);
            }}
          >
            {sortDescending ? "Descending" : "Ascending"}
          </Button>
        </Stack>
      </Paper>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      <Paper sx={{ overflow: "hidden", borderRadius: 2 }}>
        {loading ? (
          <Box
            sx={{
              p: 6,
              display: "flex",
              justifyContent: "center",
            }}
          >
            <CircularProgress />
          </Box>
        ) : (
          <TableContainer>
            <Table sx={{ minWidth: 850 }}>
              <TableHead>
                <TableRow sx={{ bgcolor: "action.hover" }}>
                  <TableCell>Title</TableCell>
                  <TableCell>Description</TableCell>
                  <TableCell align="right">Amount</TableCell>
                  <TableCell>Status</TableCell>
                  <TableCell>Assigned To</TableCell>
                  <TableCell>Created At</TableCell>
                </TableRow>
              </TableHead>

              <TableBody>
                {requests.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={6} align="center">
                      <Typography color="text.secondary" sx={{ py: 4 }}>
                        No requests found.
                      </Typography>
                    </TableCell>
                  </TableRow>
                ) : (
                  requests.map((request) => (
                    <TableRow key={request.id} hover>
                      <TableCell sx={{ fontWeight: 500 }}>
                        {request.title}
                      </TableCell>

                      <TableCell sx={{ maxWidth: 250 }}>
                        <Typography
                          variant="body2"
                          noWrap
                          title={request.description}
                        >
                          {request.description}
                        </Typography>
                      </TableCell>

                      <TableCell align="right">
                        {request.amount.toLocaleString()}
                      </TableCell>

                      <TableCell>
                        <Chip
                          label={getStatusLabel(request.status)}
                          color={getStatusColor(request.status)}
                          size="small"
                        />
                      </TableCell>

                      <TableCell>
                        {getRoleLabel(request.assignedRole)}
                      </TableCell>

                      <TableCell>
                        {new Date(request.createdAt).toLocaleString()}
                      </TableCell>
                    </TableRow>
                  ))
                )}
              </TableBody>
            </Table>
          </TableContainer>
        )}

        <TablePagination
          component="div"
          count={totalCount}
          page={page}
          rowsPerPage={pageSize}
          rowsPerPageOptions={[5, 10, 20]}
          onPageChange={(_, newPage) => setPage(newPage)}
          onRowsPerPageChange={(e) => {
            setPageSize(Number(e.target.value));
            setPage(0);
          }}
        />
      </Paper>
    </Box>
  );
}
