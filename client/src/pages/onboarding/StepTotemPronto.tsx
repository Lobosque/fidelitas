import { useNavigate } from 'react-router-dom'
import { TotemPreview } from '../../components/onboarding/TotemPreview'
import type { Campaign } from '../../types'

interface OnboardingState {
  logoPreviewUrl: string | null
  corPrimaria: string
  corSecundaria: string
  nomeCampanha: string
  checkinsNecessarios: number
  descricaoPremio: string
}

interface StepTotemProntoProps {
  campaign: Campaign
  state: OnboardingState
  nomeNegocio: string
}

export function StepTotemPronto({ campaign, state, nomeNegocio }: StepTotemProntoProps) {
  const navigate = useNavigate()

  return (
    <div className="space-y-8 text-center">
      <div>
        <div className="mx-auto flex size-12 items-center justify-center rounded-full bg-green-100">
          <svg className="size-6 text-green-600" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" d="M4.5 12.75l6 6 9-13.5" />
          </svg>
        </div>
        <h2 className="mt-4 text-xl font-semibold text-gray-900">Campanha criada!</h2>
        <p className="mt-2 text-sm text-gray-500">
          Seu totem está pronto. Imprima e coloque no balcão do seu negócio.
        </p>
      </div>

      <TotemPreview
        campaignId={campaign.id}
        nomeNegocio={nomeNegocio}
        nomeCampanha={state.nomeCampanha}
        descricaoPremio={state.descricaoPremio}
        checkinsNecessarios={state.checkinsNecessarios}
        corPrimaria={state.corPrimaria}
        logoUrl={state.logoPreviewUrl}
        printable
      />

      <div className="flex flex-col items-center gap-3 sm:flex-row sm:justify-center">
        <button
          type="button"
          onClick={() => window.print()}
          className="flex cursor-pointer justify-center rounded-md bg-white px-6 py-2 text-sm/6 font-semibold text-gray-900 shadow-xs ring-1 ring-inset ring-gray-300 hover:bg-gray-50"
        >
          Imprimir totem
        </button>
        <button
          type="button"
          onClick={() => navigate('/dashboard')}
          className="flex cursor-pointer justify-center rounded-md bg-primary px-6 py-2 text-sm/6 font-semibold text-white shadow-xs hover:bg-secondary focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-primary"
        >
          Ir para o painel
        </button>
      </div>
    </div>
  )
}
