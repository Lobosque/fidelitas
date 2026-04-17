interface WalletCardPreviewProps {
  logoUrl: string | null
  corPrimaria: string
  corSecundaria: string
  nomeNegocio: string
  nomeCampanha: string
  checkinsNecessarios: number
  descricaoPremio: string
}

function isLightColor(hex: string): boolean {
  const r = parseInt(hex.slice(1, 3), 16)
  const g = parseInt(hex.slice(3, 5), 16)
  const b = parseInt(hex.slice(5, 7), 16)
  const luminance = (0.299 * r + 0.587 * g + 0.114 * b) / 255
  return luminance > 0.6
}

export function WalletCardPreview({
  logoUrl,
  corPrimaria,
  corSecundaria,
  nomeNegocio,
  nomeCampanha,
  checkinsNecessarios,
  descricaoPremio,
}: WalletCardPreviewProps) {
  const textColor = isLightColor(corPrimaria) ? '#1f2937' : '#ffffff'
  const subtextColor = isLightColor(corPrimaria) ? '#4b5563' : 'rgba(255,255,255,0.8)'

  return (
    <div
      className="aspect-[1.586/1] w-full max-w-sm overflow-hidden rounded-2xl p-5 shadow-lg"
      style={{
        background: `linear-gradient(135deg, ${corPrimaria}, ${corSecundaria})`,
        color: textColor,
      }}
    >
      <div className="flex h-full flex-col justify-between">
        {/* Header */}
        <div className="flex items-center gap-3">
          {logoUrl ? (
            <img
              src={logoUrl}
              alt="Logo"
              className="size-10 rounded-full bg-white/20 object-cover"
            />
          ) : (
            <div className="flex size-10 items-center justify-center rounded-full bg-white/20 text-sm font-bold">
              {nomeNegocio.charAt(0).toUpperCase()}
            </div>
          )}
          <div>
            <p className="text-sm font-bold">{nomeNegocio || 'Seu Negócio'}</p>
            <p className="text-xs" style={{ color: subtextColor }}>
              Cartão Fidelidade
            </p>
          </div>
        </div>

        {/* Campaign name */}
        <div className="text-center">
          <p className="text-lg font-bold">{nomeCampanha || 'Sua Campanha'}</p>
        </div>

        {/* Reward info */}
        <div className="text-center">
          <p className="text-xs" style={{ color: subtextColor }}>
            {checkinsNecessarios} check-ins → {descricaoPremio || 'seu prêmio'}
          </p>
          <div className="mt-2 flex justify-center gap-1">
            {Array.from({ length: Math.min(checkinsNecessarios, 12) }).map((_, i) => (
              <div
                key={i}
                className="size-2.5 rounded-full"
                style={{ backgroundColor: `${textColor}40` }}
              />
            ))}
            {checkinsNecessarios > 12 && (
              <span className="text-xs" style={{ color: subtextColor }}>
                +{checkinsNecessarios - 12}
              </span>
            )}
          </div>
        </div>
      </div>
    </div>
  )
}
