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
      <ol role="list" className="flex items-center">
        {STEPS.map((step, idx) => {
          const stepNum = idx + 1
          const isComplete = stepNum < currentStep
          const isCurrent = stepNum === currentStep
          const isLast = idx === STEPS.length - 1

          return (
            <li
              key={step.name}
              className={`flex flex-col items-center ${isLast ? 'relative' : 'relative flex-1'}`}
            >
              {/* Linha + círculo */}
              <div className="flex w-full items-center">
                {/* Círculo */}
                {isComplete ? (
                  <div className="relative flex size-8 shrink-0 items-center justify-center rounded-full bg-primary">
                    <CheckIcon aria-hidden="true" className="size-5 text-white" />
                    <span className="sr-only">{step.name}</span>
                  </div>
                ) : isCurrent ? (
                  <div
                    aria-current="step"
                    className="relative flex size-8 shrink-0 items-center justify-center rounded-full border-2 border-primary bg-white"
                  >
                    <span aria-hidden="true" className="size-2.5 rounded-full bg-primary" />
                    <span className="sr-only">{step.name}</span>
                  </div>
                ) : (
                  <div className="relative flex size-8 shrink-0 items-center justify-center rounded-full border-2 border-gray-300 bg-white">
                    <span aria-hidden="true" className="size-2.5 rounded-full bg-transparent" />
                    <span className="sr-only">{step.name}</span>
                  </div>
                )}

                {/* Linha conectora */}
                {!isLast && (
                  <div className={`h-0.5 w-full ${isComplete ? 'bg-primary' : 'bg-gray-200'}`} />
                )}
              </div>

              {/* Label centralizado sob o círculo */}
              <span
                className={`mt-2 hidden text-xs font-medium sm:block ${
                  stepNum <= currentStep ? 'text-primary' : 'text-gray-500'
                }`}
              >
                {step.name}
              </span>
            </li>
          )
        })}
      </ol>
    </nav>
  )
}
