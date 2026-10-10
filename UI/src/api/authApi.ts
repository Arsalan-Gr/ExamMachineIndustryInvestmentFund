import api from "./axios";
import type { LoginRequest, LoginResponse } from "../types/auth";
import { authStorage } from "../utils/authStorage";

export const authApi = {
  async login(data: LoginRequest): Promise<LoginResponse> {
    const response = await api.post<LoginResponse>("/Auth/login", data);

    authStorage.save(response.data);
    return response.data;
  },
};
