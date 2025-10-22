import express from "express";
import cors from "cors";
import MessageRoutes from "./modules/messaje/message.routes";
import { carRoutes } from "./modules/car/car.route";
import UserRouter from "./modules/user/user.routes";
import conditionsRoutes from "./modules/car/conditions/conditions.routes";
import authRoutes from "./modules/auth/auth.route";
import transmissionsRoutes from "./modules/car/transmissions/transmissions.routes";
import uploadRoutes from "./modules/upload/upload.routes";
import rolesRoutes from "./modules/user/roles/roles.routes";
const app = express();

app.use(cors());
app.use(express.json());

app.use("/api/users", UserRouter);
app.use("/api/messages", MessageRoutes);
app.use("/api/cars", carRoutes);

app.use("/api/transmissions", transmissionsRoutes);
app.use("/api/conditions", conditionsRoutes);

app.use("/api/roles", rolesRoutes);

app.use("/api/auth", authRoutes);

app.use("/api/upload", uploadRoutes);

export default app;
