import { PrismaClient } from "@prisma/client";
import bcrypt from "bcryptjs";
import { CreateUserDto } from "./user.dto";

const prisma = new PrismaClient();

export class UserService {
  async findAll() {
    return prisma.users.findMany({
      include: { user_roles: true },
    });
  }

  async findById(id: number) {
    return prisma.users.findUnique({
      where: { id },
      include: { user_roles: true },
    });
  }
  async create(data: any) {
    const password_hash = await bcrypt.hash(data.password, 10);

    console.log("📦 Datos recibidos del frontend:", data);

    const payload = {
      username: data.username,
      full_name: data.full_name,
      email: data.email,
      password_hash,
      role_id: data.user_roles.id,
      active: data.active ?? true,
    };

    console.log("🚀 Payload que se enviará a Prisma:", payload);

    return prisma.users.create({
      data: payload,
    });
  }

  async update(id: number, data: any) {
    console.log("🧩 Datos recibidos para actualización:", data);

    // Base del update
    const updateData: any = {
      username: data.username,
      full_name: data.full_name,
      email: data.email,
      active: data.active ?? true,
    };

    // Si viene el role desde el objeto user_roles
    if (data.user_roles && data.user_roles.id) {
      updateData.role_id = data.user_roles.id;
    } else if (data.role_id) {
      updateData.role_id = data.role_id;
    }

    // Si viene password, la ciframos
    if (data.password) {
      updateData.password_hash = await bcrypt.hash(data.password, 10);
    }

    console.log("🚀 Payload que se enviará a Prisma (update):", updateData);

    // Ejecución del update en la base de datos
    return prisma.users.update({
      where: { id },
      data: updateData,
    });
  }

  async delete(id: number) {
    return prisma.users.delete({ where: { id } });
  }

  async findByUsername(username: string) {
    return prisma.users.findUnique({ where: { username } });
  }
}
