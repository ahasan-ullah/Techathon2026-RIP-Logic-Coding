export function formatClock(date) {
  return date.toLocaleTimeString('en-US', { hour12: false });
}

export function formatShortTime(date) {
  return date.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit', hour12: false });
}