import { PrismaClient } from "@prisma/client";
import { CreateMessageDto } from "./message.dto";

const prisma = new PrismaClient();

export class MessageService {
  async findAll() {
    const messages = await prisma.messages.findMany({
      orderBy: { received_at: "desc" },
      include: { cars: true },
    });

    // Convertir price de los autos a number (si existe)
    return messages.map((msg) => ({
      ...msg,
      cars: msg.cars
        ? {
            ...msg.cars,
            price: msg.cars.price ? Number(msg.cars.price) : null,
          }
        : null,
    }));
  }

  async findById(id: number) {
    const message = await prisma.messages.findUnique({
      where: { id },
      include: { cars: true },
    });

    if (!message) return null;

    return {
      ...message,
      cars: message.cars
        ? {
            ...message.cars,
            price: message.cars.price ? Number(message.cars.price) : null,
          }
        : null,
    };
  }

  async create(data: CreateMessageDto) {
    return prisma.messages.create({ data });
  }

  async markAsRead(id: number) {
    return prisma.messages.update({
      where: { id },
      data: { is_read: true },
    });
  }

  async delete(id: number) {
    return prisma.messages.delete({ where: { id } });
  }
}
