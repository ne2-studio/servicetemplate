import { Item } from './types';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000';

let _accessToken: string | undefined;

export function setAccessToken(token: string | undefined) {
  _accessToken = token;
}

const getHeaders = (): Record<string, string> => ({
  'Content-Type': 'application/json',
  'Authorization': `Bearer ${_accessToken}`,
});

const handleResponse = async (res: Response) => {
  if (!res.ok) {
    const error = await res.json().catch(() => ({ message: 'Unknown error' }));
    throw new Error(error.message || `API Error: ${res.status}`);
  }
  return res.json();
};

export const api = {
  items: {
    getAll: async (): Promise<Item[]> =>
      fetch(`${API_BASE_URL}/items`, { headers: getHeaders() }).then(handleResponse).then(data => data.map((i: any) => new Item(i))),
    create: async (item: Omit<Item, 'id' | 'createdAt'>): Promise<Item> =>
      fetch(`${API_BASE_URL}/items`, {
        method: 'POST',
        headers: getHeaders(),
        body: JSON.stringify(item),
      }).then(handleResponse).then(data => new Item(data)),
    update: async (id: string, updates: Partial<Item>): Promise<void> =>
      fetch(`${API_BASE_URL}/items/${id}`, {
        method: 'PATCH',
        headers: getHeaders(),
        body: JSON.stringify(updates),
      }).then(handleResponse),
    delete: async (id: string): Promise<void> =>
      fetch(`${API_BASE_URL}/items/${id}`, {
        method: 'DELETE',
        headers: getHeaders(),
      }).then(handleResponse),
  },
};
