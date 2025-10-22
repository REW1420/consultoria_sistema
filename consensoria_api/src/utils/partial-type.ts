import "reflect-metadata";

export function PartialType<T extends new (...args: any[]) => any>(
  classRef: T
): T {
  abstract class PartialClass {}

  // Copia los metadatos de class-validator
  const keys = Reflect.getMetadataKeys(classRef.prototype);
  for (const key of keys) {
    const meta = Reflect.getMetadata(key, classRef.prototype);
    Reflect.defineMetadata(key, meta, PartialClass.prototype);
  }

  // Copia las propiedades
  Object.getOwnPropertyNames(classRef.prototype).forEach((prop) => {
    if (prop === "constructor") return;
    Object.defineProperty(
      PartialClass.prototype,
      prop,
      Object.getOwnPropertyDescriptor(classRef.prototype, prop)!
    );
  });

  return PartialClass as any;
}
