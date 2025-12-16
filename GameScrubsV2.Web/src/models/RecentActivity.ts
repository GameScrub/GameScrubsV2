import { BracketStatus } from './BracketStatus';
import { BracketType } from './BracketType';
import { Competition } from './Competition';

export interface RecentActivityBracket {
  id: number;
  name: string | null;
  game: string | null;
  isLocked: boolean;
  type: BracketType;
  status: BracketStatus;
  competition: Competition;
  startDate: string;
}

export interface RecentActivityGroup {
  key: BracketStatus;
  brackets: RecentActivityBracket[];
}
