import { API_BASE_URL } from '@/api.config';
import { type Bracket } from '@/models/Bracket';
import { getErrorMessage } from './apiErrorHandler';

export const bracketService = {
  // GET all items
  async getAll(): Promise<Bracket[]> {
    const response = await fetch(`${API_BASE_URL}/brackets`);
    if (!response.ok) {
      const errorMessage = await getErrorMessage(response);
      throw new Error(errorMessage);
    }
    return response.json();
  },

  // GET single item
  async getById(id: number): Promise<Bracket> {
    const response = await fetch(`${API_BASE_URL}/brackets/${id}`);
    if (!response.ok) {
      const errorMessage = await getErrorMessage(response);
      throw new Error(errorMessage);
    }
    return response.json();
  },

  // POST create new item
  async create(item: Omit<Bracket, 'id' | 'createdAt'>): Promise<Bracket> {
    const response = await fetch(`${API_BASE_URL}/items`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(item),
    });
    if (!response.ok) {
      const errorMessage = await getErrorMessage(response);
      throw new Error(errorMessage);
    }
    return response.json();
  },

  // PUT update item
  async update(id: number, item: Partial<Bracket>): Promise<Bracket> {
    const response = await fetch(`${API_BASE_URL}/items/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(item),
    });
    if (!response.ok) {
      const errorMessage = await getErrorMessage(response);
      throw new Error(errorMessage);
    }
    return response.json();
  },

  // DELETE item
  async delete(id: number): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/items/${id}`, {
      method: 'DELETE',
    });
    if (!response.ok) {
      const errorMessage = await getErrorMessage(response);
      throw new Error(errorMessage);
    }
  },
};
