import { Request, Response } from "express";
import { validateOrReject } from "class-validator";
import { CreateUserDto } from "./user.dto";
import { UserService } from "./user.service";

const service = new UserService();

export class UserController {
  async getAll(req: Request, res: Response) {
    res.json(await service.findAll());
  }

  async getOne(req: Request, res: Response) {
    const id = Number(req.params.id);
    const user = await service.findById(id);
    if (!user) return res.status(404).json({ message: "User not found" });
    res.json(user);
  }

  async create(req: Request, res: Response) {
    const dto = Object.assign(new CreateUserDto(), req.body);
    console.log("Creating user with data:", dto);
    await validateOrReject(dto);
    const user = await service.create(dto);
    res.status(201).json(user);
  }

  async update(req: Request, res: Response) {
    const id = Number(req.params.id);
    const user = await service.update(id, req.body);
    res.json(user);
  }

  async delete(req: Request, res: Response) {
    const id = Number(req.params.id);
    await service.delete(id);
    res.status(204).send();
  }
}
