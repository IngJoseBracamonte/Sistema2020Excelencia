import { Injectable, isDevMode } from '@angular/core';
import { WebTracerProvider } from '@opentelemetry/sdk-trace-web';
import { BatchSpanProcessor } from '@opentelemetry/sdk-trace-base';
import { OTLPTraceExporter } from '@opentelemetry/exporter-trace-otlp-http';
import { resourceFromAttributes } from '@opentelemetry/resources';
import { SemanticResourceAttributes } from '@opentelemetry/semantic-conventions';
import { registerInstrumentations } from '@opentelemetry/instrumentation';
import { XMLHttpRequestInstrumentation } from '@opentelemetry/instrumentation-xml-http-request';
import { FetchInstrumentation } from '@opentelemetry/instrumentation-fetch';

@Injectable({
  providedIn: 'root'
})
export class TelemetryService {
  private provider: WebTracerProvider | null = null;

  constructor() {
    this.initTelemetry();
  }

  private initTelemetry() {
    if (!isDevMode()) return;

    try {
      const otlpEndpoint = (window as any)?.__ENV?.OTEL_EXPORTER_OTLP_ENDPOINT 
        ? `${(window as any).__ENV.OTEL_EXPORTER_OTLP_ENDPOINT}/v1/traces`
        : 'http://localhost:18889/v1/traces';

      const resource = resourceFromAttributes({
        [SemanticResourceAttributes.SERVICE_NAME]: 'sistema-sat-hospitalario.frontend',
      });

      this.provider = new WebTracerProvider({ resource });

      const exporter = new OTLPTraceExporter({
        url: otlpEndpoint,
      });

      // Fix: Uso de any para evitar inconsistencias de tipos en OTel 2.x dentro de Angular 19
      if (this.provider) {
        (this.provider as any).addSpanProcessor(new BatchSpanProcessor(exporter as any));
        this.provider.register();

        registerInstrumentations({
          instrumentations: [
            new XMLHttpRequestInstrumentation(),
            new FetchInstrumentation(),
          ],
          tracerProvider: this.provider,
        });
      }

      console.log('OpenTelemetry initialized for Frontend');
    } catch (e) {
      console.debug('OpenTelemetry disabled or unreachable in this environment');
    }
  }
}
