export const OFFICE_START_HOUR = 9;
export const OFFICE_END_HOUR = 17;

export function isAfterHours(date = new Date()) {
  const hour = date.getHours();
  return hour < OFFICE_START_HOUR || hour >= OFFICE_END_HOUR;
}