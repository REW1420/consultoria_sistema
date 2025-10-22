import { Request, Response } from "express";
import { CarService } from "./car.service";
import { UpdateCarDto } from "./car.dto";
import { validationMiddleware } from "../../middleware/validation.middleware";
import { prisma } from "../../config/prisma";

const service = new CarService();

export class CarController {
  async getAll(req: Request, res: Response) {
    const cars = await service.getAll();
    res.json(cars);
  }

  async getOne(req: Request, res: Response) {
    const id = Number(req.params.id);
    const car = await service.getById(id);
    car ? res.json(car) : res.status(404).json({ message: "Car not found" });
  }

  async create(req: Request, res: Response) {
    try {
      const data = req.body;

      // 🔹 Si hay varias imágenes
      const images = Array.isArray(data.images) ? data.images : [];

      const newCar = await prisma.cars.create({
        data: {
          model: data.model,
          description: data.description,
          year: data.year,
          price: Number(data.price),
          image_url: images[0] || null, // usa la primera como principal
          transmission_id: data.transmission_id,
          condition_id: data.condition_id,
          created_by: data.created_by,
          car_images: {
            create: images.map((url: string) => ({ url })),
          },
        },
        include: { car_images: true },
      });

      res.json(newCar);
    } catch (error) {
      console.error(error);
      res.status(500).json({ message: "Error al crear el vehículo" });
    }
  }

  async update(req: Request, res: Response) {
    try {
      const id = Number(req.params.id);
      const existing = await service.getById(id);

      if (!existing) {
        return res.status(404).json({ message: `Car ID ${id} not found` });
      }

      const dto = Object.assign(new UpdateCarDto(), req.body);

      const updatedCar = await service.update(id, dto);
      res.json(updatedCar);
    } catch (error) {
      console.error("❌ Error updating car:", error);
      res.status(400).json({
        message: "Error updating car",
        error: error instanceof Error ? error.message : error,
      });
    }
  }

  async delete(req: Request, res: Response) {
    const id = Number(req.params.id);
    await service.delete(id);
    res.json({ message: "Car deleted" });
  }

  async markAsSold(req: Request, res: Response) {
    try {
      const id = parseInt(req.params.id);
      const updated = await service.update(id, {
        is_sold: true,
        sold_at: new Date(),
      });
      res.json(updated);
    } catch (error) {
      console.error("Error marking car as sold:", error);
      res.status(500).json({ message: "Error marking car as sold", error });
    }
  }

  async togglePublish(req: Request, res: Response) {
    try {
      const id = parseInt(req.params.id);
      const { publish } = req.body; // true para publicar, false para pausar
      const updated = await service.update(id, { is_published: publish });
      res.json(updated);
    } catch (error) {
      console.error("Error toggling publish state:", error);
      res.status(500).json({ message: "Error updating publish state", error });
    }
  }
}
