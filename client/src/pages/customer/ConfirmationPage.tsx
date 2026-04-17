import { useParams, Link } from 'react-router-dom'

export function ConfirmationPage() {
  const { campaignId } = useParams()

  return (
    <div className="flex min-h-screen items-center justify-center bg-gray-50 px-4">
      <div className="w-full max-w-sm text-center">
        <div className="mx-auto flex size-16 items-center justify-center rounded-full bg-green-100">
          <svg className="size-8 text-green-600" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" d="M4.5 12.75l6 6 9-13.5" />
          </svg>
        </div>
        <h1 className="mt-4 text-2xl font-bold text-gray-900">Pronto!</h1>
        <p className="mt-2 text-sm text-gray-500">
          Seu cartão de fidelidade foi adicionado à sua carteira digital.
        </p>
        <p className="mt-1 text-sm text-gray-500">
          Apresente-o a cada visita para registrar seus check-ins.
        </p>
        <Link
          to={`/c/${campaignId}`}
          className="mt-6 inline-block text-sm font-semibold text-indigo-600 hover:text-indigo-500"
        >
          Voltar para a campanha
        </Link>
      </div>
    </div>
  )
}
