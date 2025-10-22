import { Router } from "express";
import { prisma } from "../../../config/prisma";
import {
  authenticate,
  authorizeRole,
} from "../../../middleware/auth.middleware";

const transmissionsRoutes = Router();

// ✅ Solo roles: Admin y Sales
transmissionsRoutes.get(
  "/",
  authenticate,
  authorizeRole("Admin", "Sales"),
  async (req, res) => {
    try {
      const transmissions = await prisma.transmissions.findMany({
        orderBy: { type: "asc" },
      });
      res.json(transmissions);
    } catch (err) {
      console.error("❌ Error al obtener transmisiones:", err);
      res.status(500).json({ error: "Error al obtener transmisiones" });
    }
  }
);

export default transmissionsRoutes;
