import { WalletCardPreview } from '../../components/onboarding/WalletCardPreview'
import { TotemPreview } from '../../components/onboarding/TotemPreview'

interface OnboardingState {
  logoPreviewUrl: string | null
  corPrimaria: string
  corSecundaria: string
  nomeCampanha: string
  checkinsNecessarios: number
  descricaoPremio: string
}

interface StepPreviewConfirmacaoProps {
  state: OnboardingState
  nomeNegocio: string
  isSubmitting: boolean
  onBack: () => void
  onSubmit: () => void
}

export function StepPreviewConfirmacao({
  state,
  nomeNegocio,
  isSubmitting,
  onBack,
  onSubmit,
}: StepPreviewConfirmacaoProps) {
  return (
    <div className="space-y-8">
      <div>
        <h2 className="text-lg font-semibold text-gray-900">Preview e Confirmação</h2>
        <p className="mt-1 text-sm text-gray-500">
          Confira como vai ficar seu cartão e totem antes de criar a campanha.
        </p>
      </div>

      <div className="grid gap-8 lg:grid-cols-2">
        {/* Wallet card preview */}
        <div>
          <h3 className="mb-3 text-sm font-medium text-gray-700">Cartão na wallet do cliente</h3>
          <WalletCardPreview
            logoUrl={state.logoPreviewUrl}
            corPrimaria={state.corPrimaria}
            corSecundaria={state.corSecundaria}
            nomeNegocio={nomeNegocio}
            nomeCampanha={state.nomeCampanha}
            checkinsNecessarios={state.checkinsNecessarios}
            descricaoPremio={state.descricaoPremio}
          />
        </div>

        {/* Totem preview */}
        <div>
          <h3 className="mb-3 text-sm font-medium text-gray-700">Totem para imprimir</h3>
          <TotemPreview
            campaignId="preview"
            nomeNegocio={nomeNegocio}
            nomeCampanha={state.nomeCampanha}
            descricaoPremio={state.descricaoPremio}
            checkinsNecessarios={state.checkinsNecessarios}
            corPrimaria={state.corPrimaria}
            logoUrl={state.logoPreviewUrl}
          />
        </div>
      </div>

      {/* Actions */}
      <div className="flex items-center justify-between pt-4">
        <button
          type="button"
          onClick={onBack}
          disabled={isSubmitting}
          className="cursor-pointer text-sm font-semibold text-gray-600 hover:text-gray-900 disabled:opacity-50"
        >
          Voltar
        </button>
        <button
          type="button"
          onClick={onSubmit}
          disabled={isSubmitting}
          className="flex cursor-pointer justify-center rounded-md bg-primary px-6 py-2 text-sm/6 font-semibold text-white shadow-xs hover:bg-secondary focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-primary disabled:opacity-50"
        >
          {isSubmitting ? 'Criando...' : 'Criar campanha'}
        </button>
      </div>
    </div>
  )
}
