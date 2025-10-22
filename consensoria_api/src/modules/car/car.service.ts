import { prisma } from "../../config/prisma";
import { Prisma } from "@prisma/client";

export class CarService {
  async getAll() {
    const cars = await prisma.cars.findMany({
      include: {
        transmissions: true,
        car_conditions: true,
        users: {
          select: { id: true, full_name: true, email: true },
        },
      },
      orderBy: { created_at: "desc" },
    });

    return cars.map((car) => ({
      ...car,
      price: car.price ? Number(car.price) : null,
    }));
  }

  async getById(id: number) {
    const car = await prisma.cars.findUnique({
      where: { id },
      include: {
        car_images: { select: { url: true } },
        transmissions: true,
        car_conditions: true,
        users: true,
        messages: true,
      },
    });

    if (!car) return null;

    // ✅ Convertir los Decimals a number (ej. price)
    return {
      ...car,
      price: car.price ? Number(car.price) : null,
    };
  }

  async create(data: Prisma.carsCreateInput) {
    return prisma.cars.create({ data });
  }

  async update(id: number, data: Prisma.carsUpdateInput) {
    return prisma.cars.update({
      where: { id },
      data,
    });
  }

  async delete(id: number) {
    return prisma.cars.delete({ where: { id } });
  }
}
