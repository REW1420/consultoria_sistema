import { Router } from "express";
import { login } from "./auth.controller";

const authRoutes = Router();

authRoutes.post("/login", login);

export default authRoutes;
