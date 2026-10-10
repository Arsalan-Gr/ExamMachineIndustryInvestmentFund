import { useState } from "react";
import type { FormEvent } from "react";
import {
  Alert,
  Box,
  Button,
  CircularProgress,
  Paper,
  Snackbar,
  Stack,
  TextField,
  Typography,
} from "@mui/material";

import requestFormConfig from "../config/requestFormConfig.json";
import { createRequest } from "../api/requestApi";

type FormValues = {
  title: string;
  description: string;
  amount: string;
};

type FieldName = keyof FormValues;
type FormErrors = Partial<Record<FieldName, string>>;

const initialValues: FormValues = {
  title: "",
  description: "",
  amount: "",
};

export default function CreateRequestPage() {
  const [values, setValues] = useState<FormValues>(initialValues);
  const [errors, setErrors] = useState<FormErrors>({});
  const [loading, setLoading] = useState(false);

  const [notification, setNotification] = useState({
    open: false,
    message: "",
    severity: "success" as "success" | "error",
  });

  const validateField = (name: FieldName, value: string): string => {
    const field = requestFormConfig.find((item) => item.name === name);

    if (!field) return "";

    if (field.required && !value.trim()) {
      return `${field.label} is required`;
    }

    if ("maxLength" in field && field.maxLength) {
      if (value.length > field.maxLength) {
        return `Maximum ${field.maxLength} characters allowed`;
      }
    }

    if (name === "amount" && value.trim()) {
      const amount = Number(value);

      if (!Number.isFinite(amount) || amount <= 0) {
        return "Amount must be greater than zero";
      }
    }

    return "";
  };

  const handleChange = (name: FieldName, value: string) => {
    setValues((prev) => ({
      ...prev,
      [name]: value,
    }));

    setErrors((prev) => ({
      ...prev,
      [name]: validateField(name, value),
    }));
  };

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault();

    if (loading) return;

    const newErrors: FormErrors = {};

    requestFormConfig.forEach((field) => {
      const name = field.name as FieldName;
      const error = validateField(name, values[name]);

      if (error) {
        newErrors[name] = error;
      }
    });

    setErrors(newErrors);

    if (Object.keys(newErrors).length > 0) {
      return;
    }

    setLoading(true);

    try {
      await createRequest({
        title: values.title.trim(),
        description: values.description.trim(),
        amount: Number(values.amount),
      });

      setNotification({
        open: true,
        message: "Request created successfully!",
        severity: "success",
      });

      setValues({ ...initialValues });
      setErrors({});
    } catch (error) {
      console.error("Create request failed:", error);

      setNotification({
        open: true,
        message: "Failed to create request. Please try again.",
        severity: "error",
      });
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      <Paper
        sx={{
          p: 3,
          maxWidth: 700,
          mx: "auto",
          mt: 3,
        }}
      >
        <Typography variant="h5" sx={{ mb: 3 }}>
          Create Request
        </Typography>

        <Box component="form" noValidate onSubmit={handleSubmit}>
          <Stack spacing={3}>
            {requestFormConfig.map((field) => {
              const name = field.name as FieldName;

              return (
                <TextField
                  key={field.name}
                  name={field.name}
                  label={field.label}
                  type={field.type === "number" ? "number" : "text"}
                  required={field.required}
                  multiline={field.type === "textarea"}
                  rows={"rows" in field ? field.rows : undefined}
                  value={values[name]}
                  disabled={loading}
                  onChange={(event) => handleChange(name, event.target.value)}
                  onBlur={() =>
                    setErrors((prev) => ({
                      ...prev,
                      [name]: validateField(name, values[name]),
                    }))
                  }
                  error={Boolean(errors[name])}
                  helperText={errors[name] || " "}
                  slotProps={{
                    htmlInput: {
                      min: field.type === "number" ? 0 : undefined,
                      step: field.type === "number" ? "any" : undefined,
                    },
                  }}
                  fullWidth
                />
              );
            })}

            <Box
              sx={{
                display: "flex",
                justifyContent: "flex-end",
              }}
            >
              <Button
                type="submit"
                variant="contained"
                disabled={loading}
                startIcon={
                  loading ? (
                    <CircularProgress size={18} color="inherit" />
                  ) : undefined
                }
              >
                {loading ? "Submitting..." : "Submit Request"}
              </Button>
            </Box>
          </Stack>
        </Box>
      </Paper>

      <Snackbar
        open={notification.open}
        autoHideDuration={4000}
        onClose={() => setNotification((prev) => ({ ...prev, open: false }))}
        anchorOrigin={{
          vertical: "bottom",
          horizontal: "center",
        }}
      >
        <Alert
          severity={notification.severity}
          onClose={() => setNotification((prev) => ({ ...prev, open: false }))}
          variant="filled"
          sx={{ width: "100%" }}
        >
          {notification.message}
        </Alert>
      </Snackbar>
    </>
  );
}
