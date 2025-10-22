import { Request, Response } from "express";
import { AuthService } from "./auth.service";

const service = new AuthService();

export async function login(req: Request, res: Response) {
  try {
    const { username, password } = req.body;
    const result = await service.login(username, password);
    res.json(result);
  } catch (error: any) {
    res.status(401).json({ message: error.message });
  }
}
