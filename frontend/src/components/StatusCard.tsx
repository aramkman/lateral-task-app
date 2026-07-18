interface StatusCardProps {
  icon: string
  title: string
  subtitle: string
}

/** Shared bordered-card layout for empty and error states: icon, title, subtitle. */
export function StatusCard({ icon, title, subtitle }: StatusCardProps) {
  return (
    <div className="overflow-hidden rounded-xl border border-black/5 bg-white px-6 py-18 text-center shadow-[0_1px_3px_rgba(0,0,0,0.05)]">
      <div className="mb-3 text-[40px] opacity-40">{icon}</div>
      <div className="mb-1 text-[19px] font-medium text-[#1D1D1F]">{title}</div>
      <div className="text-[15px] text-[#86868B]">{subtitle}</div>
    </div>
  )
}
