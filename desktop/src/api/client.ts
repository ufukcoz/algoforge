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

// ---- Question modulu ----

export interface Category {
  id: string;
  name: string;
  questionCount: number;
}

export interface QuestionSummary {
  id: string;
  title: string;
  difficulty: string;
  categoryName: string;
}

export interface PagedQuestions {
  items: QuestionSummary[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface VisibleTestCase {
  input: string;
  expectedOutput: string;
}

export interface QuestionDetail {
  id: string;
  title: string;
  difficulty: string;
  description: string;
  timeLimitMs: number;
  memoryLimitMb: number;
  categoryName: string;
  exampleTestCases: VisibleTestCase[];
}

export async function getCategories(): Promise<Category[]> {
  const response = await fetch(`${API_BASE_URL}/categories`);
  return handleResponse<Category[]>(response);
}

export interface GetQuestionsParams {
  categoryId?: string;
  difficulty?: string;
  page?: number;
  pageSize?: number;
}

export async function getQuestions(params: GetQuestionsParams = {}): Promise<PagedQuestions> {
  const query = new URLSearchParams();
  if (params.categoryId) query.set('categoryId', params.categoryId);
  if (params.difficulty) query.set('difficulty', params.difficulty);
  query.set('page', String(params.page ?? 1));
  query.set('pageSize', String(params.pageSize ?? 20));

  const response = await fetch(`${API_BASE_URL}/questions?${query.toString()}`);
  return handleResponse<PagedQuestions>(response);
}

export async function getQuestionById(id: string): Promise<QuestionDetail> {
  const response = await fetch(`${API_BASE_URL}/questions/${id}`);
  return handleResponse<QuestionDetail>(response);
}
