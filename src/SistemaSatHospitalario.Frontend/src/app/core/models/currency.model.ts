export interface CurrencyGroup {
  id: number;
  code: string;
  name: string;
  symbol: string;
  isBaseUsd: boolean;
}

export const CurrencyIds = {
  USD: 1,
  VES: 2, // Bolívares
  EUR: 3, // Euros (A futuro)
  COP: 4, // Peso Colombiano (A futuro)
  ARS: 5  // Peso Argentino (A futuro)
};

export const CURRENCY_GROUPS: Record<number, CurrencyGroup> = {
  [CurrencyIds.USD]: { id: CurrencyIds.USD, code: 'USD', name: 'Dólar', symbol: '$', isBaseUsd: true },
  [CurrencyIds.VES]: { id: CurrencyIds.VES, code: 'VES', name: 'Bolívar', symbol: 'Bs.', isBaseUsd: false },
  [CurrencyIds.EUR]: { id: CurrencyIds.EUR, code: 'EUR', name: 'Euro', symbol: '€', isBaseUsd: false },
  [CurrencyIds.COP]: { id: CurrencyIds.COP, code: 'COP', name: 'Peso Colombiano', symbol: 'COP$', isBaseUsd: false },
  [CurrencyIds.ARS]: { id: CurrencyIds.ARS, code: 'ARS', name: 'Peso Argentino', symbol: 'ARS$', isBaseUsd: false }
};
