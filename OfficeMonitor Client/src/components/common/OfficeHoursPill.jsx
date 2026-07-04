import { isAfterHours } from '../../utils/officeHours';

export default function OfficeHoursPill({ now }) {
  const afterHours = isAfterHours(now);
  return (
    <span className={`badge badge-soft font-data text-[11px] ${afterHours ? 'badge-warning' : 'badge-secondary'}`}>
      {afterHours ? '◐ AFTER HOURS' : '● OFFICE HOURS'}
    </span>
  );
}