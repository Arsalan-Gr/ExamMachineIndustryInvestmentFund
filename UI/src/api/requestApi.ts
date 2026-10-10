import api from "./axios";

export interface CreateRequestDto {
  title: string;
  description: string;
  amount: number;
}

export async function createRequest(dto: CreateRequestDto): Promise<void> {
  if (import.meta.env.VITE_MOCK_REQUESTS === "true") {
    await new Promise((resolve) => setTimeout(resolve, 500));
    console.log("MOCK request created:", dto);
    return;
  }

  await api.post("/requests", dto);
}

export interface RequestDto {
  id: string;
  title: string;
  description: string;
  amount: number;
  status: number | string;
  assignedRole: number | string;
  createdByUserId: string;
  reviewedByUserId: string | null;
  createdAt: string;
  reviewedAt: string | null;
}

export interface PagedResultDto<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

export interface RequestFilterDto {
  Status?: number;
  AssignedRole?: number;
  CreatedFrom?: string;
  CreatedTo?: string;
  MinAmount?: number;
  MaxAmount?: number;
  SearchTerm?: string;
  SortBy?: string;
  SortDescending?: boolean;
  PageNumber?: number;
  PageSize?: number;
}

export async function getRequests(
  filters: RequestFilterDto = {},
): Promise<PagedResultDto<RequestDto>> {
  const response = await api.get<PagedResultDto<RequestDto>>("/requests", {
    params: filters,
  });

  return response.data;
}
