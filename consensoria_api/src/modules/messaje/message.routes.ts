import { Router } from "express";
import { MessageController } from "./message.controller";
import { authenticate, authorizeRole } from "../../middleware/auth.middleware";

const MessageRoutes = Router();
const ctrl = new MessageController();

// ✅ Solo roles "Admin" o "Sales" pueden ver o gestionar mensajes
MessageRoutes.get(
  "/",

  ctrl.getAll
);
MessageRoutes.get(
  "/:id",

  ctrl.getOne
);
MessageRoutes.put(
  "/:id/read",
  authenticate,
  authorizeRole("Admin", "Sales"),
  ctrl.markAsRead
);
MessageRoutes.delete(
  "/:id",
  authenticate,
  authorizeRole("Admin", "Sales"),
  ctrl.delete
);

MessageRoutes.post("/", ctrl.create);

export default MessageRoutes;
