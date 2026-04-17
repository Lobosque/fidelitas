import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../../contexts/AuthContext'
import { api } from '../../lib/api'
import type { Campaign } from '../../types'

function formatDate(iso: string): string {
  return new Date(iso).toLocaleDateString('pt-BR', {
    day: '2-digit',
    month: 'short',
    year: 'numeric',
  })
}

export function DashboardPage() {
  const { user } = useAuth()
  const navigate = useNavigate()

  const [campaigns, setCampaigns] = useState<Campaign[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    api<Campaign[]>('/campaigns')
      .then(setCampaigns)
      .catch(() => {})
      .finally(() => setLoading(false))
  }, [])

  if (loading) {
    return (
      <div className="flex justify-center py-16">
        <p className="text-sm text-gray-500">Carregando...</p>
      </div>
    )
  }

  const activeCampaigns = campaigns.filter((c) => c.ativa)

  const stats = [
    { label: 'Campanhas ativas', value: activeCampaigns.length },
    { label: 'Total de campanhas', value: campaigns.length },
  ]

  if (campaigns.length === 0) {
    return (
      <div className="py-16 text-center">
        <svg
          className="mx-auto size-12 text-gray-400"
          fill="none"
          viewBox="0 0 24 24"
          strokeWidth={1.5}
          stroke="currentColor"
        >
          <path
            strokeLinecap="round"
            strokeLinejoin="round"
            d="M12 9v6m3-3H9m12 0a9 9 0 1 1-18 0 9 9 0 0 1 18 0Z"
          />
        </svg>
        <h3 className="mt-4 text-lg font-semibold text-gray-900">Nenhuma campanha ainda</h3>
        <p className="mt-2 text-sm text-gray-500">
          Crie sua primeira campanha de fidelidade para começar.
        </p>
        <button
          type="button"
          onClick={() => navigate('/onboarding')}
          className="mt-6 cursor-pointer rounded-md bg-primary px-4 py-2 text-sm font-semibold text-white shadow-xs hover:bg-secondary focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-primary"
        >
          Criar primeira campanha
        </button>
      </div>
    )
  }

  return (
    <div className="space-y-8">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Suas campanhas</h1>
          <p className="mt-1 text-sm text-gray-500">
            Olá, {user?.nome}. Aqui estão suas campanhas de fidelidade.
          </p>
        </div>
        <button
          type="button"
          onClick={() => navigate('/onboarding')}
          className="cursor-pointer rounded-md bg-primary px-4 py-2 text-sm font-semibold text-white shadow-xs hover:bg-secondary focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-primary"
        >
          Nova campanha
        </button>
      </div>

      {/* Stats */}
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        {stats.map((stat) => (
          <div
            key={stat.label}
            className="rounded-lg bg-white p-6 shadow-sm ring-1 ring-gray-900/5"
          >
            <p className="text-sm font-medium text-gray-500">{stat.label}</p>
            <p className="mt-2 text-3xl font-bold text-gray-900">{stat.value}</p>
          </div>
        ))}
      </div>

      {/* Campaign list */}
      <ul role="list" className="divide-y divide-gray-100">
        {campaigns.map((campaign) => (
          <li key={campaign.id} className="flex items-center justify-between gap-x-6 py-5">
            <div className="min-w-0">
              <div className="flex items-start gap-x-3">
                <p className="text-sm/6 font-semibold text-gray-900">{campaign.nome}</p>
                {campaign.ativa ? (
                  <p className="mt-0.5 rounded-md bg-green-50 px-1.5 py-0.5 text-xs font-medium text-green-700 inset-ring inset-ring-green-600/20">
                    Ativa
                  </p>
                ) : (
                  <p className="mt-0.5 rounded-md bg-gray-50 px-1.5 py-0.5 text-xs font-medium text-gray-600 inset-ring inset-ring-gray-500/10">
                    Inativa
                  </p>
                )}
              </div>
              <div className="mt-1 flex items-center gap-x-2 text-xs/5 text-gray-500">
                <p>{campaign.checkinsNecessarios} check-ins → {campaign.descricao}</p>
                <svg viewBox="0 0 2 2" className="size-0.5 fill-current">
                  <circle r={1} cx={1} cy={1} />
                </svg>
                <p>Criada em {formatDate(campaign.criadaEm)}</p>
              </div>
            </div>
            <div className="flex flex-none items-center gap-x-4">
              <button
                type="button"
                className="hidden cursor-pointer rounded-md bg-white px-2.5 py-1.5 text-sm font-semibold text-gray-900 shadow-xs inset-ring inset-ring-gray-300 hover:bg-gray-50 sm:block"
              >
                Ver totem
              </button>
            </div>
          </li>
        ))}
      </ul>
    </div>
  )
}
