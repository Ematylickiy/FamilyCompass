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

const client = {
  get: async <T>(path: string): Promise<T> => {
    const response = await fetch(`api/v1/${path}`);
    return parseJson<T>(response);
  },

  post: async <T>(path: string, body: Record<string, unknown>): Promise<T> => {
    const response = await fetch(`api/v1/${path}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(body),
    });
    return parseJson<T>(response);
  },

  delete: async (path: string): Promise<void> => {
    const response = await fetch(`api/v1/${path}`, { method: 'DELETE' });
    if (!response.ok) {
      const text = await response.text();
      throw new Error(text || `${response.status} ${response.statusText}`);
    }
  },
};

export default client;
