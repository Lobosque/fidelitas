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

          return (
            <li
              key={step.name}
              className={idx !== STEPS.length - 1 ? 'relative pr-8 sm:pr-20' : 'relative'}
            >
              {isComplete ? (
                <>
                  <div aria-hidden="true" className="absolute inset-0 flex items-center">
                    <div className="h-0.5 w-full bg-primary" />
                  </div>
                  <div className="relative flex size-8 items-center justify-center rounded-full bg-primary">
                    <CheckIcon aria-hidden="true" className="size-5 text-white" />
                    <span className="sr-only">{step.name}</span>
                  </div>
                </>
              ) : isCurrent ? (
                <>
                  <div aria-hidden="true" className="absolute inset-0 flex items-center">
                    <div className="h-0.5 w-full bg-gray-200" />
                  </div>
                  <div
                    aria-current="step"
                    className="relative flex size-8 items-center justify-center rounded-full border-2 border-primary bg-white"
                  >
                    <span aria-hidden="true" className="size-2.5 rounded-full bg-primary" />
                    <span className="sr-only">{step.name}</span>
                  </div>
                </>
              ) : (
                <>
                  <div aria-hidden="true" className="absolute inset-0 flex items-center">
                    <div className="h-0.5 w-full bg-gray-200" />
                  </div>
                  <div className="relative flex size-8 items-center justify-center rounded-full border-2 border-gray-300 bg-white">
                    <span aria-hidden="true" className="size-2.5 rounded-full bg-transparent" />
                    <span className="sr-only">{step.name}</span>
                  </div>
                </>
              )}
            </li>
          )
        })}
      </ol>
      <div className="mt-2 hidden sm:flex">
        {STEPS.map((step, idx) => (
          <span
            key={step.name}
            className={`text-xs font-medium ${
              idx !== STEPS.length - 1 ? 'pr-8 sm:pr-20' : ''
            } ${idx + 1 <= currentStep ? 'text-primary' : 'text-gray-500'}`}
          >
            {step.name}
          </span>
        ))}
      </div>
    </nav>
  )
}
