import { useCallback, useEffect, useState } from "react";
import {
  Alert,
  Box,
  Button,
  Chip,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  Paper,
  Snackbar,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
} from "@mui/material";

import {
  approveRequest,
  getRequests,
  rejectRequest,
  type RequestDto,
} from "../api/requestApi";

type ReviewAction = "approve" | "reject";

const statusLabels: Record<number, string> = {
  0: "Unknown",
  1: "Created",
  2: "Pending",
  3: "Approved",
  4: "Rejected",
};

const roleLabels: Record<number, string> = {
  0: "Unknown",
  1: "Finance",
  2: "Manager",
};

function getStatusValue(status: number | string): number {
  if (typeof status === "number") return status;

  const parsed = Number(status);
  if (!Number.isNaN(parsed)) return parsed;

  const index = Object.entries(statusLabels).find(
    ([, label]) => label.toLowerCase() === status.toLowerCase(),
  );

  return index ? Number(index[0]) : 0;
}

function getStatusLabel(status: number | string): string {
  const value = getStatusValue(status);
  return statusLabels[value] ?? String(status);
}

function getRoleLabel(role: number | string): string {
  if (typeof role === "number" || !Number.isNaN(Number(role))) {
    return roleLabels[Number(role)] ?? String(role);
  }

  return String(role);
}

function getErrorMessage(error: unknown): string {
  if (error instanceof Error) return error.message;
  return "Operation failed.";
}

export default function ReviewRequestsPage() {
  const [requests, setRequests] = useState<RequestDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [processingId, setProcessingId] = useState<string | null>(null);
  const [selectedRequest, setSelectedRequest] = useState<RequestDto | null>(
    null,
  );
  const [action, setAction] = useState<ReviewAction | null>(null);
  const [error, setError] = useState("");
  const [notification, setNotification] = useState("");

  const loadRequests = useCallback(async () => {
    setLoading(true);
    setError("");

    try {
      const result = await getRequests({
        PageNumber: 1,
        PageSize: 100,
        SortBy: "createdAt",
        SortDescending: true,
      });

      setRequests(result.items);
    } catch (err) {
      setError(getErrorMessage(err));
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    void loadRequests();
  }, [loadRequests]);

  function openConfirmation(request: RequestDto, nextAction: ReviewAction) {
    setSelectedRequest(request);
    setAction(nextAction);
  }

  function closeConfirmation() {
    if (processingId !== null) return;

    setSelectedRequest(null);
    setAction(null);
  }

  async function confirmAction() {
    if (!selectedRequest || !action || processingId !== null) return;

    const requestId = selectedRequest.id;
    const currentAction = action;

    setProcessingId(requestId);

    try {
      if (currentAction === "approve") {
        await approveRequest(requestId);
      } else {
        await rejectRequest(requestId);
      }

      setSelectedRequest(null);
      setAction(null);

      setNotification(
        currentAction === "approve"
          ? "Request approved successfully."
          : "Request rejected successfully.",
      );

      await loadRequests();
    } catch (err) {
      setError(getErrorMessage(err));
      setSelectedRequest(null);
      setAction(null);
    } finally {
      setProcessingId(null);
    }
  }

  return (
    <Box sx={{ p: 3 }}>
      <Stack
        direction={{ xs: "column", sm: "row" }}
        spacing={2}
        sx={{
          justifyContent: "space-between",
          alignItems: { xs: "flex-start", sm: "center" },
          mb: 3,
        }}
      >
        <Typography variant="h5" sx={{ fontWeight: 700 }}>
          Review Requests
        </Typography>

        <Button
          variant="outlined"
          onClick={() => void loadRequests()}
          disabled={loading || processingId !== null}
        >
          Refresh
        </Button>
      </Stack>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError("")}>
          {error}
        </Alert>
      )}

      {loading ? (
        <Box sx={{ display: "flex", justifyContent: "center", p: 5 }}>
          <CircularProgress />
        </Box>
      ) : requests.length === 0 ? (
        <Alert severity="info">No requests found.</Alert>
      ) : (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Title</TableCell>
                <TableCell>Amount</TableCell>
                <TableCell>Status</TableCell>
                <TableCell>Assigned To</TableCell>
                <TableCell>Created At</TableCell>
                <TableCell>Actions</TableCell>
              </TableRow>
            </TableHead>

            <TableBody>
              {requests.map((request) => {
                const status = getStatusValue(request.status);
                const canReview = status === 1 || status === 2;
                const busy = processingId !== null;

                return (
                  <TableRow key={request.id}>
                    <TableCell>
                      <Typography sx={{ fontWeight: 600 }}>
                        {request.title}
                      </Typography>
                      <Typography variant="body2" color="text.secondary">
                        {request.description}
                      </Typography>
                    </TableCell>

                    <TableCell>{request.amount.toLocaleString()}</TableCell>

                    <TableCell>
                      <Chip
                        size="small"
                        label={getStatusLabel(request.status)}
                        color={
                          status === 3
                            ? "success"
                            : status === 4
                              ? "error"
                              : "warning"
                        }
                      />
                    </TableCell>

                    <TableCell>{getRoleLabel(request.assignedRole)}</TableCell>

                    <TableCell>
                      {new Date(request.createdAt).toLocaleString()}
                    </TableCell>

                    <TableCell>
                      {canReview ? (
                        <Stack direction="row" spacing={1}>
                          <Button
                            size="small"
                            variant="contained"
                            color="success"
                            disabled={busy}
                            onClick={() => openConfirmation(request, "approve")}
                          >
                            Approve
                          </Button>

                          <Button
                            size="small"
                            variant="outlined"
                            color="error"
                            disabled={busy}
                            onClick={() => openConfirmation(request, "reject")}
                          >
                            Reject
                          </Button>
                        </Stack>
                      ) : (
                        <Typography variant="body2" color="text.secondary">
                          Reviewed
                        </Typography>
                      )}
                    </TableCell>
                  </TableRow>
                );
              })}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      <Dialog
        open={selectedRequest !== null}
        onClose={closeConfirmation}
        fullWidth
        maxWidth="xs"
      >
        <DialogTitle>
          {action === "approve" ? "Approve Request" : "Reject Request"}
        </DialogTitle>

        <DialogContent>
          <DialogContentText>
            Are you sure you want to {action} "{selectedRequest?.title}"?
          </DialogContentText>
        </DialogContent>

        <DialogActions>
          <Button onClick={closeConfirmation} disabled={processingId !== null}>
            Cancel
          </Button>

          <Button
            onClick={() => void confirmAction()}
            disabled={processingId !== null}
            variant="contained"
            color={action === "approve" ? "success" : "error"}
          >
            {processingId !== null ? "Processing..." : "Confirm"}
          </Button>
        </DialogActions>
      </Dialog>

      <Snackbar
        open={notification !== ""}
        autoHideDuration={4000}
        onClose={() => setNotification("")}
        message={notification}
      />
    </Box>
  );
}
