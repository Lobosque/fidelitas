import { useParams } from 'react-router-dom'

export function LandingPage() {
  const { campaignId } = useParams()

  return (
    <div className="flex min-h-screen items-center justify-center bg-gray-50 px-4">
      <div className="w-full max-w-sm text-center">
        <h1 className="text-2xl font-bold text-gray-900">Cartão Fidelidade</h1>
        <p className="mt-2 text-sm text-gray-500">
          Campanha: {campaignId}
        </p>
        <p className="mt-1 text-sm text-gray-500">
          Placeholder — landing page do cliente com botão de adicionar à wallet
        </p>
      </div>
    </div>
  )
}
