import { QRCodeSVG } from 'qrcode.react'

interface TotemPreviewProps {
  campaignId: string
  nomeNegocio: string
  nomeCampanha: string
  descricaoPremio: string
  checkinsNecessarios: number
  corPrimaria: string
  logoUrl: string | null
  printable?: boolean
}

export function TotemPreview({
  campaignId,
  nomeNegocio,
  nomeCampanha,
  descricaoPremio,
  checkinsNecessarios,
  corPrimaria,
  logoUrl,
  printable,
}: TotemPreviewProps) {
  const qrUrl = `${window.location.origin}/c/${campaignId}`

  return (
    <div
      id={printable ? 'totem-print-area' : undefined}
      className="mx-auto w-full max-w-xs rounded-2xl border border-gray-200 bg-white p-6 text-center shadow-sm"
    >
      {/* Header */}
      <div className="mb-4 flex items-center justify-center gap-2">
        {logoUrl ? (
          <img src={logoUrl} alt="Logo" className="size-8 rounded-full object-cover" />
        ) : (
          <div
            className="flex size-8 items-center justify-center rounded-full text-xs font-bold text-white"
            style={{ backgroundColor: corPrimaria }}
          >
            {nomeNegocio.charAt(0).toUpperCase()}
          </div>
        )}
        <p className="text-lg font-bold text-gray-900">{nomeNegocio}</p>
      </div>

      {/* Campaign name */}
      <p className="mb-1 text-sm font-semibold" style={{ color: corPrimaria }}>
        {nomeCampanha}
      </p>

      {/* QR Code */}
      <div className="my-4 flex justify-center">
        <QRCodeSVG
          value={qrUrl}
          size={180}
          level="M"
          fgColor={corPrimaria}
        />
      </div>

      {/* Rule */}
      <p className="text-sm text-gray-600">
        Junte <span className="font-semibold">{checkinsNecessarios} check-ins</span> e ganhe{' '}
        <span className="font-semibold">{descricaoPremio}</span>
      </p>

      {/* Footer */}
      <p className="mt-4 text-xs text-gray-400">Powered by Voltei</p>
    </div>
  )
}
