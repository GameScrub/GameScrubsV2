import { API_BASE_URL } from '@/api.config';

export type BracketType =
  | 'Single_8'
  | 'Single_16'
  | 'Single_32'
  | 'Double_8'
  | 'Double_16'
  | 'Double_32';
export type BracketStatus = 'Pending' | 'Started' | 'InProgress' | 'Completed' | 'Cancelled';
export type Competition = 'VideoGames' | 'Sports' | 'Esports' | 'Other';

export interface Bracket {
  id: number;
  name: string;
  game: string;
  url: string | null;
  isLocked: boolean;
  type: BracketType;
  status: BracketStatus;
  competition: Competition;
  startDate: string;
  createdDate: string;
}

export const bracketService = {
  // GET all items
  async getAll(): Promise<Bracket[]> {
    const response = await fetch(`${API_BASE_URL}/brackets`);
    if (!response.ok) throw new Error('Failed to fetch brackets');
    return response.json();
  },

  // GET single item
  async getById(id: number): Promise<Bracket> {
    const response = await fetch(`${API_BASE_URL}/items/${id}`);
    if (!response.ok) throw new Error('Failed to fetch item');
    return response.json();
  },

  // POST create new item
  async create(item: Omit<Bracket, 'id' | 'createdAt'>): Promise<Bracket> {
    const response = await fetch(`${API_BASE_URL}/items`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(item),
    });
    if (!response.ok) throw new Error('Failed to create item');
    return response.json();
  },

  // PUT update item
  async update(id: number, item: Partial<Bracket>): Promise<Bracket> {
    const response = await fetch(`${API_BASE_URL}/items/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(item),
    });
    if (!response.ok) throw new Error('Failed to update item');
    return response.json();
  },

  // DELETE item
  async delete(id: number): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/items/${id}`, {
      method: 'DELETE',
    });
    if (!response.ok) throw new Error('Failed to delete item');
  },
};
