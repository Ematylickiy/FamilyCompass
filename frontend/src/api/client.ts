const client = {
  get: async <T>(baseUrl: string): Promise<T> => {
    const response = await fetch(`api/v1/${baseUrl}`);
    return response.json() as Promise<T>;
  },
  post: async <T>(
    baseUrl: string,
    body: Record<string, unknown>,
  ): Promise<T> => {
    const response = await fetch(`api/v1/${baseUrl}`, {
      body: JSON.stringify(body),
      method: 'post',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    return response.json();
  },
  put: async <T>(
    baseUrl: string,
    body: Record<string, unknown>,
  ): Promise<T> => {
    const response = await fetch(`api/v1/${baseUrl}`, {
      body: JSON.stringify(body),
      method: 'put',
    });
    return response.json();
  },
  delete: async (baseUrl: string): Promise<Response> => {
    const response = await fetch(`api/v1/${baseUrl}`, {
      method: 'delete',
    });
    return response;
  },
};

export default client;
