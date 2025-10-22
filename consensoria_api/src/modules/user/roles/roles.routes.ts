import { Router } from "express";
import { prisma } from "../../../config/prisma";
import {
  authenticate,
  authorizeRole,
} from "../../../middleware/auth.middleware";

const rolesRoutes = Router();

// Obtener todas las condiciones de autos
rolesRoutes.get("/", authenticate, authorizeRole("Admin"), async (req, res) => {
  try {
    const roles = await prisma.user_roles.findMany();
    res.json(roles);
  } catch (err) {
    res.status(500).json({ error: "Error al obtener roles" });
  }
});

export default rolesRoutes;
