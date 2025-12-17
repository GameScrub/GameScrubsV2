export const PlayerPlaceholder = {
  BYE: '--',
  TBD: 'TBD',
} as const;

export type PlayerPlaceholderType = (typeof PlayerPlaceholder)[keyof typeof PlayerPlaceholder];
