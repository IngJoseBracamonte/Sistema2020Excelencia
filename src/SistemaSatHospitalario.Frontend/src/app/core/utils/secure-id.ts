export function generateSecureUuid(): string {
  if (typeof globalThis.crypto === 'undefined') {
    throw new Error('Web Crypto API no está disponible para generar un identificador seguro.');
  }

  if (typeof globalThis.crypto.randomUUID === 'function') {
    return globalThis.crypto.randomUUID();
  }

  const bytes = globalThis.crypto.getRandomValues(new Uint8Array(16));
  bytes[6] = (bytes[6] & 0x0f) | 0x40;
  bytes[8] = (bytes[8] & 0x3f) | 0x80;
  const hexadecimal = Array.from(bytes, value => value.toString(16).padStart(2, '0'));

  return `${hexadecimal.slice(0, 4).join('')}-${hexadecimal.slice(4, 6).join('')}-${hexadecimal.slice(6, 8).join('')}-${hexadecimal.slice(8, 10).join('')}-${hexadecimal.slice(10).join('')}`;
}
