import { Router } from "express";
import { CarController } from "./car.controller";
import { CreateCarDto, UpdateCarDto } from "./car.dto";
import { validationMiddleware } from "../../middleware/validation.middleware";
import { authenticate, authorizeRole } from "../../middleware/auth.middleware";

const controller = new CarController();
export const carRoutes = Router();

// Listar y ver — ambos roles
carRoutes.get("/", controller.getAll);
carRoutes.get("/:id", controller.getOne);

// Crear, actualizar y eliminar — ambos roles
carRoutes.post(
  "/",

  validationMiddleware(CreateCarDto),
  controller.create
);
carRoutes.put(
  "/:id",

  validationMiddleware(UpdateCarDto),
  controller.update
);
carRoutes.delete(
  "/:id",
  authenticate,
  authorizeRole("Admin", "Sales"),
  controller.delete
);

// Acciones adicionales — ambos roles
carRoutes.put(
  "/:id/sold",
  authenticate,
  authorizeRole("Admin", "Sales"),
  controller.markAsSold.bind(controller)
);
carRoutes.put(
  "/:id/publish",
  authenticate,
  authorizeRole("Admin", "Sales"),
  controller.togglePublish.bind(controller)
);
