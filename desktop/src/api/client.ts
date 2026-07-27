// Backend API'nin adresi. Geliştirme sırasında local ASP.NET Core sunucusu.
const API_BASE_URL = 'http://localhost:5000/api';

export interface RegisterPayload {
  username: string;
  email: string;
  password: string;
}

export interface LoginPayload {
  email: string;
  password: string;
}

export interface RegisterResponse {
  userId: string;
  username: string;
  email: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  username: string;
}

class ApiError extends Error {
  status: number;
  constructor(message: string, status: number) {
    super(message);
    this.status = status;
  }
}

async function handleResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    let message = `İstek başarısız oldu (${response.status})`;
    try {
      const body = await response.json();
      message = body?.title ?? body?.message ?? message;
    } catch {
      // JSON gövde yoksa varsayılan mesajı kullan
    }
    throw new ApiError(message, response.status);
  }
  return response.json() as Promise<T>;
}

export async function register(payload: RegisterPayload): Promise<RegisterResponse> {
  const response = await fetch(`${API_BASE_URL}/auth/register`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  });
  return handleResponse<RegisterResponse>(response);
}

export async function login(payload: LoginPayload): Promise<LoginResponse> {
  const response = await fetch(`${API_BASE_URL}/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  });
  return handleResponse<LoginResponse>(response);
}
