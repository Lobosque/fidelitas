import { CheckIcon } from '@heroicons/react/20/solid'

const STEPS = [
  { name: 'Identidade' },
  { name: 'Campanha' },
  { name: 'Preview' },
  { name: 'Totem' },
]

interface StepIndicatorProps {
  currentStep: number
}

export function StepIndicator({ currentStep }: StepIndicatorProps) {
  return (
    <nav aria-label="Progresso" className="mt-8">
      <ol role="list" className="flex items-start">
        {STEPS.map((step, idx) => {
          const stepNum = idx + 1
          const isComplete = stepNum < currentStep
          const isCurrent = stepNum === currentStep
          const isLast = idx === STEPS.length - 1

          return (
            <li
              key={step.name}
              className={`relative flex items-start ${isLast ? '' : 'flex-1'}`}
            >
              {/* Coluna do círculo + label */}
              <div className="flex flex-col items-center">
                {isComplete ? (
                  <div className="flex size-8 items-center justify-center rounded-full bg-primary">
                    <CheckIcon aria-hidden="true" className="size-5 text-white" />
                    <span className="sr-only">{step.name}</span>
                  </div>
                ) : isCurrent ? (
                  <div
                    aria-current="step"
                    className="flex size-8 items-center justify-center rounded-full border-2 border-primary bg-white"
                  >
                    <span aria-hidden="true" className="size-2.5 rounded-full bg-primary" />
                    <span className="sr-only">{step.name}</span>
                  </div>
                ) : (
                  <div className="flex size-8 items-center justify-center rounded-full border-2 border-gray-300 bg-white">
                    <span aria-hidden="true" className="size-2.5 rounded-full bg-transparent" />
                    <span className="sr-only">{step.name}</span>
                  </div>
                )}

                <span
                  className={`mt-2 hidden text-xs font-medium sm:block ${
                    stepNum <= currentStep ? 'text-primary' : 'text-gray-500'
                  }`}
                >
                  {step.name}
                </span>
              </div>

              {/* Linha conectora — posicionada ao lado do círculo */}
              {!isLast && (
                <div className="mt-4 h-0.5 flex-1 self-start" style={{ backgroundColor: isComplete ? undefined : '#e5e7eb' }}>
                  <div className={`h-full ${isComplete ? 'bg-primary' : ''}`} />
                </div>
              )}
            </li>
          )
        })}
      </ol>
    </nav>
  )
}
