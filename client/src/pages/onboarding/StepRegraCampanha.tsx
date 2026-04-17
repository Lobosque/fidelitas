const CHECKIN_PRESETS = [5, 8, 10, 12, 15]

interface StepRegraCampanhaProps {
  nomeCampanha: string
  checkinsNecessarios: number
  descricaoPremio: string
  onNomeCampanhaChange: (value: string) => void
  onCheckinsNecessariosChange: (value: number) => void
  onDescricaoPremioChange: (value: string) => void
}

export function StepRegraCampanha({
  nomeCampanha,
  checkinsNecessarios,
  descricaoPremio,
  onNomeCampanhaChange,
  onCheckinsNecessariosChange,
  onDescricaoPremioChange,
}: StepRegraCampanhaProps) {
  const isCustomCheckins = !CHECKIN_PRESETS.includes(checkinsNecessarios)

  return (
    <div className="mx-auto max-w-lg space-y-8">
      <div>
        <h2 className="text-lg font-semibold text-gray-900">Regra da Campanha</h2>
        <p className="mt-1 text-sm text-gray-500">
          Defina o nome, quantos check-ins e qual o prêmio.
        </p>
      </div>

      {/* Campaign name */}
      <div>
        <label htmlFor="nome-campanha" className="block text-sm/6 font-medium text-gray-900">
          Nome da campanha
        </label>
        <div className="mt-2">
          <input
            id="nome-campanha"
            type="text"
            value={nomeCampanha}
            onChange={(e) => onNomeCampanhaChange(e.target.value)}
            placeholder="Ex: Corte Fidelidade"
            className="block w-full rounded-md bg-white px-3 py-1.5 text-base text-gray-900 outline-1 -outline-offset-1 outline-gray-300 placeholder:text-gray-400 focus:outline-2 focus:-outline-offset-2 focus:outline-primary sm:text-sm/6"
          />
        </div>
      </div>

      {/* Checkins needed */}
      <div>
        <label className="block text-sm/6 font-medium text-gray-900">
          Check-ins para o prêmio
        </label>
        <div className="mt-3 flex flex-wrap gap-2">
          {CHECKIN_PRESETS.map((n) => (
            <button
              key={n}
              type="button"
              onClick={() => onCheckinsNecessariosChange(n)}
              className={`cursor-pointer rounded-full px-4 py-1.5 text-sm font-medium ${
                checkinsNecessarios === n
                  ? 'bg-primary text-white'
                  : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              }`}
            >
              {n}
            </button>
          ))}
          <button
            type="button"
            onClick={() => {
              if (!isCustomCheckins) onCheckinsNecessariosChange(7)
            }}
            className={`cursor-pointer rounded-full px-4 py-1.5 text-sm font-medium ${
              isCustomCheckins
                ? 'bg-primary text-white'
                : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
            }`}
          >
            Outro
          </button>
        </div>
        {isCustomCheckins && (
          <div className="mt-3">
            <input
              type="number"
              min={2}
              max={50}
              value={checkinsNecessarios}
              onChange={(e) => onCheckinsNecessariosChange(Number(e.target.value))}
              className="block w-24 rounded-md bg-white px-3 py-1.5 text-base text-gray-900 outline-1 -outline-offset-1 outline-gray-300 focus:outline-2 focus:-outline-offset-2 focus:outline-primary sm:text-sm/6"
            />
          </div>
        )}
      </div>

      {/* Reward description */}
      <div>
        <label htmlFor="descricao-premio" className="block text-sm/6 font-medium text-gray-900">
          Descrição do prêmio
        </label>
        <div className="mt-2">
          <input
            id="descricao-premio"
            type="text"
            value={descricaoPremio}
            onChange={(e) => onDescricaoPremioChange(e.target.value)}
            placeholder="Ex: 1 corte grátis"
            className="block w-full rounded-md bg-white px-3 py-1.5 text-base text-gray-900 outline-1 -outline-offset-1 outline-gray-300 placeholder:text-gray-400 focus:outline-2 focus:-outline-offset-2 focus:outline-primary sm:text-sm/6"
          />
        </div>
      </div>
    </div>
  )
}
