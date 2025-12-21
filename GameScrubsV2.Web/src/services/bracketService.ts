import { API_BASE_URL } from '@/api.config';
import { type Bracket } from '@/models/Bracket';
import type { BracketPosition } from '@/models/BracketPosition';
import type { BracketType } from '@/models/BracketType';
import { getErrorMessage } from './apiErrorHandler';

export interface PaginatedBracketResponse {
  brackets: Bracket[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export const bracketService = {
  // GET all items with optional pagination and filters
  async search(params?: {
    pageNumber?: number;
    pageSize?: number;
    name?: string;
    game?: string;
    type?: string;
    status?: string;
    competition?: string;
  }): Promise<PaginatedBracketResponse> {
    const body: any = {};

    if (params) {
      if (params.pageNumber !== undefined) body.pageNumber = params.pageNumber;
      if (params.pageSize !== undefined) body.pageSize = params.pageSize;
      if (params.name) body.name = params.name;
      if (params.game) body.game = params.game;
      if (params.type) body.type = params.type;
      if (params.status) body.status = params.status;
      if (params.competition) body.competition = params.competition;
    }

    const response = await fetch(`${API_BASE_URL}/brackets/search`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: Object.keys(body).length > 0 ? JSON.stringify(body) : undefined,
    });
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
  async create(item: {
    name: string;
    game: string;
    type: string;
    competition: string;
    startDate: string;
    email?: string;
    url?: string;
    lockCode?: string;
  }): Promise<Bracket> {
    const response = await fetch(`${API_BASE_URL}/brackets`, {
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
  async update(
    item: {
      id: number;
      name: string;
      game: string;
      type: string;
      competition: string;
      startDate: string;
      email?: string;
      url?: string;
      lockCode?: string;
    },
    lockCode?: string,
  ): Promise<Bracket> {
    const url = lockCode ? `${API_BASE_URL}/brackets/${lockCode}` : `${API_BASE_URL}/brackets`;

    const response = await fetch(url, {
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

  // POST change bracket status
  async changeStatus(bracketId: number, status: string, lockCode?: string): Promise<Bracket> {
    const url = lockCode
      ? `${API_BASE_URL}/brackets/change-status/${lockCode}`
      : `${API_BASE_URL}/brackets/change-status`;

    const response = await fetch(url, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ bracketId, status }),
    });
    if (!response.ok) {
      const errorMessage = await getErrorMessage(response);
      throw new Error(errorMessage);
    }
    return response.json();
  },

  // GET bracket positions by type
  async getPositionsByType(bracketType: BracketType): Promise<BracketPosition[]> {
    const response = await fetch(`${API_BASE_URL}/brackets/positions/${bracketType}`);

    if (!response.ok) {
      const errorMessage = await getErrorMessage(response);
      throw new Error(errorMessage);
    }

    return response.json();
  },
};
