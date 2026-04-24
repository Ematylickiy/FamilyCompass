import { getStoredAccessToken } from '../../auth/tokenStorage';

/** Base path for backend JSON API (Vite proxy → backend). */
export const API_V1_PREFIX = 'api/v1/';

async function parseJson<T>(response: Response): Promise<T> {
  const text = await response.text();
  if (!response.ok) {
    throw new Error(text || `${response.status} ${response.statusText}`);
  }
  if (!text) {
    return undefined as T;
  }
  return JSON.parse(text) as T;
}

function authHeader(): HeadersInit {
  const token = getStoredAccessToken();
  return token ? { Authorization: `Bearer ${token}` } : {};
}

/** Authenticated JSON client for protected routes. */
const client = {
  get: async <T>(path: string): Promise<T> => {
    const response = await fetch(`${API_V1_PREFIX}${path}`, {
      headers: { ...authHeader() },
    });
    return parseJson<T>(response);
  },

  post: async <T>(path: string, body: Record<string, unknown>): Promise<T> => {
    const response = await fetch(`${API_V1_PREFIX}${path}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        ...authHeader(),
      },
      body: JSON.stringify(body),
    });
    return parseJson<T>(response);
  },

  delete: async (path: string): Promise<void> => {
    const response = await fetch(`${API_V1_PREFIX}${path}`, {
      method: 'DELETE',
      headers: { ...authHeader() },
    });
    if (!response.ok) {
      const text = await response.text();
      throw new Error(text || `${response.status} ${response.statusText}`);
    }
  },
};

export default client;
