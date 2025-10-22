import { Router } from "express";
import { prisma } from "../../../config/prisma";
import {
  authenticate,
  authorizeRole,
} from "../../../middleware/auth.middleware";

const conditionsRoutes = Router();

// Obtener todas las condiciones de autos
conditionsRoutes.get(
  "/",
  authenticate,
  authorizeRole("Admin", "Sales"),
  async (req, res) => {
    try {
      const conditions = await prisma.car_conditions.findMany({
        orderBy: { label: "asc" },
      });
      res.json(conditions);
    } catch (err) {
      res.status(500).json({ error: "Error al obtener condiciones" });
    }
  }
);

export default conditionsRoutes;
