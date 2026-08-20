// Backend API'nin adresi. GeliÅŸtirme sÄ±rasÄ±nda local ASP.NET Core sunucusu.
// Vite build sirasinda VITE_API_BASE_URL ortam degiskeni verilirse onu kullanir,
// verilmezse local gelistirme icin localhost:5000'e duser. Boylece ayni kod hem
// local backend'e hem Render'daki canli API'ye baglanabilir - .env dosyasiyla degistirilir.
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'https://algoforge-api-b3b9.onrender.com/api';

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
    let message = `Ä°stek baÅŸarÄ±sÄ±z oldu (${response.status})`;
    try {
      const body = await response.json();
      message = body?.title ?? body?.message ?? message;
    } catch {
      // JSON gÃ¶vde yoksa varsayÄ±lan mesajÄ± kullan
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

// ---- Judge0 - kod calistirma ve gonderme ----

export interface TestCaseRunResult {
  input: string;
  expectedOutput: string;
  actualOutput: string | null;
  passed: boolean;
  stderr: string | null;
  compileOutput: string | null;
  runtimeMs: number | null;
}

export interface RunCodeResult {
  allPassed: boolean;
  results: TestCaseRunResult[];
}

export interface SubmissionResult {
  submissionId: string;
  status: string;
  passedCount: number;
  totalCount: number;
  runtimeMs: number | null;
  memoryKb: number | null;
}

export async function runCode(
  questionId: string,
  language: string,
  sourceCode: string,
  accessToken: string
): Promise<RunCodeResult> {
  const response = await fetch(`${API_BASE_URL}/questions/${questionId}/run`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${accessToken}`,
    },
    body: JSON.stringify({ language, sourceCode }),
  });
  return handleResponse<RunCodeResult>(response);
}

export async function submitCode(
  questionId: string,
  language: string,
  sourceCode: string,
  accessToken: string
): Promise<SubmissionResult> {
  const response = await fetch(`${API_BASE_URL}/submissions`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${accessToken}`,
    },
    body: JSON.stringify({ questionId, language, sourceCode }),
  });
  return handleResponse<SubmissionResult>(response);
}

// ---- Profile ve Leaderboard ----

export interface ProfileData {
  username: string;
  email: string;
  emailVerified: boolean;
  xp: number;
  level: number;
  memberSince: string;
  totalSubmissions: number;
  acceptedSubmissions: number;
  questionsSolved: number;
}

export interface LeaderboardEntry {
  rank: number;
  username: string;
  xp: number;
  level: number;
}

export async function getProfile(accessToken: string): Promise<ProfileData> {
  const response = await fetch(`${API_BASE_URL}/profile`, {
    headers: { Authorization: `Bearer ${accessToken}` },
  });
  return handleResponse<ProfileData>(response);
}

export async function getLeaderboard(top: number = 50): Promise<LeaderboardEntry[]> {
  const response = await fetch(`${API_BASE_URL}/leaderboard?top=${top}`);
  return handleResponse<LeaderboardEntry[]>(response);
}

// ---- Contest sistemi ----

export interface ContestSummary {
  id: string;
  title: string;
  startTime: string;
  endTime: string;
  isPublic: boolean;
  participantCount: number;
  questionCount: number;
  status: 'Upcoming' | 'Active' | 'Ended';
  isJoined: boolean;
}

export interface ContestQuestionItem {
  questionId: string;
  title: string;
  difficulty: string;
  points: number;
  orderIndex: number;
}

export interface ContestDetail {
  id: string;
  title: string;
  description: string;
  startTime: string;
  endTime: string;
  isPublic: boolean;
  inviteCode: string | null;
  status: 'Upcoming' | 'Active' | 'Ended';
  isJoined: boolean;
  participantCount: number;
  questions: ContestQuestionItem[];
}

export interface ContestLeaderboardEntry {
  rank: number;
  username: string;
  totalPoints: number;
  solvedCount: number;
  totalPenaltySeconds: number;
}

export async function getContests(accessToken: string): Promise<ContestSummary[]> {
  const response = await fetch(`${API_BASE_URL}/contests`, {
    headers: { Authorization: `Bearer ${accessToken}` },
  });
  return handleResponse<ContestSummary[]>(response);
}

export async function getContestById(id: string, accessToken: string): Promise<ContestDetail> {
  const response = await fetch(`${API_BASE_URL}/contests/${id}`, {
    headers: { Authorization: `Bearer ${accessToken}` },
  });
  return handleResponse<ContestDetail>(response);
}

export async function joinContest(
  id: string,
  inviteCode: string | null,
  accessToken: string
): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/contests/${id}/join`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${accessToken}`,
    },
    body: JSON.stringify({ inviteCode }),
  });
  if (!response.ok) {
    let message = 'Yarismaya katilinamadi.';
    try {
      const body = await response.json();
      message = body?.title ?? body?.message ?? message;
    } catch {
      // JSON gÃ¶vde yoksa varsayÄ±lan mesajÄ± kullan
    }
    throw new Error(message);
  }
}

export async function getContestLeaderboard(
  id: string,
  accessToken: string
): Promise<ContestLeaderboardEntry[]> {
  const response = await fetch(`${API_BASE_URL}/contests/${id}/leaderboard`, {
    headers: { Authorization: `Bearer ${accessToken}` },
  });
  return handleResponse<ContestLeaderboardEntry[]>(response);
}

// ---- AI Assistant ----

export type AiAssistAction = 'Hint' | 'ComplexityAnalysis' | 'ExplainBug' | 'ExplainCode' | 'SuggestSolution';

export async function getAiAssistance(
  questionId: string,
  code: string,
  language: string,
  action: AiAssistAction,
  accessToken: string
): Promise<string> {
  const response = await fetch(`${API_BASE_URL}/ai/assist`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${accessToken}`,
    },
    body: JSON.stringify({ questionId, code, language, action }),
  });
  const data = await handleResponse<{ message: string }>(response);
  return data.message;
}

// ---- Refresh token ----

export interface RefreshTokenResponse {
  accessToken: string;
  refreshToken: string;
}

export async function refreshAccessToken(refreshToken: string): Promise<RefreshTokenResponse> {
  const response = await fetch(`${API_BASE_URL}/auth/refresh`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ refreshToken }),
  });
  return handleResponse<RefreshTokenResponse>(response);
}

export async function logoutRequest(refreshToken: string): Promise<void> {
  // Best-effort: cikis yaparken sunucu tarafinda da token'i iptal etmeye calisiyoruz,
  // ama basarisiz olsa bile kullaniciyi engellemiyoruz (local state zaten temizlenecek).
  try {
    await fetch(`${API_BASE_URL}/auth/logout`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ refreshToken }),
    });
  } catch {
    // sessizce yut - logout'un kullanici deneyimini engellemesine gerek yok
  }
}

// ---- Email dogrulama ----

export async function resendVerificationEmail(accessToken: string): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/auth/resend-verification`, {
    method: 'POST',
    headers: { Authorization: `Bearer ${accessToken}` },
  });
  if (!response.ok) {
    throw new Error('Dogrulama emaili gonderilemedi.');
  }
}

