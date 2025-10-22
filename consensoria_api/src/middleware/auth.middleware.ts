import { Request, Response, NextFunction } from "express";
import jwt from "jsonwebtoken";

const JWT_SECRET = process.env.JWT_SECRET || "super_secret_key";

export function authenticate(req: Request, res: Response, next: NextFunction) {
  const authHeader = req.headers.authorization;
  console.log("Auth Header:", authHeader);
  if (!authHeader)
    return res.status(401).json({ message: "Token not provided" });

  const token = authHeader.split(" ")[1];
  try {
    const decoded = jwt.verify(token, JWT_SECRET) as {
      id: number;
      username: string;
      role: string;
    };

    (req as any).user = decoded;
    next();
  } catch (err) {
    res.status(401).json({ message: "Invalid or expired token" });
  }
}

/**
 * 🔒 Middleware para restringir por roles
 * Ejemplo: authorizeRole("Administrator", "Sales")
 */
export function authorizeRole(...allowedRoles: string[]) {
  return (req: Request, res: Response, next: NextFunction) => {
    const user = (req as any).user;
    if (!user)
      return res.status(401).json({ message: "User not authenticated" });

    if (!allowedRoles.includes(user.role))
      return res
        .status(403)
        .json({ message: `Access denied for role: ${user.role}` });

    next();
  };
}
