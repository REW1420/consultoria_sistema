import { Request, Response } from "express";
import { validateOrReject } from "class-validator";
import { CreateMessageDto } from "./message.dto";
import { MessageService } from "./message.service";

const service = new MessageService();

export class MessageController {
  async getAll(req: Request, res: Response) {
    res.json(await service.findAll());
  }

  async getOne(req: Request, res: Response) {
    const id = Number(req.params.id);
    const msg = await service.findById(id);
    if (!msg) return res.status(404).json({ message: "Message not found" });
    res.json(msg);
  }

  async create(req: Request, res: Response) {
    const dto = Object.assign(new CreateMessageDto(), req.body);
    await validateOrReject(dto);
    const message = await service.create(dto);
    res.status(201).json(message);
  }

  async markAsRead(req: Request, res: Response) {
    const id = Number(req.params.id);
    const message = await service.markAsRead(id);
    res.json(message);
  }

  async delete(req: Request, res: Response) {
    const id = Number(req.params.id);
    await service.delete(id);
    res.status(204).send();
  }
}
