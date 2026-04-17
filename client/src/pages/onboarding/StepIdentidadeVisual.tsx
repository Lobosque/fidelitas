import { useRef } from 'react'
import { WalletCardPreview } from '../../components/onboarding/WalletCardPreview'

const COLOR_PRESETS = [
  { primaria: '#4f46e5', secundaria: '#818cf8', nome: 'Indigo' },
  { primaria: '#0891b2', secundaria: '#22d3ee', nome: 'Ciano' },
  { primaria: '#059669', secundaria: '#34d399', nome: 'Esmeralda' },
  { primaria: '#d97706', secundaria: '#fbbf24', nome: 'Âmbar' },
  { primaria: '#dc2626', secundaria: '#f87171', nome: 'Vermelho' },
  { primaria: '#7c3aed', secundaria: '#a78bfa', nome: 'Violeta' },
  { primaria: '#db2777', secundaria: '#f472b6', nome: 'Pink' },
  { primaria: '#1d4ed8', secundaria: '#60a5fa', nome: 'Azul' },
]

interface StepIdentidadeVisualProps {
  logoPreviewUrl: string | null
  corPrimaria: string
  corSecundaria: string
  nomeNegocio: string
  nomeCampanha: string
  checkinsNecessarios: number
  descricaoPremio: string
  onLogoChange: (file: File, previewUrl: string) => void
  onCoresChange: (primaria: string, secundaria: string) => void
}

export function StepIdentidadeVisual({
  logoPreviewUrl,
  corPrimaria,
  corSecundaria,
  nomeNegocio,
  nomeCampanha,
  checkinsNecessarios,
  descricaoPremio,
  onLogoChange,
  onCoresChange,
}: StepIdentidadeVisualProps) {
  const fileInputRef = useRef<HTMLInputElement>(null)

  function handleFileChange(e: React.ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0]
    if (!file) return
    const previewUrl = URL.createObjectURL(file)
    onLogoChange(file, previewUrl)
  }

  return (
    <div className="grid gap-10 lg:grid-cols-2">
      {/* Preview — aparece primeiro no mobile */}
      <div className="order-first flex items-start justify-center lg:order-last">
        <WalletCardPreview
          logoUrl={logoPreviewUrl}
          corPrimaria={corPrimaria}
          corSecundaria={corSecundaria}
          nomeNegocio={nomeNegocio}
          nomeCampanha={nomeCampanha}
          checkinsNecessarios={checkinsNecessarios}
          descricaoPremio={descricaoPremio}
        />
      </div>

      {/* Form */}
      <div className="space-y-8">
        <div>
          <h2 className="text-lg font-semibold text-gray-900">Identidade Visual</h2>
          <p className="mt-1 text-sm text-gray-500">
            Personalize as cores e logo do seu cartão de fidelidade.
          </p>
        </div>

        {/* Logo upload */}
        <div>
          <label className="block text-sm/6 font-medium text-gray-900">
            Logo <span className="font-normal text-gray-400">(opcional)</span>
          </label>
          <div className="mt-2">
            <input
              ref={fileInputRef}
              type="file"
              accept="image/*"
              onChange={handleFileChange}
              className="hidden"
            />
            {logoPreviewUrl ? (
              <div className="flex items-center gap-4">
                <img
                  src={logoPreviewUrl}
                  alt="Logo preview"
                  className="size-16 rounded-full object-cover"
                />
                <button
                  type="button"
                  onClick={() => fileInputRef.current?.click()}
                  className="cursor-pointer text-sm font-semibold text-primary hover:text-secondary"
                >
                  Trocar
                </button>
              </div>
            ) : (
              <button
                type="button"
                onClick={() => fileInputRef.current?.click()}
                className="flex w-full cursor-pointer justify-center rounded-lg border-2 border-dashed border-gray-300 px-6 py-8 hover:border-gray-400"
              >
                <div className="text-center">
                  <svg
                    className="mx-auto size-10 text-gray-300"
                    viewBox="0 0 24 24"
                    fill="currentColor"
                    aria-hidden="true"
                  >
                    <path
                      fillRule="evenodd"
                      d="M1.5 6a2.25 2.25 0 0 1 2.25-2.25h16.5A2.25 2.25 0 0 1 22.5 6v12a2.25 2.25 0 0 1-2.25 2.25H3.75A2.25 2.25 0 0 1 1.5 18V6ZM3 16.06V18c0 .414.336.75.75.75h16.5A.75.75 0 0 0 21 18v-1.94l-2.69-2.689a1.5 1.5 0 0 0-2.12 0l-.88.879.97.97a.75.75 0 1 1-1.06 1.06l-5.16-5.159a1.5 1.5 0 0 0-2.12 0L3 16.061Zm10.125-7.81a1.125 1.125 0 1 1 2.25 0 1.125 1.125 0 0 1-2.25 0Z"
                      clipRule="evenodd"
                    />
                  </svg>
                  <p className="mt-2 text-sm text-gray-600">
                    Clique para enviar sua logo
                  </p>
                  <p className="mt-1 text-xs text-gray-400">PNG, JPG até 2MB</p>
                </div>
              </button>
            )}
          </div>
        </div>

        {/* Color presets */}
        <div>
          <label className="block text-sm/6 font-medium text-gray-900">Cores do cartão</label>
          <div className="mt-3 grid grid-cols-4 gap-3">
            {COLOR_PRESETS.map((preset) => {
              const isSelected = preset.primaria === corPrimaria
              return (
                <button
                  key={preset.nome}
                  type="button"
                  onClick={() => onCoresChange(preset.primaria, preset.secundaria)}
                  className={`flex cursor-pointer flex-col items-center gap-1.5 rounded-lg p-2 ${
                    isSelected ? 'ring-2 ring-primary ring-offset-2' : 'hover:bg-gray-50'
                  }`}
                >
                  <div className="flex gap-0.5">
                    <div
                      className="size-6 rounded-l-full"
                      style={{ backgroundColor: preset.primaria }}
                    />
                    <div
                      className="size-6 rounded-r-full"
                      style={{ backgroundColor: preset.secundaria }}
                    />
                  </div>
                  <span className="text-xs text-gray-600">{preset.nome}</span>
                </button>
              )
            })}
          </div>
        </div>

        {/* Custom color */}
        <details className="group">
          <summary className="cursor-pointer text-sm font-medium text-gray-500 hover:text-gray-700">
            Cor personalizada
          </summary>
          <div className="mt-3 flex gap-4">
            <div>
              <label className="block text-xs text-gray-500">Primária</label>
              <input
                type="color"
                value={corPrimaria}
                onChange={(e) => onCoresChange(e.target.value, corSecundaria)}
                className="mt-1 h-8 w-14 cursor-pointer rounded border border-gray-300"
              />
            </div>
            <div>
              <label className="block text-xs text-gray-500">Secundária</label>
              <input
                type="color"
                value={corSecundaria}
                onChange={(e) => onCoresChange(corPrimaria, e.target.value)}
                className="mt-1 h-8 w-14 cursor-pointer rounded border border-gray-300"
              />
            </div>
          </div>
        </details>
      </div>
    </div>
  )
}
