import { BracketStatus } from './BracketStatus';
import { BracketType } from './BracketType';
import { Competition } from './Competition';

export interface Bracket {
  id: number;
  name: string;
  game: string;
  isLocked: boolean;
  type: BracketType;
  status: BracketStatus;
  competition: Competition;
  startDate: string;
  createdDate: string;
}
