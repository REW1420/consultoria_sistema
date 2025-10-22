import { Router } from "express";
import { UserController } from "./user.controller";
import { authenticate, authorizeRole } from "../../middleware/auth.middleware";

const UserRouter = Router();
const ctrl = new UserController();

UserRouter.get("/", authenticate, authorizeRole("Admin"), ctrl.getAll);
UserRouter.get("/:id", authenticate, authorizeRole("Admin"), ctrl.getOne);
UserRouter.post("/", authenticate, authorizeRole("Admin"), ctrl.create);
UserRouter.put("/:id", authenticate, authorizeRole("Admin"), ctrl.update);
UserRouter.delete("/:id", authenticate, authorizeRole("Admin"), ctrl.delete);

export default UserRouter;
